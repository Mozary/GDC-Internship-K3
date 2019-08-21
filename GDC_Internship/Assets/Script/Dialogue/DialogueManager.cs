using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameUI;
    public TextMeshProUGUI dialogueUI;

    private Queue<string> ListDialogues;

    public GameObject dialogueBox;

    [HideInInspector]
    public bool dialogueEnded = false;
    // Start is called before the first frame update
    private void Start()
    {
        ListDialogues = new Queue<string>();
    }

    public void DisplayAfterInteract(Dialogue dialogue)
    {
        ListDialogues.Clear();
        nameUI.text = dialogue.name;
        //bool for SceneLoader
        dialogueEnded = false;

        foreach (string sentence in dialogue.ListSentence)
        {
            ListDialogues.Enqueue(sentence);
        }

        DisplayNextSentence();
    }
    public void DisplayNextSentence()
    {
        if (ListDialogues.Count == 0)
        {
            EndDialogue();
            return;
        }

        string sentence = ListDialogues.Dequeue();
        dialogueUI.text = sentence;
    }
    public void EndDialogue()
    {
        ListDialogues.Clear();
        dialogueBox.SetActive(false);

        //bool for Interactable_Dialogue and SceneLoader
        dialogueEnded = true;
    }
}

