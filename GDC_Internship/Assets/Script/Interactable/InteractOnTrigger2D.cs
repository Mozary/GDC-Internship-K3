using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InteractOnTrigger2D : MonoBehaviour
{
    public GameObject dialogueBox;
    public PlayerController i_player;

    private bool readyActive;

    ////
    [Space]
    public LayerMask layers;                                        //biar gampang bedain layer?
    public UnityEvent OnEnter, OnExit, OnStay;                      //bisa MANGGIL SESUATU dengan gampang!

    protected Collider2D m_Collider;
    void Reset()                                                    //gak ngerti kenapa reset, tapi tahu fungsinya
    {
        layers = LayerMask.NameToLayer("Everything");
        m_Collider = GetComponent<Collider2D>();
        m_Collider.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!enabled)                                                //coba jangan dipake dulu, kayak, bukannya udah pasti terjadi?
            return;

        if (layers.Contains(other.gameObject))                       //Contains di sini dicover di LayerMaskExtension
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ExecuteOnEnter(other);                               //other kayaknya input kita sendiri dari menu di unity
            }
        }
    }
    void OnTriggerExit2D(Collider2D other)                           //sama dengan diatas
    {
        if (!enabled)
            return;

        if (layers.Contains(other.gameObject))
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ExecuteOnExit(other);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (!enabled)                                                
            return;

        if (layers.Contains(other.gameObject))                       
        {
            if (other.gameObject.CompareTag("Player"))
            {
                ExecuteOnStay(other);                                   
            }
        }

        if (GetComponent<Interactable_Dialogue>() != null)
        {
            if (readyActive)
            {
                if (!dialogueBox.activeSelf)
                {
                    Debug.Log("masuk? ____bisa gerak");
                    i_player.UnFreeze();
                }
            }
        }
    }

    protected virtual void ExecuteOnEnter(Collider2D other)         //virtual itu buat anak class
    {
        Debug.Log("masuk? ____siap aktif");
        readyActive = true;
        OnEnter.Invoke();
    }
    protected virtual void ExecuteOnExit(Collider2D other)
    {
        readyActive = false;
        OnExit.Invoke();
    }
    protected virtual void ExecuteOnStay(Collider2D other)         
    {
        if(GetComponent<Interactable_Dialogue>() != null)
        {
            if (readyActive)
            {
                if (dialogueBox.activeSelf)
                {
                    if (Input.GetKeyDown("e"))
                    {
                        OnStay.Invoke();
                    }
                }
                else
                {
                    Debug.Log("masuk? ____bisa gerak");
                    i_player.UnFreeze();
                }
            }
        }
        else
        {
            OnStay.Invoke();
        }
    }
}

