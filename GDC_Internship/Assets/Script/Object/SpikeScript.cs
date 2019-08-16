using UnityEngine;

public class SpikeScript : MonoBehaviour
{
    //Invisible short-ranged hitbox (to be used like arrow)
    [SerializeField] private float Knockback = 35f;
    [SerializeField] private float Damage = 0.25f;
    private new Collider2D collider;
    
    void Awake()
    {
        collider = GetComponent<Collider2D>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        float knockforceX = Knockback; //Knockback force
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(Damage);
            if (collision.gameObject.transform.position.x < transform.position.x)
            {
                knockforceX = -1 * knockforceX;
            }
            collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockforceX, -5));
        }

    }
}
