using UnityEngine;

public class SceneDoors_ch2 : MonoBehaviour
{
    public DialogueManager dialogueManager;

    public PlayerController i_player;
    public GameObject enemy;
    public GameObject targetPosition;

    private bool p_PosAbove = false;
    private bool e_PosAbove = true;
    private void Update()
    {
        if (dialogueManager.dialogueEnded)
        {
            i_player.transform.position = targetPosition.transform.position;
        }
    }

    private void OnTriggerEnter2D(PolygonCollider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            p_PosAbove = true;
        }
        if(collision.gameObject == enemy)
        {
            e_PosAbove = true;
        }
    }
    private void OnTriggerExit2D(PolygonCollider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            p_PosAbove = false;
        }
        if(collision.gameObject == enemy)
        {
            e_PosAbove = false;
        }
    }
    private void OnTriggerEnter2D(BoxCollider2D collision)
    {
        if (p_PosAbove && !e_PosAbove || !p_PosAbove && e_PosAbove)
        {
            if (collision.gameObject == enemy)
            {
                collision.transform.position = targetPosition.transform.position;
            }
        }
    }
}
