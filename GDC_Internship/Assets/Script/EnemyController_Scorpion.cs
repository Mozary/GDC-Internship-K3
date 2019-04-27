using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Scorpion : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb2d;
    private Transform target;
    private CapsuleCollider2D bodyCollider;
    private Bounds patrolBounds;
    private Vector3 ref_velocity = Vector3.zero;
    public BoxCollider2D patrolArea;

    public float maxSpeed = 30f;
    public float health = 2f;
    public float hitRange = 0.4f;
    public float waitTime = 2f;

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
        target = transform;
        Debug.Log(transform.position);
        Debug.Log(target.position);
        Patrol();
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(transform.position, target.position) <= 0.1f)
        {
            if(waitTime <= 0)
            {
                Patrol();
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }
    void FixedUpdate()
    {
        if (rb2d != null && target != null)
        {
            Move();
            if (target.position.x > transform.position.x && !faceRight) Flip();
            else if (target.position.x < transform.position.x && faceRight) Flip();
        }
    }
    void Patrol()
    {
        float new_patrolPoint = Random.Range(-patrolBounds.extents.x, patrolBounds.extents.x);
        target.position = new Vector3(new_patrolPoint, transform.position.y, transform.position.z);
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