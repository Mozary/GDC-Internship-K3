using UnityEngine;

public class HarmTouchScript : MonoBehaviour
{
    //Invisible short-ranged hitbox (to be used like arrow)
    [SerializeField] private float Knockback = 35f;
    private new Collider2D collider;
    
    void Awake()
    {
        collider = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(transform.tag == "Attack")
        {
            float knockforceX = Knockback; //Knockback force
            if (collision.gameObject.tag == "Enemy")
            {
                if (collision.gameObject.transform.position.x < transform.position.x)
                {
                    knockforceX = -1 * knockforceX;
                }
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockforceX, -5));
            }
                
        }
        else if (transform.tag == "EnemyAttack")
        {
            float knockforceX = Knockback; //Knockback force
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(0.5f);
                if (collision.gameObject.transform.position.x < transform.position.x)
                {
                    knockforceX = -1 * knockforceX;
                }
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockforceX, -5));
            }
        }

    }
}
