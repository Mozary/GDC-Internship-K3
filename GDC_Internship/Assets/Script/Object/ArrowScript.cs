using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private new Collider2D collider;
    void Awake()
    {
        collider = GetComponent<Collider2D>();
        Destroy(this.gameObject, 1f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        float knockforceX = 35; //Knockback force
        if(gameObject.tag == "Attack")
        {
            if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Ground" || collision.gameObject.tag == "EnemyAttack")
            {
                collider.enabled = false;
                if (collision.gameObject.tag == "Enemy")
                {
                    if (collision.gameObject.transform.position.x < transform.position.x)
                    {
                        knockforceX = -1 * knockforceX;
                    }
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity =  Vector2.zero;
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockforceX*2, 0));
                }
                Destroy(this.gameObject);
            }
        }
        else if(gameObject.tag == "EnemyAttack")
        {
            if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Ground" || collision.gameObject.tag == "Attack")
            {
                collider.enabled = false;
                if (collision.gameObject.tag == "Player")
                {
                    Debug.Log("Player Got Hit");
                    collision.gameObject.GetComponent<PlayerController>().TakeDamage(0.5f);
                    if (collision.gameObject.transform.position.x < transform.position.x)
                    {
                        knockforceX = -1 * knockforceX;
                    }
                    collision.gameObject.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                    collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockforceX, 0));
                }
                Destroy(this.gameObject);
            }
        }
    }
}
