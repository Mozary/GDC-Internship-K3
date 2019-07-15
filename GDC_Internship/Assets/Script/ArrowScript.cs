using UnityEngine;

public class ArrowScript : MonoBehaviour
{
    private new Collider2D collider;

    void Awake()
    {
        collider = GetComponent<Collider2D>();
        Destroy(this.gameObject, 10f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "Ground")
        {
            Destroy(this.gameObject);
        }
    }
}
