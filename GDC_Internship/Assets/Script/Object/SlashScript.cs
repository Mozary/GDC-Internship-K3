using UnityEngine;

public class SlashScript : MonoBehaviour
{
    //Invisible short-ranged hitbox (to be used like arrow)
    [SerializeField] private float Knockback = 35f;
    [SerializeField] private float SlashRange = 0.05f;
    [SerializeField] private float SlashDamage = 1f;

    private new Collider2D collider;
    
    void Awake()
    {
        collider = GetComponent<Collider2D>();
        Destroy(this.gameObject, SlashRange);
    }
    public float GetDamage()
    {
        return SlashDamage;
    }
    public void SetDamage(float damage)
    {
        SlashDamage = damage;
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
                Destroy(this.gameObject);
            }
                
        }
        else if (transform.tag == "EnemyAttack")
        {
            float knockforceX = Knockback; //Knockback force
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<PlayerController>().TakeDamage(SlashDamage);

                if (collision.gameObject.transform.position.x < transform.position.x)
                {
                    knockforceX = -1 * knockforceX;
                }
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockforceX, -5));
                Destroy(this.gameObject);
            }
        }

    }
}
