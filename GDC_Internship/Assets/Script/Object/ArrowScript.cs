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
        float knockforceX = 75; //Knockback force
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Ground")
        {
            if (collision.gameObject.tag == "Enemy")
            {
                if (collision.gameObject.transform.position.x < transform.position.x)
                {
                    knockforceX = -1* knockforceX;
                }
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(knockforceX, 0));
            }
            Destroy(this.gameObject);
        }
    }
}
