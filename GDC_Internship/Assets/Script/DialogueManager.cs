using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameUI;
    public TextMeshProUGUI dialogueUI;

    private Queue<string> ListDialogues;

    GameObject parentCanvas;
    GameObject dialogueBox;

    [HideInInspector]
    public bool moveScene = false;
    // Start is called before the first frame update
    void Start()
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
        moveScene = false;
        Debug.Log("dari displayafteronteract ___" + moveScene);

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

        //bool for SceneLoader
        moveScene = true;
        Debug.Log("dari end dialogue ___" + moveScene);
    }
}

