using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public GameObject[] EnemytoDelete;

    private GameObject usedCutscene;

    private Cutscene_PlayerAnimator endMarkerClip;
    private bool endMarker = false;

    private GameObject activeDialogueClip;
    void Awake()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
    }
    void Update()
    {
        if (usedCutscene == null)
        {
            usedCutscene = gameObject.transform.GetChild(0).gameObject;
            endMarkerClip = usedCutscene.transform.GetChild(0).GetComponent<Cutscene_PlayerAnimator>();

            //// Set_Dialogue Terakhir, harus berada di posisi paling bawah
            activeDialogueClip = usedCutscene.transform.GetChild(usedCutscene.transform.childCount - 1).gameObject;
        }

        if (activeDialogueClip.activeSelf)
        {
            endMarker = endMarkerClip.moveMagic;
            if (!endMarker)
            {
                if (EnemytoDelete[0].transform.name == usedCutscene.transform.name)
                {
                    Destroy(EnemytoDelete[0]);
                }

                endMarker = false;
                dialogueManager.dialogueEnded = false;
                Destroy(usedCutscene);

            }
        }
    }
}
