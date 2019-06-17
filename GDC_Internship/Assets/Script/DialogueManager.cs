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
    // Start is called before the first frame update
    void Start()
    {
        ListDialogues = new Queue<string>();

    }

    public void DisplayAfterInteract(Dialogue dialogue)
    {
        ListDialogues.Clear();
        nameUI.text = dialogue.name;
        Debug.Log("display after interact terpanggil");

        foreach (string sentence in dialogue.ListSentence)
        {
            ListDialogues.Enqueue(sentence);
            Debug.Log(sentence + " ____disiapkan");

        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        Debug.Log("display next sentence terpanggil");

        if (ListDialogues.Count == 0)
        {
            Debug.Log("list dialogue habis");
            EndDialogue();
            return;
        }

        //foreach (string sentence in ListDialogues)
        //if(Input.GetKeyDown("e") &&  )
        {
            string sentence = ListDialogues.Dequeue();
            Debug.Log(sentence + " _____terpanggil");
            dialogueUI.text = sentence;
            
            //if(sentence == ListDialogues)
            {

            }
        }
        
    }

    public void EndDialogue()
    {
        GameObject checkCanvas = GameObject.FindGameObjectWithTag("Canvas");
        Debug.Log(checkCanvas.activeSelf);

        Debug.Log("testii dihapus harusnya ulang semua");
        ListDialogues.Clear();
        GameObject.FindGameObjectWithTag("Canvas").SetActive(false);

        
        Debug.Log(checkCanvas.activeSelf);
    }
}

