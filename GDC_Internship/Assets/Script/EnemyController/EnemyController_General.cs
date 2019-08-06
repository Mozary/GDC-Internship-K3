using System.Collections;
using UnityEngine;

public class EnemyController_General : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb2d;
    private Transform dummyTarget;
    private Transform target = null;
    private Transform selfTransform;
    private Vector3 m_Velocity = Vector3.zero;

    public float maxSpeed;
    public float hitRange;
    public float health;

    [SerializeField] private Transform SlashPoint;
    [SerializeField] private GameObject SlashEffect;
    [SerializeField] private GameObject Slash;
    [SerializeField] private float argoRange = 2f;
    [SerializeField] private GameObject Herb;

    private float SmoothMovement = 0.05f;
    private bool hadap_kanan = true;
    private bool onGround = true;
    private float direction = 1f;
    private bool AttackCheck = false;
    private bool StunCheck = false;
    private Coroutine StunTimer = null;
    private float targetDistance;
    private Color originColor;
    private bool Invulnerable = false;

    private bool gotHit = false;
    private bool canSpecial = true;

    private Transform HealthBar;
    private float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        dummyTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb2d = this.GetComponent<Rigidbody2D>();
        selfTransform = this.GetComponent<Transform>();
        originColor = GetComponent<SpriteRenderer>().color;

        HealthBar = transform.Find("HealthBar");
        maxHealth = health;
    }
    // Update is called once per frame
    void Update()
    {
        HealthBar.localScale = new Vector3(Mathf.Clamp(health / maxHealth, 0, maxHealth), HealthBar.localScale.y, HealthBar.localScale.z);
        if (target == null && dummyTarget)
        {
            float dummyDistance = Vector2.Distance(selfTransform.position, dummyTarget.position);
            if (dummyDistance <= argoRange)
            {
                target = dummyTarget;
                dummyTarget = null;
            }
        }
        if (target != null)
        {
            targetDistance = Vector2.Distance(selfTransform.position, target.position);
            if ((targetDistance > hitRange*3 && targetDistance<argoRange) && !StunCheck && !AttackCheck && canSpecial)
            {
                canSpecial = false;
                if (Random.Range(0,100)>60 )
                {
                    StartCoroutine("Special");
                }
                else
                {
                    StartCoroutine(DelayAttempt());
                }
            }
            else if (targetDistance <= hitRange && !StunCheck && !AttackCheck && animator.GetFloat("speed") == 0f)
            {
                animator.SetBool("attack", true);
                StartCoroutine("Attacking");
            }
        }
    }
    void FixedUpdate()
    {
        if (rb2d != null && !AttackCheck && !StunCheck && target != null)
        {
            CloseDistance();
            if (target.position.x > selfTransform.position.x && !hadap_kanan) Flip();
            else if (target.position.x < selfTransform.position.x && hadap_kanan) Flip();
        }
        else
        {
            animator.SetFloat("speed", 0f);
        }
    }
    void CloseDistance()
    {
        if (targetDistance > hitRange)
        {
            Vector3 moveVelocity = new Vector2(direction * maxSpeed * Time.deltaTime, rb2d.velocity.y);
            animator.SetFloat("speed", Mathf.Abs(moveVelocity.x));
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, moveVelocity, ref m_Velocity, SmoothMovement);
        }
        else { animator.SetFloat("speed", 0f); }
    }
    private void Flip()
    {
        hadap_kanan = !hadap_kanan;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        direction *= -1;
    }
    IEnumerator DelayAttempt()
    {
        yield return new WaitForSeconds(2f);
        canSpecial = true;
    }
    IEnumerator Special()
    {
        animator.SetTrigger("special");
        AttackCheck = true;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("GeneralPrepare") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime <1)
        {
            if (StunCheck)
            {
                animator.SetBool("attack", false);
                AttackCheck = false;
                yield break;
            }
            yield return null;
        }
        yield return new WaitForSeconds(1f);
        Invulnerable = true;
        Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
        animator.SetBool("attack", true);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < .5)
        {
            yield return null;
        }
        Vector3 UnleashPoint = SlashPoint.position;
        UnleashPoint.y += 0.2f;
        GameObject fx = Instantiate(SlashEffect, UnleashPoint, transform.rotation);
        fx.GetComponent<SpecialSlashScript>().Unleash(SlashDirection);
        Invulnerable = false;
        yield return new WaitForSeconds(0.7f);
        AttackCheck = false;
        animator.SetBool("attack", false);

        yield return new WaitForSeconds(7f);
        canSpecial = true;
    }
    IEnumerator Attacking()
    {
        AttackCheck = true;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime < .35f)
        {
            if (StunCheck)
            {
                animator.SetBool("attack", false);
                AttackCheck = false;
                yield break;
            }
            yield return null;
        }
        Invulnerable = true;
        Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
        GameObject clone = Instantiate(Slash, SlashPoint.position, Slash.transform.rotation);
        clone.transform.localScale *= SlashDirection.x;
        clone.GetComponent<Rigidbody2D>().velocity = SlashDirection * 2f;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        Invulnerable = false;
        animator.SetBool("attack", false);
        yield return new WaitForSeconds(0.5f);
        AttackCheck = false;
    }
    IEnumerator Stunned()
    {
        animator.SetTrigger("isHurt");
        StunCheck = true;
        StartCoroutine(Hurt());
        if (health <= 0)
        {
            DropHerb();
            DropHerb();
            animator.SetTrigger("isDying");
            rb2d.isKinematic = true;
            this.GetComponent<Collider2D>().enabled = false;
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyDie"))
            {
                yield return null;
            }
            while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }
            Color mycolour = GetComponent<SpriteRenderer>().color;
            float colourfade = 1;
            while (colourfade > 0)
            {
                colourfade -= 0.03f;
                mycolour.a = colourfade;
                GetComponent<SpriteRenderer>().color = mycolour;
                yield return null;
            }
            Destroy(this.gameObject);
        }
        else
        {
            yield return new WaitForSeconds(1f);
            StunCheck = false;
            animator.SetBool("stunned", false);
        }
    }
    IEnumerator Hurt()
    {
        float flashTime = 0.05f;
        Color mycolour = GetComponent<SpriteRenderer>().color;
        mycolour.g = 0f;
        mycolour.b = 0f;
        GetComponent<SpriteRenderer>().color = mycolour;
        while (flashTime > 0)
        {
            yield return new WaitForSeconds(flashTime);
            flashTime = 0;
        }
        gotHit = false;
        GetComponent<SpriteRenderer>().color = originColor;
    }
    private void DropHerb()
    {
        Vector3 DropDirection = new Vector3(-transform.localScale.x, 1, 0).normalized;
        GameObject clone = Instantiate(Herb, transform.position, transform.rotation);
        clone.GetComponent<Rigidbody2D>().velocity = DropDirection * 1f;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack" && !Invulnerable)
        {
            if (StunTimer != null)
            {
                StopCoroutine(StunTimer);
            }
            this.health = this.health - 1;
            StartCoroutine(Stunned());
        }
    }
}
