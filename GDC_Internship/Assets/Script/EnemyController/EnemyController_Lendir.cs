using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Lendir : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb2d;
    private Vector3 target = Vector3.zero;
    private CapsuleCollider2D bodyCollider;
    private Bounds patrolBounds;
    private Vector3 ref_velocity = Vector3.zero;
    private Transform targetPlayer;
    private Transform selfTransform;
    public BoxCollider2D patrolArea;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float health;
    [SerializeField] private float hitRange;
    [SerializeField] private float waitTime; //Idle setelah nyampe target
    [SerializeField] private float argoRange; //Argo Distance, bakal gerak kalo dalam range
    [SerializeField] private float giveupRange; //bakal nyerah kalo diluar range

    [SerializeField] private Transform SlashPoint;
    [SerializeField] private GameObject Slash;
    [SerializeField] private GameObject Herb;

    private float constSpeed;
    private float constWait;

    private float SmoothMovement = 0.05f;
    private string state = "idle";
    private float faceDirection = 1f;
    private bool AttackCheck = false;
    private float targetDistance;
    private Coroutine ActiveCoroutine = null;
    private bool faceRight = true;
    private bool StunCheck = false;
    // Start is called before the first frame update
    void Start()
    {
        constSpeed = maxSpeed;
        constWait = waitTime;

        bodyCollider = GetComponent<CapsuleCollider2D>();
        targetPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        selfTransform = this.GetComponent<Transform>();
        rb2d = GetComponent<Rigidbody2D>();
        if (patrolArea)
        {
            state = "patrol";
            patrolBounds = patrolArea.GetComponent<BoxCollider2D>().bounds;
            Patrol();
        }
        else
        {
            state = "follow";
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (targetPlayer)
        {
            targetDistance = Vector2.Distance(selfTransform.position, targetPlayer.position);
            if (targetDistance <= argoRange && state == "patrol")
            {
                waitTime = constWait;
                maxSpeed = constSpeed;
                state = "follow";
            }
            else if(targetDistance >= giveupRange && state == "follow")
            {
                state = "patrol";
            }
        }

        if (state == "patrol" && targetPlayer)
        {
            if (Mathf.Abs(transform.localPosition.x - target.x) <= 0.4f)
            {
                if (waitTime <= 0)
                {
                    Patrol();
                    waitTime = constWait;
                    maxSpeed = constSpeed;
                }
                else
                {
                    waitTime -= Time.deltaTime;
                    maxSpeed = 0f;
                }
            }
        }
        else if (state == "follow" && targetPlayer)
        {
            if (targetDistance <= hitRange && !AttackCheck &!StunCheck && animator.GetFloat("speed") == 0f)
            {
                animator.SetBool("attack", true);
                StartCoroutine("Attacking");
            }
        }
    }
    void FixedUpdate()
    {
        if (targetPlayer)
        {
            if (state == "patrol" && rb2d.isKinematic == false && !StunCheck)
            {
                if (rb2d != null && target != null)
                {
                    Move();
                    if (target.x > transform.localPosition.x && !faceRight) Flip();
                    else if (target.x < transform.localPosition.x && faceRight) Flip();
                }
            }
            else if (state == "follow" && rb2d.isKinematic == false && !StunCheck)
            {
                if (rb2d != null && targetPlayer != null)
                {
                    Follow();
                    if (targetPlayer.position.x > transform.localPosition.x && !faceRight) Flip();
                    else if (targetPlayer.position.x < transform.localPosition.x && faceRight) Flip();
                }
            }
        }

    }
    void Follow()
    {
        if (targetDistance > hitRange)
        {
            Vector3 moveVelocity = new Vector2(faceDirection * maxSpeed * Time.deltaTime, rb2d.velocity.y);
            animator.SetFloat("speed", Mathf.Abs(moveVelocity.x));
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, moveVelocity, ref ref_velocity, SmoothMovement);
        }
        else { animator.SetFloat("speed", 0f); }
    }
    void Patrol()
    {
        float new_patrolPoint = Random.Range(-patrolBounds.extents.x, patrolBounds.extents.x);
        new_patrolPoint = new_patrolPoint + patrolBounds.center.x;
        target =  new Vector2(new_patrolPoint, transform.localPosition.y);
    }
    void Move()
    {
        Vector3 moveVelocity = new Vector2(faceDirection * maxSpeed * Time.deltaTime, rb2d.velocity.y);
        animator.SetFloat("speed", Mathf.Abs(moveVelocity.x));
        rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, moveVelocity, ref ref_velocity, SmoothMovement);
    }
    private void Flip()
    {
        faceRight = !faceRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
        faceDirection *= -1;
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
            yield return null;
        }
        if (!StunCheck)
        {
            Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
            GameObject clone = Instantiate(Slash, SlashPoint.position, Slash.transform.rotation);
            clone.transform.localScale *= SlashDirection.x;
            clone.GetComponent<Rigidbody2D>().velocity = SlashDirection * 1f;
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
        AttackCheck = false;
        animator.SetBool("attack", false);
    }
    IEnumerator Hurt()
    {
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
    private void DropHerb()
    {
        Vector3 DropDirection = new Vector3(-transform.localScale.x, 1, 0).normalized;
        GameObject clone = Instantiate(Herb, transform.position, transform.rotation);
        clone.GetComponent<Rigidbody2D>().velocity = DropDirection * 1f;
    }
    IEnumerator Stunned()
    {
        animator.SetTrigger("isHurt");
        animator.SetBool("stunned", true);
        StunCheck = true;
        StartCoroutine(Hurt());
        float counter = 0.5f;
        while (counter > 0)
        {
            yield return new WaitForSeconds(0.5f);
            counter = counter - 0.5f;
        }
        if (health <= 0)
        {
            DropHerb();
            animator.SetTrigger("isDying");
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack")
        {
            if (ActiveCoroutine != null)
            {
                StopCoroutine(ActiveCoroutine);
            }
            this.health = this.health - 1;
            ActiveCoroutine = StartCoroutine(Stunned());
        }
    }
}