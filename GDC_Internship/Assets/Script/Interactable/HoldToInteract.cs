using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HoldToInteract : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public GameObject hold_button;
    public Animator aniHold;

    private bool readyActive = false;
    private bool holdActivate = false;
    float holdStart = 0.0f;
    private void OnTriggerStay2D(Collider2D other)
    {
        if(dialogueManager.dialogueEnded)
        {
            Debug.Log("masuk?  ___dialogue ended terupdate");
            if(readyActive)
            {
                if(Input.GetKey("e"))
                {
                    holdStart += Time.deltaTime;
                    hold_button.SetActive(true);
                    aniHold.SetTrigger("holdButton");
                    if (holdStart > 3.0f)
                    {
                        holdActivate = true;
                        holdStart = 0.0f;
                    }
                }
                else
                {
                    hold_button.SetActive(false);
                    holdStart = 0.0f;
                }
            }
        }

        if(GetComponent<HintOfInstruction>().enabled)
        {
            readyActive = true;
            Debug.Log("masuk?  ___ready active dari HTI");
        }

        if(holdActivate)
        {
            //// activate a desired gameobject
            Debug.Log("Berhasil coeg");
            if(GetComponent<SceneDoors>() != null)
            {
                GetComponent<SceneDoors>().enabled = true;

                holdActivate = false;
            }
/*
            if (GetComponent<SceneDoors>() != null)
            {
                GetComponent<SceneDoors>().enabled = true;
            }
*/
        }
    }
    private void Start()
    {
        if (!gameObject.GetComponent<HintOfInstruction>().gameObject.activeSelf)
        {
            Debug.Log("Belum punya HoI di     _____" + gameObject.name);
        }
    }

}
