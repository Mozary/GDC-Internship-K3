using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Scorpion : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb2d;
    private Vector3 target = Vector3.zero;
    private CapsuleCollider2D bodyCollider;
    private Bounds patrolBounds;
    private Vector3 ref_velocity = Vector3.zero;
    private Transform targetPlayer;
    public BoxCollider2D patrolArea;

    public float maxSpeed = 30f;
    public float health = 2f;
    public float hitRange = 0.4f;
    public float waitTime = 0f;

    private float SmoothMovement = 0.05f;
    private string state = "idle";
    private float faceDirection = 1f;

    private bool faceRight = true;
    // Start is called before the first frame update
    void Start()
    {
        bodyCollider = GetComponent<CapsuleCollider2D>();
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
            if (Vector2.Distance(transform.localPosition, target) <= 0.4f)
            {
                //Telling the object to wait
                if (waitTime <= 0)
                {
                    Patrol();
                    waitTime = 3f;
                    maxSpeed = 30f;
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
        if(state == "patrol")
        {
            if (rb2d != null && target != null)
            {
                Move();
                if (target.x > transform.localPosition.x && !faceRight) Flip();
                else if (target.x < transform.localPosition.x && faceRight) Flip();
            }
        }
        else if (state == "follow")
        {
            if (rb2d != null && targetPlayer != null)
            {
                Move();
                if (targetPlayer.position.x > transform.localPosition.x && !faceRight) Flip();
                else if (targetPlayer.position.x < transform.localPosition.x && faceRight) Flip();
            }
        }
        
    }
    void Patrol()
    {
        float new_patrolPoint = Random.Range(-patrolBounds.extents.x, patrolBounds.extents.x);
        new_patrolPoint = new_patrolPoint + patrolBounds.center.x;
        target =  new Vector2(new_patrolPoint, transform.localPosition.y);
        Debug.Log(target);
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
}