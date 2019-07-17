using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// **** NOTES ****
//use it as component in game object that can trigger scene changing
//note that the player will have *some atribute* that will carried over between scene
//note again that 'bar bahan obat/ramuan' too will be carried over between scene
//note that enemy should and not should be get spawn in some of the scene
//     especially with menu system that can load progress
//maybe more notes will come in the future
// ******  ******
public class SceneLoader : MonoBehaviour
{
    InteractOnButton2D interactOnButton2D;
    private bool inCollider;

    DialogueManager dialogueManager;

    [SerializeField]
    private string SceneName;
    private bool doMoveScene = false;

    GameObject parentCanvas;
    [SerializeField]
    private Animator aniHold;
    GameObject hold_button;
    GameObject fade_out;
    [SerializeField]
    private Animator fadeScreen;

    GameObject hintofInstruction;
    public TextMeshProUGUI HintofInstruction;
    public string instruction;
    private bool moveScene;


    private float holdStart = 0.0f;
    private float holdPass = 3.0f;
    private bool holdActivate = false;
    private float holdInstruction = 0.0f;

    private void Start()
    {
        interactOnButton2D = GetComponent<InteractOnButton2D>();

        parentCanvas = GameObject.FindGameObjectWithTag("Canvas");
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

        if (inCollider /* && detect player sedang idle */)      //belum rangkum ##################)
        {
            //////
            if (moveScene && !doMoveScene)
            {
                doMoveScene = true;
                hintofInstruction.SetActive(true);
                holdInstruction += Time.deltaTime;
            }
            if (!hintofInstruction.activeSelf && doMoveScene)
            {
                holdInstruction += Time.deltaTime;

                if (holdInstruction > 10.0f)
                {
                    hintofInstruction.SetActive(true);
                    holdInstruction = 0.0f;
                }
            }
            //////
            if (doMoveScene)
            {
                if (Input.GetKey("e"))
                {
                    holdStart += Time.deltaTime;
                    hold_button.SetActive(true);
                    aniHold.SetTrigger("holdButton");
                    if (holdStart > holdPass)
                    {
                        holdActivate = true;
                        holdStart = holdStart - holdPass;
                    }
                }
                else
                {
                    hold_button.SetActive(false);
                    holdStart = 0.0f;
                }
            }
            //////
            if (holdActivate)
            {
                Debug.Log("scene berubah ____dari scene loader");

                fade_out.SetActive(true);
                fadeScreen.SetTrigger("fadeOut");
                holdActivate = false;
                SceneManager.LoadScene(SceneName);
            }
        }
        else
        {
            moveScene = false;
            doMoveScene = false;
            holdActivate = false;
            hold_button.SetActive(false);
        }

        
        if (hintofInstruction.activeSelf)
        {
            holdInstruction += Time.deltaTime;

            if (holdInstruction > 3.0f)
            {
                hintofInstruction.SetActive(false);
                holdInstruction = 0.0f;
            }
        }
    }
}
