using UnityEngine;

public class HerbScript : MonoBehaviour
{
    private new Collider2D collider;
    private InteractToCollect itC;
    void Awake()
    {
        itC = FindObjectOfType<InteractToCollect>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Canvas")
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && gameObject.layer != LayerMask.NameToLayer("UI"))
        {
            gameObject.layer = LayerMask.NameToLayer("UI");
            itC.DroppedHerb();
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }
}
