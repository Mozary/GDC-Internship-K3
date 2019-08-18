using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Sorcerer : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem particle;
    [SerializeField] private CapsuleCollider2D Slimebody;
    [SerializeField] private CapsuleCollider2D Mainbody;
    [SerializeField] private Rigidbody2D rb2d;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float hitRange;
    [SerializeField] private float health;
    [SerializeField] private float argoRange = 2f;

    [SerializeField] private Transform TeleportPoint1;
    [SerializeField] private Transform TeleportPoint2;
    [SerializeField] private Transform TeleportPoint3;

    [SerializeField] private Transform SpellPoint;
    [SerializeField] private GameObject Herb;

    //Attack Spell
    [SerializeField] private GameObject Fireball;
    [SerializeField] private GameObject Iceshard;
    [SerializeField] private GameObject Blind;

    //Summoned Beasts
    [SerializeField] private GameObject Summon_Gargoyle;
    [SerializeField] private GameObject Summon_Skeleton;

    //Exploding FX
    [SerializeField] private GameObject Deathplode;
    [SerializeField] private GameObject Summonplode;

    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioSource StepSound;
    [SerializeField] private AudioClip SoundHurt;
    [SerializeField] private AudioClip SoundArgo;
    [SerializeField] private AudioClip SoundDie;
    [SerializeField] private AudioClip SoundAttack;
    [SerializeField] private AudioClip SoundToMud;
    [SerializeField] private AudioClip SoundToBody;

    private Coroutine StunTimer = null;
    private Transform HealthBar;
    private Transform dummyTarget;
    private Transform target = null;
    private Vector3 m_Velocity = Vector3.zero;
    private List<GameObject> SummonedBeast = new List<GameObject>();

    private float SmoothMovement = 0.05f;
    private bool hadap_kanan = true;
    private bool Invulnerable = false;
    private float direction = 1f;
    private bool AttackCheck = false;
    private bool StunCheck = false;
    private float targetDistance;
    private float maxHealth;
    private float rageChance = 100;
    private bool isMoving = false;
    

    // Start is called before the first frame update
    void Start()
    {
        dummyTarget = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
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
        if (target != null)
        {
            targetDistance = Vector2.Distance(transform.position, target.position);
            if (targetDistance <= hitRange && !StunCheck && !AttackCheck && animator.GetFloat("speed") == 0f)
            {
                animator.SetBool("attack", true);
                StartCoroutine("Attacking");
            }
        }
        if (!isMoving && animator.GetFloat("speed")>0)
        {
            isMoving = true;
            StepSound.Play();
            Audio.PlayOneShot(SoundToMud);
        }
        else if (isMoving && animator.GetFloat("speed") < 0.01)
        {
            isMoving = false;
            StepSound.Stop();
            Audio.PlayOneShot(SoundToBody);
        }

    }
    void FixedUpdate()
    {
        if (rb2d != null && !AttackCheck && !StunCheck && target != null)
        {
            if (targetDistance > hitRange)
            {
                CloseDistance();
            }
            else
            {
                animator.SetFloat("speed", 0f);
            }

            if (target.position.x > transform.position.x && !hadap_kanan) Flip();
            else if (target.position.x < transform.position.x && hadap_kanan) Flip();
        }
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ShiftToSorcerer") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.7f && Mainbody.enabled==false)
        {
            Mainbody.enabled = true;
        }
    }
    void CloseDistance()
    {
        Vector3 moveVelocity = new Vector2(direction * maxSpeed * Time.deltaTime, rb2d.velocity.y);
        animator.SetFloat("speed", Mathf.Abs(moveVelocity.x));
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ShiftToShadow") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            if (Mainbody.enabled)
            {
                Mainbody.enabled = false;
            }
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, moveVelocity, ref m_Velocity, SmoothMovement);
        }
    }
    private void Flip()
    {
        hadap_kanan = !hadap_kanan;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        direction *= -1;
    }
    IEnumerator Teleport()
    {
        Invulnerable = true;
        StunCheck = true;
        animator.SetFloat("speed", 3);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("ShiftToShadow")) {
            animator.SetFloat("speed", 3);
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            animator.SetFloat("speed", 3);
            yield return null;
        }

        float rand = Random.Range(0, 100);
        if (rand<30)
        {
            transform.position = TeleportPoint1.position;
        }
        else if (rand >70)
        {
            transform.position = TeleportPoint2.position;
        }
        else
        {
            transform.position = TeleportPoint3.position;
        }
        animator.SetFloat("speed", 0);

        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("ShiftToSorcerer")) {
            animator.SetFloat("speed", 0);
            yield return null;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f) {
            animator.SetFloat("speed", 0);
            yield return null;
        }
        StunCheck = false;
        Invulnerable = false;
    }
    IEnumerator Attacking()
    {
        AttackCheck = true;
        Audio.PlayOneShot(SoundAttack);
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

        for (var i = SummonedBeast.Count - 1; i > -1; i--)
        {
            if (SummonedBeast[i] == null)
                SummonedBeast.RemoveAt(i);
        }
        if (Random.Range(0, 100) > 60 && SummonedBeast.Count <4)
        {
            GameObject beast = null;
            float rand = Random.Range(0, 100);
            Instantiate(Summonplode, SpellPoint.position, SpellPoint.transform.rotation);
            if (rand < 70 && Summon_Skeleton)
            {
                beast = Instantiate(Summon_Skeleton, SpellPoint.position, SpellPoint.transform.rotation);
            }
            else
            {
                beast = Instantiate(Summon_Gargoyle, SpellPoint.position, SpellPoint.transform.rotation);
                beast.GetComponent<EnemyController_Gargoyle>().SetAsSummoned();
                beast.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                beast.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, 50));
            }

            SummonedBeast.Add(beast);
        }
        else
        {
            GameObject spell = null;
            float SpellRand = Random.Range(1, 100);
            if (SpellRand < 40)
            {
                spell = Instantiate(Fireball, SpellPoint.position, SpellPoint.transform.rotation);
            }
            else if(SpellRand>60)
            {
                spell = Instantiate(Iceshard, SpellPoint.position, SpellPoint.transform.rotation);
            }
            else
            {
                spell = Instantiate(Blind, SpellPoint.position, SpellPoint.transform.rotation);
            }
            if (direction < 0)
            {
                Quaternion rotation = spell.transform.rotation;
                rotation.z -= 180;
                spell.transform.rotation = rotation;
            }
        }
        
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        Invulnerable = false;
        yield return new WaitForSeconds(1.5f);
        AttackCheck = false;
        
    }
    IEnumerator Hurt()
    {
        if (Random.Range(0, 100) < rageChance && TeleportPoint1 && TeleportPoint2 && TeleportPoint3)
        {
            particle.Play();
            if (target && targetDistance < 0.7)
            {
                target.GetComponent<PlayerController>().TakeDamage(0.2f);
                float knockforceX = 100;
                if (target.transform.position.x < transform.position.x)
                {
                    knockforceX = -1 * knockforceX;
                }
                target.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                target.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockforceX, -10));
            }
            StartCoroutine(Teleport());
            rageChance = 0;
        }
        else
        {
            rageChance += 2f;
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
        StartCoroutine(Hurt());
        StunCheck = true;
        if (health <= 0)
        {
            DropHerb(); DropHerb(); DropHerb(); DropHerb(); DropHerb(); DropHerb(); DropHerb();
            Audio.PlayOneShot(SoundDie);
            rb2d.isKinematic = true;
            Mainbody.enabled= false;
            Slimebody.enabled = false;
            transform.Find("Trail").gameObject.SetActive(false);
            GameObject.Find("PlayerHUDCanvas").GetComponent<BoardManager>().BossIsDefeated();
            GameObject Explosion = Instantiate(Deathplode, transform.position, transform.rotation);
            yield return null;
            while (Explosion)
            {
                if ( transform.localScale.x  > 0)
                {
                    transform.Rotate(Vector3.forward*30);
                    transform.localScale -= new Vector3(0.025f, 0.025f, 0.025f);
                    if (transform.localScale.x < 0) { transform.localScale = Vector3.zero;}
                }
                else if (transform.localScale.x < 0)
                {
                    transform.Rotate(Vector3.back * 30);
                    transform.localScale += new Vector3(0.025f, 0.025f, 0.025f);
                    if(transform.localScale.x > 0) { transform.localScale = Vector3.zero;}
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
