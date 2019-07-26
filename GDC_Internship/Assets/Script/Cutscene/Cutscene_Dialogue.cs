using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Cutscene_Dialogue : MonoBehaviour
{
    private Interactable_Dialogue interactDialogue;
    private DialogueManager dialogueManager;
    private bool dialogueEnded = false;

    private bool startDialogue = false;
    private bool dialogueMagic = false;

    private void Awake()
    {
        interactDialogue = gameObject.GetComponent<Interactable_Dialogue>();
        dialogueManager = FindObjectOfType<DialogueManager>();
    }
    private void OnEnable()
    {
        startDialogue = true;
        dialogueMagic = true;
    }
    private void Update()
    {
        if (startDialogue)
        {
            if (!dialogueEnded)
            {
                startDialogue = false;
                interactDialogue.InteractableDialogue();
            }
        }

        if (dialogueMagic)
        {
            if (!startDialogue)
            {
                dialogueEnded = dialogueManager.dialogueEnded;
                Debug.Log("dialogueEnded di Cutscene_Dialogue _____" + dialogueEnded);
                if (Input.GetKeyDown("e"))
                {
                    dialogueManager.DisplayNextSentence();
                }
                if (dialogueEnded == true)
                {
                    dialogueMagic = false;
                }
            }
        }
    }
}
