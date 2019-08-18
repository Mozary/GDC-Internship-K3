using UnityEngine;

public class SceneDoors : MonoBehaviour
{
    public DialogueManager dialogueManager;

    public PlayerController i_player;
    protected GameObject enemy;
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
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject != collision.gameObject.CompareTag("Player"))
        {
            enemy = collision.gameObject;
        }

        if (collision.gameObject == i_player)
        {
            p_PosAbove = true;
        }
        if (collision.gameObject == enemy)
        {
            e_PosAbove = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject != collision.gameObject.CompareTag("Player"))
        {
            enemy = collision.gameObject;
        }

        if (collision.gameObject == i_player)
        {
            p_PosAbove = false;
        }
        if (collision.gameObject == enemy)
        {
            e_PosAbove = false;
        }
    }
}
