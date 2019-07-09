using UnityEngine;

public class Interactable_Dialogue : MonoBehaviour
{
    public Dialogue dialogue;
    DialogueManager dialogueManager;
    private bool dialogueEnded;

    private GameObject parentCanvas;
    private GameObject dialogueBox;

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();

        parentCanvas = GameObject.FindGameObjectWithTag("Canvas");
        dialogueBox = parentCanvas.transform.GetChild(0).gameObject;
    }
    private void Update()
    {
        dialogueEnded = dialogueManager.dialogueEnded;
    }

    public void InteractableDialogue()
    {
        FindObjectOfType<DialogueManager>().DisplayAfterInteract(dialogue);
        dialogueBox.SetActive(true);
    }
    public void OneTimeDialogue()
    {
        if (dialogueEnded)
            Destroy(GetComponent<Interactable_Dialogue>());
    }
}
