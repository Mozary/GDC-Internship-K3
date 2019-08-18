using UnityEngine;
using UnityEngine.Events;

public class InteractOnButton2D : InteractOnTrigger2D
{
    
    public static string Notes = "Salah satu collider di Player harus dimatikan terlebih/n " +
                                 "dahulu agar OnStay tidak terpanggil dua kali sekaligus.";

    public UnityEvent OnButtonPress;
    private bool m_CanExecuteButtons;

    //bool untuk Script lain
    [HideInInspector]
    public bool inCollider = false;
    protected override void ExecuteOnEnter(Collider2D other)
    {
        m_CanExecuteButtons = true;
        inCollider = true;
        OnEnter.Invoke();
    }
    protected override void ExecuteOnExit(Collider2D other)
    {
        m_CanExecuteButtons = false;
        inCollider = false;
        OnExit.Invoke();
    }
    protected override void ExecuteOnStay(Collider2D other)         
    {
        if (!m_CanExecuteButtons)
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

        if (gameObject.GetComponent<Interactable_Dialogue>() == null)
        {
            if (m_CanExecuteButtons && !dialogueBox.activeSelf)
            {
                if (Input.GetKeyDown("e"))
                {
                    OnStay.Invoke();
                }
            }
        }
    }

    private void Update()
    {
        if (m_CanExecuteButtons /* && detect player sedang idle */)      //pos.x pos.y == 0 ##################
        {
            if (Input.GetKeyDown("e"))
            {
                m_CanExecuteButtons = false;
                OnButtonPress.Invoke();
            }
        }
/*
        if (!dialogueBox.activeSelf && !i_player.enabled)
        {
            i_player.enabled = true;
        }
*/
    }
}
