using System.Collections;
using UnityEngine;

public class EnemyController_Mage : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb2d;
    private Transform dummyTarget;
    private Transform target = null;
    private Vector3 m_Velocity = Vector3.zero;

    public float maxSpeed;
    public float hitRange;
    public float health;

    [SerializeField] private GameObject Herb;
    [SerializeField] private Transform SpellPoint;
    [SerializeField] private GameObject Spell1;
    [SerializeField] private GameObject Spell2;
    [SerializeField] private GameObject Deathplode;
    [SerializeField] private float argoRange = 2f;

    private float SmoothMovement = 0.05f;
    private bool hadap_kanan = true;
    private bool Invulnerable = false;
    private float direction = 1f;
    private bool AttackCheck = false;
    private bool StunCheck = false;
    private float targetDistance;

    private Coroutine StunTimer = null;
    private Transform HealthBar;
    private float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        dummyTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb2d = this.GetComponent<Rigidbody2D>();

        HealthBar = transform.Find("HealthBar");
        maxHealth = health;
    }

    // Update is called once per frame
    void Update()
    {
        HealthBar.localScale = new Vector3(Mathf.Clamp(health / maxHealth, 0, maxHealth), HealthBar.localScale.y, HealthBar.localScale.z);

        if (target == null && dummyTarget)
        {
            float dummyDistance = Vector2.Distance(transform.position, dummyTarget.position);
            if (dummyDistance <= argoRange)
            {
                target = dummyTarget;
                dummyTarget = null;
            }
        }
        if (target != null)
        {
            targetDistance = Vector2.Distance(transform.position, target.position);
            if (targetDistance <= hitRange && !StunCheck && !AttackCheck && animator.GetFloat("speed") == 0f)
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
            if (target.position.x > transform.position.x && !hadap_kanan) Flip();
            else if (target.position.x < transform.position.x && hadap_kanan) Flip();
        }
    }
    void CloseDistance()
    {
        if (targetDistance > hitRange)
        {
            Vector3 moveVelocity = new Vector2(direction * maxSpeed * Time.deltaTime, rb2d.velocity.y);
            animator.SetFloat("speed", Mathf.Abs(moveVelocity.x));
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, moveVelocity, ref m_Velocity, SmoothMovement);
        } else { animator.SetFloat("speed", 0f); }
    }
    private void Flip()
    {
        hadap_kanan = !hadap_kanan;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        direction *= -1;
    }
    IEnumerator Hurt()
    {
        float flashTime = 0.1f;
        Color mycolour = GetComponent<SpriteRenderer>().color;
        mycolour.g = 0f;
        mycolour.b = 0f;
        GetComponent<SpriteRenderer>().color = mycolour;
        while (flashTime > 0)
        {
            yield return new WaitForSeconds(flashTime);
            flashTime = 0;
        }
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    IEnumerator Attacking()
    {
        AttackCheck = true;
        //---Charging---//
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SpellCharge"))
        {
            if (StunCheck)
            {
                AttackCheck = false;
                animator.SetBool("attack", false);
                yield break;
            }
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            if (StunCheck)
            {
                AttackCheck = false;
                animator.SetBool("attack", false);
                yield break;
            }
            yield return null;
        }
        animator.SetBool("attack", false);
        Invulnerable = true;
        if (!target)
        {
            Invulnerable = false;
            AttackCheck = false;
            yield break;
        }
        Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
        GameObject clone =null;
        if (Random.Range(1, 100) <= 50)
        {
            clone = Instantiate(Spell1, SpellPoint.position, SpellPoint.transform.rotation);
        }
        else
        {
            clone = Instantiate(Spell2, SpellPoint.position, SpellPoint.transform.rotation);
        }
        clone.transform.localScale *= SlashDirection.x;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        Invulnerable = false;
        AttackCheck = false;
    }
    IEnumerator Stunned()
    {
        StartCoroutine(Hurt());
        StunCheck = true;
        if (health <= 0)
        {
            DropHerb();
            rb2d.isKinematic = true;
            this.GetComponent<Collider2D>().enabled = false;

            GameObject Explosion = Instantiate(Deathplode, transform.position, transform.rotation);
            yield return null;
            while (Explosion.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime < 0.9f)
            {
                if (transform.localScale.x > 0)
                {
                    transform.Rotate(Vector3.forward*30);
                    transform.localScale -= new Vector3(0.025f, 0.025f, 0.025f);
                }
                yield return null;
            }
            Destroy(gameObject);
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
            StunCheck = false;
        }
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
            StunTimer = StartCoroutine(Stunned());
        }
    }
}
