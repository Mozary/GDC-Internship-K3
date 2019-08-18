using TMPro;
using UnityEngine;

public class HintOfInstruction : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public GameObject hio_Instruction;

    public TextMeshProUGUI HintofInstruction;
    public string instruction;

    private bool haveActive = false;
    private bool readyActive;
    private float holdActive;
    float tryActive = 0.0f;
    void Start()
    {
        HintofInstruction.text = instruction;
    }
    void Update()
    {
        if (!hio_Instruction.activeSelf)
        {
            if (!haveActive)
            {
                readyActive = dialogueManager.dialogueEnded;
                Debug.Log("masuk?  ___dialogue ended terupdate");

                if (readyActive)
                {
                    tryActive += Time.deltaTime;

                    if (tryActive > 3.0f)
                    {
                        hio_Instruction.SetActive(true);
                        haveActive = true;
                        tryActive = 0.0f;
                    }
                }
            }
            else

            {
                holdActive += Time.deltaTime;

                if(holdActive > 10.0f)
                {
                    hio_Instruction.SetActive(true);
                    holdActive = 0.0f;
                }
            }
        }
        else

        {
            holdActive += Time.deltaTime;

            if(holdActive > 3.0f)
            {
                hio_Instruction.SetActive(false);
            }
        }
    }
}
