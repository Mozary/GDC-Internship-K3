using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

//use it as component in game object that can trigger scene changing
//note that the player will have *some atribute* that will carried over between scene
//note again that 'bar bahan obat/ramuan' too will be carried over between scene
//note that enemy should and not should be get spawn in some of the scene
//     especially with menu system that can load progress
//maybe more will come in the future
public class SceneLoader : MonoBehaviour
{
    InteractOnButton2D interactOnButton2D;
    //PlayerController i_player;

    GameObject parentCanvas;
    GameObject dialogueBox;
    GameObject hintofInstruction;
    GameObject fade_out;
    private bool inCollider;

    [SerializeField]
    private string SceneName;
    [HideInInspector]
    private bool doMoveScene = false;
    DialogueManager dialogueManager;
    [SerializeField]
    private Animator aniHold;
    GameObject hold_button;
    [SerializeField]
    private Animator fadeScreen;

    private float holdStart = 0f;
    private float holdPass = 3f;
    private bool holdActivate = false;
    private float holdInstruction = 0f;

    public TextMeshProUGUI HintofInstruction;
    public string instruction;
    private bool moveScene;

    private void Start()
    {
        interactOnButton2D = GetComponent<InteractOnButton2D>();
        //i_player = interactOnButton2D.i_player;

        parentCanvas = GameObject.FindGameObjectWithTag("Canvas");
        dialogueBox = parentCanvas.transform.GetChild(0).gameObject;
        hintofInstruction = parentCanvas.transform.GetChild(1).gameObject;
        fade_out = parentCanvas.transform.GetChild(2).gameObject;

        dialogueManager = FindObjectOfType<DialogueManager>();
        HintofInstruction.text = instruction;

        hold_button = gameObject.transform.GetChild(0).gameObject;
    }
    private void Update()
    {
        inCollider = interactOnButton2D.inCollider;
        moveScene = dialogueManager.dialogueEnded;

        //instruction on holding key is displayed in other script
        if (Input.GetKey("e") && inCollider && doMoveScene)
        {
            holdStart += Time.deltaTime;
            Debug.Log("ini holdStart ____" + holdStart);
            
            hold_button.SetActive(true);
            aniHold.SetTrigger("holdButton");
            if(holdStart > holdPass)
            {
                Debug.Log("holdActivate sudah true ____dari scene loader");

                holdActivate = true;
                holdStart = holdStart - holdPass;
            }
        }
        else
        {
            hold_button.SetActive(false);
            holdStart = 0f;
        }

        if(holdActivate)
        {
            Debug.Log("scene berubah ____dari scene loader");

            fade_out.SetActive(true);
            fadeScreen.SetTrigger("fadeOut");
            SceneManager.LoadScene(SceneName);
            holdActivate = false;
        }

        if (inCollider /* && detect player sedang idle */)      //belum rangkum ##################
        {
            if(moveScene)
            {
                Debug.Log("masuk ke moveScene  ___if");
                doMoveScene = true;

                if (!hintofInstruction.activeSelf)
                {
                    hintofInstruction.SetActive(true);
                }
                else
                {
                    holdInstruction += Time.deltaTime;
                    Debug.Log("ini holdInstruction ___" + holdInstruction);
                }

                if (holdInstruction > 3f)
                {
                    hintofInstruction.SetActive(false);
                    holdInstruction = -10f;
                }
            }
        }
        else
        {
            hintofInstruction.SetActive(false);
            doMoveScene = false;
        }
    }
}
