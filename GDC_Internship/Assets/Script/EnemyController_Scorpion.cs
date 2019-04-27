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
    public BoxCollider2D patrolArea;

    public float maxSpeed = 30f;
    public float health = 2f;
    public float hitRange = 0.4f;
    public float waitTime = 0f;

    private float SmoothMovement = 0.05f;
    private string state = "patroling";
    private float faceDirection = 1f;

    private bool faceRight = true;
    // Start is called before the first frame update
    void Start()
    {
        bodyCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        patrolBounds = patrolArea.GetComponent<BoxCollider2D>().bounds;
        Patrol();
    }

    // Update is called once per frame
    void Update()
    {
        //Check if patrol position is reached
        if (Vector2.Distance(transform.localPosition, target) <= 0.4f)
        {
            //Telling the object to wait
            if(waitTime <= 0)
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
    void FixedUpdate()
    {
        if (rb2d != null && target != null)
        {
            Move();
            if (target.x > transform.localPosition.x && !faceRight) Flip();
            else if (target.x < transform.localPosition.x && faceRight) Flip();
        }
    }
    void Patrol()
    {
        float new_patrolPoint = Random.Range(-patrolBounds.extents.x, patrolBounds.extents.x);
        target = new Vector2(new_patrolPoint, transform.localPosition.y);
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