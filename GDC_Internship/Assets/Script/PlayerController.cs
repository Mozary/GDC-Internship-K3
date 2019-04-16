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
    [SerializeField] private GameObject Arrow;

    bool jump = false;
    bool onGround = true;
    public float runSpeed = 20f;
    float movement = 0f;
    private bool AttackCheck = false;
    private bool RangedMode = false;
    private bool Immovable = false;
    // Start is called before the first frame update
    void Start()
    {
    }
    // Update is called once per frame
    void Update()
    {
        movement = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(movement));
        if (Input.GetButtonDown("Jump"))
        {
            if (!AttackCheck)
            {
                jump = true;
                animator.SetTrigger("jumping");
                animator.SetBool("isJumping", true);
            }
        }
        if (Input.GetButtonDown ("Attack")) {
            attack();
        }
        if (Input.GetButtonDown("Switch") && !checkPendingAttack())
        {
            RangedMode = !RangedMode;
            animator.SetBool("RangedMode", RangedMode);
        }
        if (animator.GetBool("attack1") || animator.GetBool("attack2") || animator.GetBool("attack3") ||animator.GetBool("Firing"))
        {
            if (!AttackCheck)
            {
                StartCoroutine("OnCompleteAttackAnimation");
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
    public void onLand() //Bug Fix buat CharacterController
    {
        if (!onGround)
        {
            animator.SetBool("isJumping", false);
            onGround = true;
        }
    }
    public void attack()
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
    public void rangedAttack()
    {
        if (movement == 0 && !animator.GetBool("isJumping") && RangedMode)
        {
            animator.SetBool("Firing", true);
        }

    }
    public bool checkPendingAttack()
    {
        if (animator.GetBool("attack1") || animator.GetBool("attack2") || animator.GetBool("attack3") || animator.GetBool("Firing"))
        {
            return true;
        }
        else return false;
    }
    IEnumerator OnCompleteAttackAnimation()
    {
        AttackCheck = true;
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            movement = 0f;
            yield return null;
        }
        AttackCheck = false;
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
}
