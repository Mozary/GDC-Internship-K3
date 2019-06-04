using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{

    public Text nameUI;
    public Text dialogueUI;

    private Queue<string> ListDialogues;
    // Start is called before the first frame update
    void Start()
    {
        ListDialogues = new Queue<string>();

    }

    public void InteractOnButton(Dialogue dialogue)
    {
        ListDialogues.Clear();
        nameUI.text = dialogue.name;

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

    void EndDialogue()
    {
        Debug.Log("testii selesai");
    }
}

