using UnityEngine;

public class Interactable_Dialogue : MonoBehaviour
{
    public DialogueManager dialogueManager;
    private bool dialogueEnded;

    public GameObject dialogueBox;

    [Space]
    public Dialogue dialogue;
    private void Update()
    {
        dialogueEnded = dialogueManager.dialogueEnded;
    }

    public void InteractableDialogue()
    {
        dialogueBox.SetActive(true);
        dialogueManager.DisplayAfterInteract(dialogue);
    }
    public void OneTimeDialogue()
    {
        if (dialogueEnded)
        {
            Destroy(GetComponent<Interactable_Dialogue>());
        }   
    }
}
