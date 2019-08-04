using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpellScript : MonoBehaviour
{
    enum Spell
    {
        Fireball,
        Iceshard,
        Blind
    }

    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem Emitor;
    [SerializeField] private float Damage = 2;
    [SerializeField] private float Knockback = 40;
    [SerializeField] private float Lifetime = 5f;
    [SerializeField] private float MaxSpeed = 200;
    [SerializeField] private Spell SpellType = Spell.Fireball;

    private Transform Target;
    private Rigidbody2D rb2d;
    private Collider2D collitor;
    private Vector3 m_Velocity = Vector3.zero;
    private float rotateSpeed = 4500f;
    private float countdown = 0f;
    private float Speed = 0;
    private Vector3 OriginScale;
    private Coroutine Explosion = null;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collitor = GetComponent<Collider2D>();
        OriginScale = transform.localScale;
        Target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    void Awake()
    {
        if(SpellType == Spell.Blind)
        {
            //transform.localScale = Vector3.zero;
        }
        StartCoroutine(Charging());
    }
    private void Update()
    {
        countdown += Time.deltaTime;
        if (Target && countdown < Lifetime)
        {
            rb2d.velocity = transform.right * Speed * Time.deltaTime;
            Vector3 targetVector = Target.position - transform.position;
            float rotatingIndex = Vector3.Cross(targetVector, transform.right).z;
            rb2d.angularVelocity = -1 * rotatingIndex * rotateSpeed * Time.deltaTime;
        }
        else
        {
            if(Explosion == null)
            {
                Explosion = StartCoroutine(Explode());
            }
        }
    }
    public void SetDamage(float NewDamage)
    {
        Damage = NewDamage;
    }
    IEnumerator Charging()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Homing"))
        {
            if (SpellType == Spell.Blind && transform.localScale.x < OriginScale.x)
            {
                float AnimTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                transform.localScale = new Vector3(AnimTime * OriginScale.x, AnimTime * OriginScale.x, 0);
                //transform.localScale += new Vector3(0.025f, 0.025f, 0.025f);
            }
            Speed = 0;
            yield return null;
        }
        Speed = MaxSpeed;
    }
    IEnumerator Explode()
    {
        Speed = 0;
        animator.SetTrigger("contact");
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Explode"))
        {
            yield return null;
        }
        collitor.enabled = false;
        rb2d.isKinematic = true;
        Emitor.Play();

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Explode"))
        {
            yield return null;
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Ground")
        {
            collitor.enabled = false;
            rb2d.isKinematic = true;
            Emitor.Play();

            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(Damage);
                if (SpellType == Spell.Fireball)
                {
                    collision.gameObject.GetComponent<PlayerController>().TakeFireDamage();
                }
                else if (SpellType == Spell.Iceshard)
                {
                    collision.gameObject.GetComponent<PlayerController>().TakeIceDamage();
                }
                else if (SpellType == Spell.Blind)
                {
                    collision.gameObject.GetComponent<PlayerController>().TakeBlindDamage();
                }

                if (collision.gameObject.transform.position.x < transform.position.x)
                {
                    Knockback = -1 * Knockback;
                }
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(Knockback, -5));
            }
            if(Explosion == null)
            {
                Explosion = StartCoroutine(Explode());
            }
        }
    }
}
