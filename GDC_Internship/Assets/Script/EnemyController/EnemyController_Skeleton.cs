using System.Collections;
using UnityEngine;

public class EnemyController_Skeleton : MonoBehaviour
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

    [SerializeField] private GameObject Herb;
    [SerializeField] private Transform SlashPoint;
    [SerializeField] private GameObject Slash;
    [SerializeField] private float argoRange = 2f;

    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioSource StepSound;
    [SerializeField] private AudioClip SoundHurt;
    [SerializeField] private AudioClip SoundArgo;
    [SerializeField] private AudioClip SoundAttack;
    [SerializeField] private AudioClip SoundDie;

    private float SmoothMovement = 0.05f;
    private bool hadap_kanan = true;
    private bool onGround = true;
    private float direction = 1f;
    private bool AttackCheck = false;
    private bool StunCheck = false;
    private Coroutine StunTimer = null;
    private float targetDistance;
    private bool IsMoving = false;

    private Transform HealthBar;
    private float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        dummyTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb2d = this.GetComponent<Rigidbody2D>();
        selfTransform = this.GetComponent<Transform>();

        HealthBar = transform.Find("HealthBar");
        maxHealth = health;
        StartCoroutine(Step());
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
                Audio.PlayOneShot(SoundArgo);
            }
        }
        if (target != null)
        {
            targetDistance = Vector2.Distance(selfTransform.position, target.position);
            if (targetDistance <= hitRange && !StunCheck && !AttackCheck && animator.GetFloat("speed") == 0f)
            {
                animator.SetBool("attack", true);
                StartCoroutine("Attacking");
            }
            else if (targetDistance >= argoRange * 1.5)
            {
                dummyTarget = target;
                target = null;
                animator.SetFloat("speed", 0f);
            }
        }
        if (animator.GetFloat("speed") > 0 && !StepSound.isPlaying)
        {
            IsMoving = true;
        }
        else if (animator.GetFloat("speed") <= 0.01 && StepSound.isPlaying)
        {
            IsMoving = false;
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
    }
    void CloseDistance()
    {
        if (onGround && (targetDistance > hitRange))
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
    IEnumerator Step()
    {
        while (true)
        {
            if (IsMoving)
            {
                yield return new WaitForSeconds(0.3f);
                StepSound.Play();
            }
            else
            {
                yield return null;
            }
        }
    }
    IEnumerator Hurt()
    {
        rb2d.velocity = Vector2.zero;
        rb2d.AddForce(new Vector2(-100*direction, -10));
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

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack"))
        {
            yield return null;
        }
        Audio.PlayOneShot(SoundAttack);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            yield return null;
        }
        if (!StunCheck)
        {
            Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
            GameObject clone = Instantiate(Slash, SlashPoint.position, Slash.transform.rotation);
            clone.transform.localScale *= SlashDirection.x;
            clone.GetComponent<Rigidbody2D>().velocity = SlashDirection * 1.5f;
        }
        else
        {
            AttackCheck = false;
            animator.SetBool("attack", false);
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        animator.SetBool("attack", false);
        yield return new WaitForSeconds(0.7f);
        AttackCheck = false;
        
    }
    IEnumerator Stunned()
    {
        Audio.PlayOneShot(SoundHurt);
        animator.SetTrigger("isHurt");
        animator.SetBool("stunned", true);
        StartCoroutine(Hurt());
        StunCheck = true;
        float counter = 0.5f;
        while (counter > 0)
        {
            yield return new WaitForSeconds(0.5f);
            counter = counter - 0.5f;
        }
        if (health <= 0)
        {
            Audio.PlayOneShot(SoundDie);
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
            yield return new WaitForSeconds(2f);
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
            StunCheck = false;
            animator.SetBool("stunned", false);
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
        if (collision.gameObject.tag == "Attack")
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
