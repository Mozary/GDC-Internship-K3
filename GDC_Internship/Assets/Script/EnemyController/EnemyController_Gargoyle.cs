using System.Collections;
using UnityEngine;

public class EnemyController_Gargoyle : MonoBehaviour
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
    private float Lifetime = 0f;

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

        
        if (!AttackCheck && !StunCheck && Lifetime > 10f)
        {
            StunCheck = true;
            health = 0;
            StartCoroutine(Stunned());
        }
        else if (target == null && dummyTarget)
        {
            float dummyDistance = Vector2.Distance(transform.position, dummyTarget.position);
            if (dummyDistance <= argoRange)
            {
                target = dummyTarget;
                dummyTarget = null;
            }
        }
        else if (target != null)
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
            if (target.position.x > transform.position.x && !hadap_kanan) Flip();
            else if (target.position.x < transform.position.x && hadap_kanan) Flip();
        }
        Lifetime += Time.deltaTime;
        
    }
    private void Flip()
    {
        hadap_kanan = !hadap_kanan;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        direction *= -1;
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
        if (direction < 0)
        {
            Quaternion rotation = clone.transform.rotation;
            rotation.z -= 180;
            clone.transform.rotation = rotation;
        }
        //clone.transform.localScale *= SlashDirection.x;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        
        Invulnerable = false;
        AttackCheck = false;
    }
    IEnumerator Stunned()
    {
        StunCheck = true;
        if (health <= 0)
        {
            DropHerb();
            rb2d.isKinematic = true;
            this.GetComponent<Collider2D>().enabled = false;

            GameObject Explosion = Instantiate(Deathplode, transform.position, transform.rotation);
            yield return null;
            while (Explosion)
            {
                if ( transform.localScale.x  > 0)
                {
                    transform.Rotate(Vector3.forward*30);
                    transform.localScale -= new Vector3(0.1f, 0.15f, 0.1f);
                    if (transform.localScale.x < 0) { transform.localScale = Vector3.zero;}
                }
                else if (transform.localScale.x < 0)
                {
                    transform.Rotate(Vector3.back * 30);
                    transform.localScale += new Vector3(0.1f, 0.1f, 0.1f);
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
