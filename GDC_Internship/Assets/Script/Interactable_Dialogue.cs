using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Dialogue : MonoBehaviour
{
    public Dialogue dialogue;

    public void InteractableDialogue()
    {
        FindObjectOfType<DialogueManager>().DisplayAfterInteract(dialogue);

    }  
}
