﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerFSM : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private Transform target;
    private Transform selfTransform;
    private Vector3 m_Velocity = Vector3.zero;
    [SerializeField] private Animator animator;
    [SerializeField] private float SmoothMovement;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float hitRange;
    [SerializeField] private float health;
    private bool hadap_kanan = true;
    private bool onGround = true;
    private float direction = 1f;
    private bool AttackCheck = false;
    private float targetDistance = 0f;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        rb2d = this.GetComponent<Rigidbody2D>();
        selfTransform = this.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        targetDistance = Vector2.Distance(selfTransform.position, target.position);
        if (targetDistance <= hitRange && !AttackCheck && animator.GetFloat("speed") == 0f)
        {
            animator.SetBool("attack", true);
            StartCoroutine("Attacking");
        }
    }
    void FixedUpdate()
    {  
        if (rb2d != null && !AttackCheck)
        {
            closeDistance();
            if (target.position.x > selfTransform.position.x && !hadap_kanan) Flip();
            else if (target.position.x < selfTransform.position.x && hadap_kanan) Flip();
        }
    }
    void closeDistance()
    {
        if (onGround && (targetDistance > hitRange))
        {
            Vector3 moveVelocity = new Vector2(direction * maxSpeed * Time.deltaTime, rb2d.velocity.y);
            animator.SetFloat("speed", Mathf.Abs(moveVelocity.x));
            rb2d.velocity = Vector3.SmoothDamp(rb2d.velocity, moveVelocity, ref m_Velocity, SmoothMovement);
        } else { animator.SetFloat("speed", 0f); }
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
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
        {
            yield return null;
        }
        AttackCheck = false;
        animator.SetBool("attack", false);
    }
}
