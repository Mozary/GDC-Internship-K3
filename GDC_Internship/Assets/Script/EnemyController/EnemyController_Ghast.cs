using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Ghast : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem Emitor;
    [SerializeField] private float Damage = 2.5f;
    [SerializeField] private float Knockback = 70;
    [SerializeField] private float MaxSpeed = 200;
    [SerializeField] private float ArgoRange = 3f;
    [SerializeField] private GameObject Herb;

    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioSource StepSound;
    [SerializeField] private AudioClip SoundAppear;
    [SerializeField] private AudioClip SoundExplode;

    private Transform Target = null;
    private Transform Dummy;
    private Rigidbody2D rb2d;
    private Collider2D collitor;
    private Vector3 m_Velocity = Vector3.zero;
    private Coroutine Explosion = null;
    private bool hadap_kanan = true;
    private bool onGround = true;

    private float rotateSpeed = 4500f;
    private float Speed = 0;
    private float SmoothMovement = 0.05f;
    private float directionX = 1f;
    private float directionY = 1f;
    private float targetDistance;
   

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collitor = GetComponent<Collider2D>();
        Dummy = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        StartCoroutine(Charging());
    }
    private void Update()
    {
        if (Target == null && Dummy)
        {
            float dummyDistance = Vector2.Distance(transform.position, Dummy.position);
            if (dummyDistance <= ArgoRange)
            {
                Audio.PlayOneShot(SoundAppear);
                StepSound.PlayDelayed(0.5f);
                animator.SetTrigger("inRange");
                Target = Dummy;
                Dummy = null;
            }
        }
        else if(Target != null)
        {
            targetDistance = Vector2.Distance(transform.position, Target.position);
        }
    }

    private void FixedUpdate()
    {
        if (Target != null && targetDistance<= ArgoRange)
        {
            Vector3 moveVelocity = new Vector2(directionX * Speed * Time.deltaTime, directionY * Speed/2 * Time.deltaTime);
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, moveVelocity, ref m_Velocity, SmoothMovement);

            if (Target.position.x > transform.position.x && !hadap_kanan) Flip();
            else if (Target.position.x < transform.position.x && hadap_kanan) Flip();

            if (Target.position.y > transform.position.y)
            {
                directionY = 0.5f;
            }
            else if (Target.position.y < transform.position.y)
            {
                directionY = -0.5f;
            }
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
        directionX *= -1;
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
        StepSound.Stop();
        animator.SetTrigger("contact");
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Vanish"))
        {
            yield return null;
        }
        Audio.PlayOneShot(SoundExplode);
        collitor.enabled = false;
        rb2d.isKinematic = true;
        Emitor.Play();

        while (animator.GetCurrentAnimatorStateInfo(0).IsName("Vanish"))
        {
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Attack")
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerController>().TakeBlindDamage();
                collision.gameObject.GetComponent<PlayerController>().TakeIceDamage();
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(Damage);

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
