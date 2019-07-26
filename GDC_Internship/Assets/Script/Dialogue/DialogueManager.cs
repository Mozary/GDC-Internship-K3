using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameUI;
    public TextMeshProUGUI dialogueUI;

    private Queue<string> ListDialogues;

    private GameObject parentCanvas;
    private GameObject dialogueBox;

    [HideInInspector]
    public bool dialogueEnded = false;
    // Start is called before the first frame update
    private void Start()
    {
        ListDialogues = new Queue<string>();

        parentCanvas = GameObject.FindGameObjectWithTag("Canvas");
        dialogueBox = parentCanvas.transform.GetChild(0).gameObject;
    }

    public void DisplayAfterInteract(Dialogue dialogue)
    {
        ListDialogues.Clear();
        nameUI.text = dialogue.name;
        //bool for SceneLoader
        dialogueEnded = false;
        Debug.Log("dialogueEnded dari displayafteronteract ___" + dialogueEnded);

        foreach (string sentence in dialogue.ListSentence)
        {
            ListDialogues.Enqueue(sentence);
            Debug.Log(sentence);
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
        Debug.Log(sentence + "dari nextsentence");
    }
    public void EndDialogue()
    {
        ListDialogues.Clear();
        dialogueBox.SetActive(false);

        //bool for Interactable_Dialogue and SceneLoader
        dialogueEnded = true;
        Debug.Log("dialogueEnded dari end dialogue ___" + dialogueEnded);
    }
}

