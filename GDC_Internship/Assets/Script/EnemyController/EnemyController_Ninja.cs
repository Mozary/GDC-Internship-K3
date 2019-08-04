using System.Collections;
using UnityEngine;

public class EnemyController_Ninja : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb2d;
    private Transform target;
    private Transform dummyTarget;
    private Vector3 m_Velocity = Vector3.zero;
    private ParticleSystem.MainModule ParticleSetting;

    [SerializeField] private ParticleSystem Particle;
    [SerializeField] private TrailRenderer Trail;
    [SerializeField] private EnemyController_Priest PriestPartner;
    [SerializeField] private Transform SlashPoint;
    [SerializeField] private GameObject Slash;
    [SerializeField] private GameObject Herb;
    [SerializeField] private GameObject Smoke;


    [SerializeField] private float maxSpeed;
    [SerializeField] private float hitRange;
    [SerializeField] private float health;
    [SerializeField] private float argoRange;

    
    private float SmoothMovement = 0.05f;
    private float direction = 1f;

    private bool hadap_kanan = true;
    private bool AttackCheck = false;
    private bool StunCheck = false;
    private bool SeekingHeal = false;
    private bool Invulnerable = false;
    private bool Immobile = false;
    private bool CanSpecial = true;

    private Coroutine StunTimer = null;
    private float targetDistance;

    private Transform HealthBar;
    private float maxHealth;
    private float PartnerDistance;

    // Start is called before the first frame update
    void Start()
    {
        dummyTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb2d = this.GetComponent<Rigidbody2D>();
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
            float dummyDistance = Vector2.Distance(transform.position, dummyTarget.position);
            if (dummyDistance <= argoRange)
            {
                target = dummyTarget;
            }
        }
        if (PriestPartner != null)
        {
            PartnerDistance = Vector2.Distance(PriestPartner.transform.position, transform.position);
        }

        if (target != null)
        {
            targetDistance = Vector2.Distance(transform.position, target.position);
            if (targetDistance <= hitRange &&!SeekingHeal && !StunCheck && !AttackCheck && animator.GetFloat("speed") == 0f)
            {
                animator.SetBool("attack", true);
                StartCoroutine("Attacking");
            }
            else if(targetDistance > hitRange*4 && !SeekingHeal && !StunCheck && !AttackCheck && !Immobile && CanSpecial)
            {
                if (Random.Range(0, 100) > 97)
                {
                    CanSpecial = false;
                    StartCoroutine("Special");
                }
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
        else if(rb2d != null && target != null && (AttackCheck || StunCheck))
        {
            rb2d.velocity = Vector2.zero;
            animator.SetFloat("speed", 0f);
        }

    }
    void CloseDistance()
    {
        if (SeekingHeal && targetDistance>0.3f)
        {
            Vector3 moveVelocity = new Vector2(direction * maxSpeed * Time.deltaTime, rb2d.velocity.y);
            animator.SetFloat("speed", Mathf.Abs(moveVelocity.x));
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, moveVelocity, ref m_Velocity, SmoothMovement);
        }
        else if (targetDistance > hitRange && !SeekingHeal)
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

    IEnumerator Special()
    {
        Debug.Log("SPECIAL");
        Immobile = true;
        AttackCheck = true;
        animator.SetBool("special", true);
        float wait = 3f;
        Particle.Play();
        while (wait > 0)
        {
            wait -= Time.deltaTime;
            if (StunCheck)
            {
                Particle.Stop();
                animator.SetBool("special", false);
                CanSpecial = true;
                Immobile = false;
                AttackCheck = false;
                yield break;
            }
            yield return null;
        }
        Particle.Stop();
        animator.SetBool("special", false);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("NinjaSpecial") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime <0.5f)
        {
            yield return null;
        }

        Invulnerable = true;
        Vector3 tptarget = target.position;
        tptarget.y += 0.08f;
        if (target.GetComponent<CharacterController2D>().IsFacingRight())
        {
            tptarget.x -= 0.2f;
        }
        else
        {
            tptarget.x += 0.2f;
        }

        transform.position = tptarget;
        Instantiate(Smoke, transform.position, transform.rotation);
        if (target.position.x > transform.position.x && !hadap_kanan) Flip();
        else if (target.position.x < transform.position.x && hadap_kanan) Flip();

        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < .75f)
        {
            yield return null;
        }
        Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
        GameObject clone = Instantiate(Slash, SlashPoint.position, Slash.transform.rotation);
        clone.transform.localScale *= SlashDirection.x;
        clone.GetComponent<Rigidbody2D>().velocity = SlashDirection * 1f;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        Invulnerable = false;
        Immobile = false;
        AttackCheck = false;

        yield return new WaitForSeconds(15f);
        CanSpecial = true;
        
    }
    IEnumerator Attacking()
    {
        AttackCheck = true;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("EnemyAttack"))
        {
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
        {
            if (StunCheck)
            {
                AttackCheck = false;
                animator.SetBool("attack", false);
                yield break;
            }
            yield return null;
        }
        Invulnerable = true;
        Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
        GameObject clone = Instantiate(Slash, SlashPoint.position, Slash.transform.rotation);
        clone.transform.localScale *= SlashDirection.x;
        clone.GetComponent<Rigidbody2D>().velocity = SlashDirection * 1f;
        
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        
        animator.SetBool("attack", false);
        Invulnerable = false;
        yield return new WaitForSeconds(0.5f);
        AttackCheck = false;
    }
    IEnumerator Hurt()
    {
        if (Random.Range(1, 100) >= 60 && health/maxHealth <0.5f)
        {
            if (PriestPartner.AskForHeal())
            {
                target = PriestPartner.transform.Find("HealPoint").transform;
                if (PartnerDistance > 0.75f)
                {
                    StartCoroutine(TeleportForHeal());
                }
                SeekingHeal = true;
            }
        }

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
            rb2d.AddForce(new Vector2(-70*direction, 0f));
            yield return new WaitForSeconds(0.25f);
            StunCheck = false;
            animator.SetBool("stunned", false);
        }
    }

    IEnumerator Healing()
    {
        AttackCheck = true;
        target = dummyTarget;
        SeekingHeal = false;
        float HealCounter = 3f;
        while (HealCounter > 0)
        {
            if (StunCheck)
            {
                ResetTarget();
                AttackCheck = false;
                yield break;
            }
            else
            {
                health += Time.deltaTime * 1.5f;
                HealCounter -= Time.deltaTime;
                yield return null;
            }
        }
        AttackCheck = false;
    }
    IEnumerator TeleportForHeal()
    {
        AttackCheck = true;
        Invulnerable = true;
        animator.SetTrigger("teleport");
        Debug.Log("TELEPORTING");
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Teleport"))
        {
            AttackCheck = true;
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5)
        {
            AttackCheck = true;
            yield return null;
        }
        transform.position = PriestPartner.transform.Find("HealPoint").transform.position;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            AttackCheck = true;
            yield return null;
        }
        AttackCheck = false;
        Invulnerable = false;

    }
    public void ResetTarget()
    {
        target = dummyTarget;
    }
    public void AskForBodySwap()
    {
        if (PartnerDistance > 1f && !StunCheck && !AttackCheck && !Immobile)
        {
            Debug.Log("SWAPING");

            Instantiate(Smoke, PriestPartner.transform.position, PriestPartner.transform.rotation);
            Instantiate(Smoke, transform.position, transform.rotation);

            Vector3 temp = PriestPartner.transform.position;
            PriestPartner.transform.position = transform.position;
            transform.position = temp;
        }
    }
    public void Heal()
    {
        Debug.Log("RECIEVED HEALING");
        StartCoroutine(Healing());
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
