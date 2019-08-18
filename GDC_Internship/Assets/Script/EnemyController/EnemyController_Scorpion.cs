using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Scorpion : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb2d;
    private Vector3 target = Vector3.zero;
    private Bounds patrolBounds;
    private Vector3 ref_velocity = Vector3.zero;
    private Transform targetPlayer;
    public BoxCollider2D patrolArea;

    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioClip SoundDeath;

    [SerializeField] private float maxSpeed;
    [SerializeField] private float health;
    [SerializeField] private float hitRange;
    [SerializeField] private float waitTime;
    [SerializeField] private GameObject Herb;

    private float constSpeed;
    private float constWait;

    private float SmoothMovement = 0.05f;
    private string state = "idle";
    private float faceDirection = 1f;

    private Coroutine ActiveCoroutine = null;
    private bool faceRight = true;
    private bool gotHit = false;
    // Start is called before the first frame update
    void Start()
    {
        constSpeed = maxSpeed;
        constWait = waitTime;
        
        targetPlayer = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        if (patrolArea)
        {
            state = "patrol";
            patrolBounds = patrolArea.GetComponent<BoxCollider2D>().bounds;
            Patrol();
        }
        else
        {
            Debug.Log("Patrol Area not Assigned, Following Player");
            state = "follow";
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //Check if patrol position is reached
        if (state == "patrol")
        {
            if (Mathf.Abs(transform.position.x - target.x) <= 0.4f)
            {
                //Telling the object to wait
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

    }
    void FixedUpdate()
    {
        if(state == "patrol" && rb2d.isKinematic == false)
        {
            if (rb2d != null && target != null)
            {
                Move();
                if (target.x > transform.position.x && !faceRight) Flip();
                else if (target.x < transform.position.x && faceRight) Flip();
            }
        }
        else if (state == "follow" && rb2d.isKinematic == false)
        {
            if (rb2d != null && targetPlayer != null)
            {
                Move();
                if (targetPlayer.position.x > transform.position.x && !faceRight) Flip();
                else if (targetPlayer.position.x < transform.position.x && faceRight) Flip();
            }
        }
        
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
    private void DropHerb()
    {
        Vector3 DropDirection = new Vector3(-transform.localScale.x, 1, 0).normalized;
        GameObject clone = Instantiate(Herb, transform.position, transform.rotation);
        clone.GetComponent<Rigidbody2D>().velocity = DropDirection * 1f;
    }
    IEnumerator Death()
    {
        
        maxSpeed = 0;
        constSpeed = 0;
        this.GetComponent<Collider2D>().enabled = false;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        rb2d.isKinematic = true;
        
        DropHerb();
        float transformChange = 0f;
        while (transformChange < 180)
        {
            transform.Rotate(Vector3.forward *10);
            
            transformChange += 10;
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
    IEnumerator Hurt()
    {
        Audio.PlayOneShot(SoundDeath);
        float flashTime = 0.05f;
        Color mycolour = GetComponent<SpriteRenderer>().color;
        mycolour.g = 0f;
        mycolour.b = 0f;
        GetComponent<SpriteRenderer>().color = mycolour;
        while (flashTime > 0)
        {
            yield return new WaitForSeconds(flashTime);
            flashTime = 0;
        }
        gotHit = false;
        GetComponent<SpriteRenderer>().color = Color.white;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack" && !gotHit)
        {
            gotHit = true;
            this.health = this.health - 1;
            if (health <= 0)
            {
                if (ActiveCoroutine != null)
                {
                    StopCoroutine(ActiveCoroutine);
                    GetComponent<SpriteRenderer>().color = Color.white;
                }
                ActiveCoroutine = StartCoroutine(Death());
            }
            else
            {
                if (ActiveCoroutine != null)
                {
                    StopCoroutine(ActiveCoroutine);
                    GetComponent<SpriteRenderer>().color = Color.white;
                }
                ActiveCoroutine = StartCoroutine(Hurt());
            }
        }
    }
}