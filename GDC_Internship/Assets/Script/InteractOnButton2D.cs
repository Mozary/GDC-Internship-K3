using UnityEngine;
using UnityEngine.Events;

public class InteractOnButton2D : InteractOnTrigger2D
{
    
    public static string Notes = "Salah satu collider di Player harus dimatikan terlebih/n " +
             "dahulu agar OnStay tidak terpanggil dua kali sekaligus.";

    public UnityEvent OnButtonPress;
    private bool m_CanExecuteButtons;

    private GameObject parentCanvas;
    private GameObject dialogueBox;
    private DialogueManager dialogueManager;
    private Interactable_Dialogue cekDialogue;
    [HideInInspector]                     //dijadikan public karena SceneLoader ########################################
    public PlayerController i_player;

    //bool untuk SceneLoader
    [HideInInspector]
    public bool inCollider;

    float a;
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
    int countt = 0;
    protected override void ExecuteOnStay(Collider2D other)         
    {
        if (!m_CanExecuteButtons && dialogueBox.activeSelf)
        {
            if (Input.GetKeyDown("e"))
            {
                OnStay.Invoke();
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

    private void Start()
    {
        parentCanvas = GameObject.FindGameObjectWithTag("Canvas");
        dialogueBox = parentCanvas.transform.GetChild(0).gameObject;

        cekDialogue = gameObject.GetComponent<Interactable_Dialogue>();
        if (cekDialogue != null)
        {
            dialogueManager = FindObjectOfType<DialogueManager>();
        }
        
        i_player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();

        inCollider = false;
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
