using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialSlashScript : MonoBehaviour
{

    private Animator animator;
    private Coroutine effect = null;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
    }

    IEnumerator Discharge()
    {
        animator.SetTrigger("Discharge");
        GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        while (!animator.GetCurrentAnimatorStateInfo(0).IsName("Slash") || animator.GetCurrentAnimatorStateInfo(0).normalizedTime<1)
        {
            yield return null;
        }
        Destroy(gameObject);
    }
    IEnumerator Lifetime()
    {
        yield return new WaitForSeconds(0.5f);
        if (effect == null)
        {
            effect = StartCoroutine(Discharge());
        }
    }
    private void Flip()
    {
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }
    public void Unleash(Vector3 direction)
    {

        if (direction.x <0)
        {
            Flip();
        }
        GetComponent<Rigidbody2D>().velocity = direction * 7.5f;
        StartCoroutine(Lifetime());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        float knockforceX = 70; //Knockback force
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(1.5f);
            if (collision.gameObject.transform.position.x < transform.position.x)
            {
                knockforceX = -1 * knockforceX;
            }
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockforceX, -5));
            if (effect == null)
            {
                effect = StartCoroutine(Discharge());
            }
        }
        else if(collision.gameObject.tag == "Ground")
        {
            if(effect == null)
            {
                effect = StartCoroutine(Discharge());
            }
        }
    }
}
