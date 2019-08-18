using System.Collections;
using UnityEngine;

public class EnemyController_Priest : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb2d;
    private Transform dummyTarget;
    private Transform target = null;
    private Vector3 m_Velocity = Vector3.zero;

    public float maxSpeed;
    public float hitRange;
    public float health;

    [SerializeField] private EnemyController_Ninja NinjaPartner;
    [SerializeField] private GameObject Herb;
    [SerializeField] private Transform SpellPoint;
    [SerializeField] private GameObject Iceshard;
    [SerializeField] private GameObject Blind;
    [SerializeField] private GameObject Heal;
    [SerializeField] private GameObject Deathplode;
    [SerializeField] private float argoRange = 2f;

    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioClip SoundHurt;
    [SerializeField] private AudioClip SoundArgo;
    [SerializeField] private AudioClip SoundDie;
    [SerializeField] private AudioClip SoundConfirm;
    [SerializeField] private AudioClip SoundHeal;

    private float SmoothMovement = 0.05f;
    private bool hadap_kanan = true;
    private bool Invulnerable = false;
    private float direction = 1f;
    private bool AttackCheck = false;
    private bool StunCheck = false;
    private float targetDistance;
    private float PartnerDistance;

    private Coroutine StunTimer = null;
    private Transform HealthBar;
    private float maxHealth;
    private bool AskedForHeal = false;

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
                Audio.PlayOneShot(SoundArgo);
                target = dummyTarget;
                dummyTarget = null;
            }
        }
        if(NinjaPartner != null)
        {
            PartnerDistance = Vector2.Distance(NinjaPartner.transform.position, transform.position);
        }
        if (target != null)
        {
            targetDistance = Vector2.Distance(transform.position, target.position);
            if (targetDistance <= hitRange &&!AskedForHeal && !StunCheck && !AttackCheck && animator.GetFloat("speed") == 0f)
            {
                animator.SetBool("attack", true);
                StartCoroutine("Attacking");
            }
            else if(AskedForHeal && !StunCheck && !AttackCheck && animator.GetFloat("speed") == 0f)
            {
                Debug.Log("REQUEST ACCEPTED");
                AskedForHeal = false;
                StartCoroutine(Healing());
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
    IEnumerator Healing()
    {
        animator.SetBool("attack", true);
        AttackCheck = true;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("SpellCharge"))
        {
            if (StunCheck)
            {
                AttackCheck = false;
                NinjaPartner.ResetTarget();
                animator.SetBool("attack", false);
                yield break;
            }
            yield return null;
        }
        GameObject HealCharge = Instantiate(Heal, SpellPoint.position, SpellPoint.transform.rotation);
        float PrepareTime = 3f;
        Audio.PlayOneShot(SoundConfirm);
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f || PartnerDistance>=0.5f)
        {
            if (StunCheck || !NinjaPartner || HealCharge == false)
            {
                Invulnerable = false;
                AttackCheck = false;
                NinjaPartner.ResetTarget();
                if (HealCharge == true)
                {
                    Destroy(HealCharge);
                }
                animator.SetBool("attack", false);
                yield break;
            }
            yield return null;
        }
        while (PrepareTime > 0f)
        {
            if (StunCheck || !NinjaPartner || HealCharge == false)
            {
                Invulnerable = false;
                AttackCheck = false;
                NinjaPartner.ResetTarget();
                if (HealCharge == true)
                {
                    Destroy(HealCharge);
                }
                animator.SetBool("attack", false);
                yield break;
            }
            PrepareTime -= Time.deltaTime;
            yield return null;
        }
        animator.SetBool("attack", false);
        Audio.PlayOneShot(SoundHeal);
        NinjaPartner.Heal();
        HealCharge.GetComponent<HealScript>().Disperse();

        Invulnerable = true;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        Invulnerable = false;
        yield return new WaitForSeconds(2f);
        AttackCheck = false;
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
            if (StunCheck || !target)
            {
                Invulnerable = false;
                AttackCheck = false;
                animator.SetBool("attack", false);
                yield break;
            }
            yield return null;
        }

        animator.SetBool("attack", false);
        Invulnerable = true;
        Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
        GameObject clone =null;
        if (Random.Range(1, 100) <= 80)
        {
            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.1f);
                clone = Instantiate(Iceshard, SpellPoint.position, SpellPoint.transform.rotation);
                clone.GetComponent<AttackSpellScript>().SetDamage(0.25f);
                if (direction < 0)
                {
                    Quaternion rotation = clone.transform.rotation;
                    rotation.z -= 180;
                    clone.transform.rotation = rotation;
                }
            }
        }
        else
        {
            clone = Instantiate(Blind, SpellPoint.position, SpellPoint.transform.rotation);
            if (direction < 0)
            {
                Quaternion rotation = clone.transform.rotation;
                rotation.z -= 180;
                clone.transform.rotation = rotation;
            }
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        Invulnerable = false;
        yield return new WaitForSeconds(2f);
        AttackCheck = false;
    }
    IEnumerator Hurt()
    {
        if (Random.Range(0, 100) > 60)
        {
            NinjaPartner.AskForBodySwap();
        }
        float flashTime = 0.1f;
        Color mycolour = GetComponent<SpriteRenderer>().color;
        mycolour.g = 0f;
        mycolour.b = 0f;
        GetComponent<SpriteRenderer>().color = mycolour;
        yield return new WaitForSeconds(flashTime);
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    IEnumerator Stunned()
    {
        Audio.PlayOneShot(SoundHurt);
        animator.SetTrigger("isHurt");
        StartCoroutine(Hurt());
        StunCheck = true;
        if (health <= 0)
        {
            Audio.PlayOneShot(SoundDie);
            animator.SetTrigger("isDying");
            DropHerb();
            rb2d.isKinematic = true;
            this.GetComponent<Collider2D>().enabled = false;
            while (animator.GetCurrentAnimatorStateInfo(0).IsName("PriestDie"))
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
    public bool AskForHeal()
    {
        if(targetDistance > 1.5f)
        {
            AskedForHeal = true;
            return true;
        }
        else
        {
            return false;
        }
        
    }
}
