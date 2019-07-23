using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public Rigidbody2D rb2d;
    public Collider2D col2d;
    public Animator animator;
    public CharacterController2D controller;
    [SerializeField] private Transform FirePoint;
    [SerializeField] private Transform SlashPoint;
    [SerializeField] private GameObject Arrow;
    [SerializeField] private GameObject Slash;
    [SerializeField] private float runSpeed;
    [SerializeField] private float health;
    [SerializeField] private float dodgeCooldown;

    float movement = 0f;
    bool jump = false;
    bool onGround = true;
    bool Immovable = false;
    bool RangedMode = false;
    bool canDodge = true;
    Coroutine ActiveCoroutine = null;
    // Start is called before the first frame update
    void Start()
    {
        rb2d = this.GetComponent<Rigidbody2D>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!Immovable)
        {
            movement = Input.GetAxisRaw("Horizontal") * runSpeed;
        }
        else
        {
            movement = 0;
        }
        animator.SetFloat("Speed", Mathf.Abs(movement));
        if (Input.GetButtonDown("Jump"))
        {
            if (!Immovable)
            {
                jump = true;
                animator.SetTrigger("jumping");
                animator.SetBool("isJumping", true);
            }
        }
        if (Input.GetButtonDown ("Attack")) {
            Attack();
        }
        if (Input.GetButtonDown("Switch") && !CheckPendingAttack())
        {
            RangedMode = !RangedMode;
            animator.SetBool("RangedMode", RangedMode);
        }
        if (animator.GetBool("attack1") || animator.GetBool("attack2") || animator.GetBool("attack3") ||animator.GetBool("Firing"))
        {
            if (!Immovable)
            {
                StartCoroutine("OnCompleteAttackAnimation");
            }
        }
        if (Input.GetButtonDown("Dodge"))
        {
            if(!Immovable && canDodge && !jump)
            {
                animator.SetTrigger("isDodging");
                StartCoroutine("Dodge");
            }
        }
    }
    void FixedUpdate()
    {
        if (!col2d.IsTouchingLayers())
        {
            onGround = false;
        }
        controller.Move(movement * Time.fixedDeltaTime, false, jump);
        jump = false;
    }
    public void OnLand()
    {
        if (!onGround && animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerJump"))
        {
            animator.SetBool("isJumping", false);
            onGround = true;
        }
    }
    public void Attack()
    {
        if (movement == 0 && !animator.GetBool("isJumping") && !RangedMode)
        {
            
            if (animator.GetBool("attack2") == true)
            {
                animator.SetBool("attack3", true);
            }
            else if (animator.GetBool("attack1") == true)
            {
                animator.SetBool("attack2", true);
            }
            else
            {
                animator.SetBool("attack1", true);
            }
        }
        else if (movement == 0 && !animator.GetBool("isJumping") && RangedMode)
        {
            animator.SetBool("Firing", true);
        }
    }
    public void RangedAttack()
    {
        if (movement == 0 && !animator.GetBool("isJumping") && RangedMode)
        {
            animator.SetBool("Firing", true);
        }

    }
    public bool CheckPendingAttack()
    {
        if (animator.GetBool("attack1") || animator.GetBool("attack2") || animator.GetBool("attack3") || animator.GetBool("Firing"))
        {
            return true;
        }
        else return false;
    }
    IEnumerator Dodge()
    {
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDodge"))
        {
            yield return null;
        }
        Immovable = true;
        canDodge = false;
        float direction =Mathf.Abs(transform.localScale.x)/transform.localScale.x;
        rb2d.AddForce(new Vector2(direction*-125, -5f));
        while (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDodge"))
        {
            yield return null;
        }
        Immovable = false;
        yield return new WaitForSeconds(dodgeCooldown);
        canDodge = true;
    }
    IEnumerator OnCompleteAttackAnimation()
    {
        Immovable = true;
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerFireBow"))
        {
            Vector3 SlashDirection = new Vector3(transform.localScale.x, 0, 0).normalized;
            GameObject clone = Instantiate(Slash, SlashPoint.position, Slash.transform.rotation);
            clone.transform.localScale *= SlashDirection.x;
            clone.GetComponent<Rigidbody2D>().velocity = SlashDirection * 5f;
        }
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            movement = 0f;
            yield return null;
        }
        Immovable = false;
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerFireBow"))
        {
            Vector3 FireDirection = new Vector3(transform.localScale.x, 0,0).normalized;
            GameObject clone =  Instantiate(Arrow, FirePoint.position, Arrow.transform.rotation);
            clone.transform.localScale *= FireDirection.x;
            clone.GetComponent<Rigidbody2D>().velocity= FireDirection*10f;
            animator.SetBool("Firing", false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack1"))
        {
            animator.SetBool("attack1", false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack2"))
        {
            animator.SetBool("attack2", false);
            animator.SetBool("attack1", false);
        }
        else if (animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerAttack3"))
        {
            animator.SetBool("attack3", false);
            animator.SetBool("attack2", false);
            animator.SetBool("attack1", false);
        }
    }
    IEnumerator Stunned()
    {
        animator.SetTrigger("isHurt");
        animator.SetBool("stunned",true);
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerHurt"))
        {
            yield return null;
        }
        Immovable = true;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        if (health <= 0)
        {
            animator.SetTrigger("isDying");
            rb2d.isKinematic = true;
            Collider2D[]cols = this.GetComponents<Collider2D>();
            foreach (Collider2D element in cols)
            {
                element.enabled = false;
            }
            while (!animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDie"))
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
            Debug.Log("GAME OVER");
        }
        else
        {
            animator.SetBool("stunned", false);
            Immovable = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "EnemyAttack" && !animator.GetCurrentAnimatorStateInfo(0).IsName("PlayerDodge"))
        {

            if (ActiveCoroutine != null)
            {
                StopCoroutine(ActiveCoroutine);
            }
            animator.SetBool("attack3", false);
            animator.SetBool("attack2", false);
            animator.SetBool("attack1", false);
            animator.SetBool("Firing", false);
            health = health - 1;
            ActiveCoroutine = StartCoroutine("Stunned");
        }
    }
}
