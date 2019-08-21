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
            if(gameObject.transform.GetChild(0) != null)
            {
                usedCutscene = gameObject.transform.GetChild(0).gameObject;
                endMarkerClip = usedCutscene.transform.GetChild(0).GetComponent<Cutscene_PlayerAnimator>();
                Debug.Log(usedCutscene);
                Debug.Log(endMarkerClip);

                //// Set_Dialogue Terakhir, harus berada di posisi paling bawah
                activeDialogueClip = usedCutscene.transform.GetChild(usedCutscene.transform.childCount - 1).gameObject;
            }
        }

        if (activeDialogueClip.activeSelf)
        {
            endMarker = endMarkerClip.moveMagic;

            if (!endMarker)
            {
                for (int i = 0; i < EnemytoDelete.Length; i++)
                {
                    Debug.Log(i);
                    if (EnemytoDelete[i].transform.name == usedCutscene.transform.name)
                    {
                        Destroy(EnemytoDelete[i]);
                    }
                }

                endMarker = false;
                dialogueManager.dialogueEnded = false;
                Destroy(usedCutscene);
            }
        }
    }
}
