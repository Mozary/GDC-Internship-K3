﻿using System.Collections;
using UnityEngine;

public class EnemyController_Knight : MonoBehaviour
{
    public Animator animator;
    private Rigidbody2D rb2d;
    private Transform target;
    private Transform selfTransform;
    private Transform dummyTarget;
    private Vector3 m_Velocity = Vector3.zero;
    private ParticleSystem.MainModule ParticleSetting;

    [SerializeField] private ParticleSystem Particle;
    [SerializeField] private TrailRenderer Trail;
    [SerializeField] private Transform SlashPoint;
    [SerializeField] private GameObject Slash;
    [SerializeField] private GameObject Herb;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float hitRange;
    [SerializeField] private float health;
    [SerializeField] private float argoRange;

    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioSource StepSound;
    [SerializeField] private AudioClip SoundHurt;
    [SerializeField] private AudioClip SoundArgo;
    [SerializeField] private AudioClip SoundAttack;
    [SerializeField] private AudioClip SoundSpecial;
    [SerializeField] private AudioClip SoundDodge;
    [SerializeField] private AudioClip SoundDie;

    private float SmoothMovement = 0.05f;
    private bool hadap_kanan = true;
    private bool onGround = true;
    private float direction = 1f;
    private bool AttackCheck = false;
    private bool StunCheck = false;
    private bool counterCheck = false;
    private bool Invulnerable = false;
    private bool IsMoving = false;

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
        StartCoroutine(Step());
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
            targetDistance = Vector2.Distance(selfTransform.position, target.position);
            if (targetDistance <= hitRange && !StunCheck && !AttackCheck && !counterCheck && animator.GetFloat("speed") == 0f)
            {
                if (Random.Range(0, 100) < 10)
                {
                    animator.SetBool("onGuard", true);
                }
                else
                {
                    animator.SetBool("attack", true);
                    StartCoroutine("Attacking");
                }
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
        if (rb2d != null && !AttackCheck && !StunCheck && !counterCheck && target != null)
        {
            CloseDistance();
            if (target.position.x > selfTransform.position.x && !hadap_kanan) Flip();
            else if (target.position.x < selfTransform.position.x && hadap_kanan) Flip();
        }
        else if(rb2d != null && target != null && (AttackCheck || StunCheck || counterCheck))
        {
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
        Invulnerable = true;
        AttackCheck = true;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Knight_Dodge"))
        {
            if (StunCheck)
            {
                animator.SetBool("onGuard", false);
                counterCheck = false;
                yield break;
            }
            yield return null;
        }
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Knight_Dodge") && !animator.GetCurrentAnimatorStateInfo(0).IsName("Knight_Guard"))
        {
            animator.SetBool("onGuard", false);
            counterCheck = false;
            yield break;
        }
        Audio.PlayOneShot(SoundDodge);
        rb2d.mass = 0.2f;
        rb2d.gravityScale = 4;

        Trail.emitting = true;
        float direction = Mathf.Abs(transform.localScale.x) / transform.localScale.x;
        rb2d.AddForce(new Vector2(direction * -70, -5f));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Knight_Dodge"))
        {
            yield return null;
        }
        Audio.PlayOneShot(SoundSpecial);
        transform.Find("Special").gameObject.SetActive(true);
        Trail.emitting = false;
        Particle.Play();

        Invulnerable = false;
        float counter = 0f;
        while (counter<1f)
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
            counter+= Time.deltaTime;
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
        Audio.PlayOneShot(SoundAttack);
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
        clone.GetComponent<SlashScript>().SetDamage(0.25f);
        clone.transform.localScale *= SlashDirection.x;
        clone.GetComponent<Rigidbody2D>().velocity = SlashDirection * 1f;
        
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
        Audio.PlayOneShot(SoundHurt);
        if (health <= 0)
        {
            Audio.PlayOneShot(SoundDie);
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
            
            yield return new WaitForSeconds(0.25f);
            StunCheck = false;
            animator.SetBool("stunned", false);
        }
    }

    IEnumerator Jump()
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
