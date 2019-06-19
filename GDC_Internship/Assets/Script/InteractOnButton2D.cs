using UnityEngine;
using UnityEngine.Events;

public class InteractOnButton2D : InteractOnTrigger2D
{
    public UnityEvent OnButtonPress;
    bool m_CanExecuteButtons;

    GameObject parentCanvas;
    GameObject dialogueBox;
    [HideInInspector]                     //dijadikan public karena SceneLoader ########################################
    public PlayerController i_player;

    //bool untuk SceneLoader
    [HideInInspector]
    public bool inCollider;

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

    void Start()
    {
        parentCanvas = GameObject.FindGameObjectWithTag("Canvas");
        dialogueBox = parentCanvas.transform.GetChild(0).gameObject;
        
        i_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        inCollider = false;
    }
    void Update()
    {
        if (!m_CanExecuteButtons && Input.GetKeyDown("e") && dialogueBox.activeSelf)
        {
            FindObjectOfType<DialogueManager>().DisplayNextSentence();
        }
        if (m_CanExecuteButtons /* && detect player sedang idle */)      //belum rangkum ##################
        {
            if (Input.GetKeyDown("e"))
            {
                m_CanExecuteButtons = false;
                OnButtonPress.Invoke();
            }
        }

        if (!dialogueBox.activeSelf && !i_player.enabled)
        {
            i_player.enabled = true;
        }
    }
    void branchEvent()
    {
        //make a special event with code here


    }
}
