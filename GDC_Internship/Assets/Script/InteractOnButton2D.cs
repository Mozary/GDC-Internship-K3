using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractOnButton2D : InteractOnTrigger2D
{
    public UnityEvent OnButtonPress;

    bool m_CanExecuteButtons;

    protected override void ExecuteOnEnter(Collider2D other)
    {
        m_CanExecuteButtons = true;
        OnEnter.Invoke();
    }

    protected override void ExecuteOnExit(Collider2D other)
    {
        m_CanExecuteButtons = false;
        OnExit.Invoke();
    }

    void Update()
    {
        GameObject checkCanvas = GameObject.FindGameObjectWithTag("Canvas");

        if (!m_CanExecuteButtons && Input.GetKeyDown("e") && checkCanvas.activeSelf) 
        {
            Debug.Log("kondisi terpenuhii untuk manggil display next sentence");
            FindObjectOfType<DialogueManager>().DisplayNextSentence();
        }
        else 
        {
            //Debug.Log("kondisi tidak terpenuhii");
        }

        if (m_CanExecuteButtons) 
        {
            if (Input.GetKeyDown("e"))
            {
                Debug.Log("invoke di update terjadih");
                m_CanExecuteButtons = false;
                OnButtonPress.Invoke();
            }
        }

        if (!checkCanvas.activeSelf && GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled == false)
        {
            Debug.Log("bisa gerak lagee");
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled = true;
        }
        else
        {
            Debug.Log(GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>().enabled);
            Debug.Log(" 3");

            Debug.Log("masih gak bisa geraaakkkk");

            Debug.Log(checkCanvas.activeSelf);
            Debug.Log(" 2");
        }
    }
}
