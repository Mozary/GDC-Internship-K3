using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSpellScript : MonoBehaviour
{
    enum Spell
    {
        Fireball,
        Iceshard
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
    private Coroutine Explosion = null;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collitor = GetComponent<Collider2D>();
        Target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }
    void Awake()
    {
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
            Debug.Log(rotatingIndex);
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
    IEnumerator Charging()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Homing"))
        {
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
                if (SpellType == Spell.Fireball)
                {
                    collision.gameObject.GetComponent<PlayerController>().TakeFireDamage();
                }
                else if (SpellType == Spell.Iceshard)
                {
                    collision.gameObject.GetComponent<PlayerController>().TakeIceDamage();
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
