using UnityEngine;

public class SceneDoors : MonoBehaviour
{
    public DialogueManager dialogueManager;

    public PlayerController i_player;
    public GameObject targetPosition;

    private void Update()
    {
        if (dialogueManager.dialogueEnded)
        {
            i_player.transform.position = targetPosition.transform.position;
        }
    }
}
