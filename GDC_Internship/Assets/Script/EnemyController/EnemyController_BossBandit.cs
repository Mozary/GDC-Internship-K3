using System.Collections;
using UnityEngine;

public class EnemyController_BossBandit : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb2d;
    private Transform dummyTarget;
    private Transform target;
    private Transform selfTransform;
    private Vector3 m_Velocity = Vector3.zero;
    private ParticleSystem.MainModule ParticleSetting;

    [SerializeField] private ParticleSystem Particle;
    [SerializeField] private TrailRenderer Trail;

    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioClip SoundDeath;
    [SerializeField] private AudioClip SoundHurt;
    [SerializeField] private AudioClip SoundDodge;
    [SerializeField] private AudioClip SoundAttack;
    [SerializeField] private AudioClip SoundArgo;

    public float maxSpeed;
    public float hitRange;
    public float health;

    [SerializeField] private Transform SlashPoint;
    [SerializeField] private GameObject Slash;
    [SerializeField] private GameObject Herb;
    [SerializeField] private float argoRange = 2f;

    private float SmoothMovement = 0.05f;
    private bool hadap_kanan = true;
    private bool onGround = true;
    private float direction = 1f;
    private bool AttackCheck = false;
    private bool StunCheck = false;
    private bool counterCheck = false;
    private bool Invulnerable = false;

    private Coroutine StunTimer = null;
    private float targetDistance;

    private Transform HealthBar;
    private float maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        dummyTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb2d = this.GetComponent<Rigidbody2D>();
        selfTransform = this.GetComponent<Transform>();
        ParticleSetting = Particle.main;

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
                Audio.PlayOneShot(SoundArgo);
                target = dummyTarget;
                dummyTarget = null;
            }
        }
        if (target != null)
        {
            targetDistance = Vector2.Distance(selfTransform.position, target.position);
            if (targetDistance <= hitRange && !StunCheck && !AttackCheck && !counterCheck && animator.GetFloat("speed") == 0f)
            {
                if (Random.Range(0, 100) < 15)
                {
                    animator.SetBool("onGuard", true);
                }
                else
                {
                    animator.SetBool("attack", true);
                    StartCoroutine("Attacking");
                }
            }

        }

    }
    void FixedUpdate()
    {   
        if (rb2d != null && !AttackCheck && !StunCheck && !counterCheck && target != null)
        {
            CloseDistance();
            if (target.position.x > selfTransform.position.x && !hadap_kanan) Flip();
            else if (target.position.x < selfTransform.position.x && hadap_kanan) Flip();
        }
        else if(rb2d != null && target != null && (AttackCheck || StunCheck || counterCheck))
        {
            //Debug.Log(AttackCheck+","+StunCheck+","+counterCheck);
            animator.SetFloat("speed", 0f);
        }
        if (!counterCheck && animator.GetBool("onGuard") && !AttackCheck && !StunCheck)
        {
            counterCheck = true;
            StartCoroutine(EnGarde());
        }

    }
    void CloseDistance()
    {
        if (onGround && (targetDistance > hitRange))
        {
            Vector3 moveVelocity = new Vector2(direction * maxSpeed * Time.deltaTime, rb2d.velocity.y);
            animator.SetFloat("speed", Mathf.Abs(moveVelocity.x));
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, moveVelocity, ref m_Velocity, SmoothMovement);
        }
        else
        {
            animator.SetFloat("speed", 0f);
        }
    }
    private void DropHerb()
    {
        Vector3 DropDirection = new Vector3(-transform.localScale.x, 1, 0).normalized;
        GameObject clone = Instantiate(Herb, transform.position, transform.rotation);
        clone.GetComponent<Rigidbody2D>().velocity = DropDirection * 1f;
    }

    private void Flip()
    {
        hadap_kanan = !hadap_kanan;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        direction *= -1;
    }
    IEnumerator EnGarde()
    {
        Debug.Log("ENGARDE!");
        Invulnerable = true;
        AttackCheck = true;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("BossBandit_Dodge"))
        {
            yield return null;
        }
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("BossBandit_Dodge") && !animator.GetCurrentAnimatorStateInfo(0).IsName("BossBandit_Guard"))
        {
            animator.SetBool("onGuard", false);
            counterCheck = false;
            yield break;
        }

        rb2d.mass = 0.2f;
        rb2d.gravityScale = 4;

        Trail.emitting = true;
        float direction = Mathf.Abs(transform.localScale.x) / transform.localScale.x;
        rb2d.AddForce(new Vector2(direction * -70, -5f));
        Audio.PlayOneShot(SoundDodge);
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("BossBandit_Dodge"))
        {
            yield return null;
        }

        transform.Find("Special").gameObject.SetActive(true);
        Trail.emitting = false;
        Particle.Play();

        Invulnerable = false;
        float counter = 0f;
        while (targetDistance >= hitRange * 7 && counter < 1.5f)
        {
            rb2d.mass = 5f;
            if (StunCheck)
            {
                rb2d.mass = 1f;
                rb2d.gravityScale = 1f;

                animator.SetBool("onGuard", false);
                AttackCheck = false;
                counterCheck = false;
                Trail.emitting = false;
                Particle.Stop();
                yield break;
            }
            counter += Time.deltaTime;
            yield return null;
        }
        Invulnerable = true;

        Trail.emitting = true;
        Particle.Stop();

        rb2d.mass = 0.15f;
        rb2d.gravityScale = 5f;
        rb2d.drag = 5;

        direction = Mathf.Abs(transform.localScale.x) / transform.localScale.x;
        rb2d.AddForce(new Vector2(direction * 150, -5f));

        animator.SetBool("attack", true);
        StartCoroutine(Attacking());
        animator.SetBool("onGuard", false);
        

        yield return new WaitForSeconds(1.5f);
        rb2d.mass = 1f;
        rb2d.drag = 0;
        rb2d.gravityScale = 1f;
        counterCheck = false;



    }
    IEnumerator Attacking()
    {
        AttackCheck = true;

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack"))
        {
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.25f)
        {
            if (StunCheck)
            {
                AttackCheck = false;
                animator.SetBool("attack", false);
                yield break;
            }
            yield return null;
        }
        Audio.PlayOneShot(SoundAttack);
        Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
        GameObject clone = Instantiate(Slash, SlashPoint.position, Slash.transform.rotation);
        clone.transform.localScale *= SlashDirection.x;
        clone.GetComponent<Rigidbody2D>().velocity = SlashDirection * 1f;

        Invulnerable = true;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        AttackCheck = false;
        animator.SetBool("attack", false);
        Invulnerable = false;
        Trail.emitting = false;
        transform.Find("Special").gameObject.SetActive(false);
    }
    IEnumerator Hurt()
    {
        Audio.PlayOneShot(SoundHurt);
        if (Random.Range(1, 100) >= 75)
        {
            animator.SetBool("onGuard", true);
        }
        Trail.emitting = false;
        Particle.Stop();
        transform.Find("Special").gameObject.SetActive(false);

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
    IEnumerator Stunned()
    {
        animator.SetTrigger("isHurt");
        animator.SetBool("stunned", true);
        StunCheck = true;
        StartCoroutine(Hurt());
        yield return new WaitForSeconds(0.5f);
        if (health <= 0)
        {
            Audio.PlayOneShot(SoundDeath);
            DropHerb();
            DropHerb();
            DropHerb();
            GameObject.Find("PlayerHUDCanvas").GetComponent<BoardManager>().BossIsDefeated();
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

    IEnumerator CallReinforcement()
    {
        while (true)
        {

        }
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
