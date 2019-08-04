using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    private DialogueManager dialogueManager;

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
            activeDialogueClip = usedCutscene.transform.GetChild(1).gameObject;    // dialogue terakhir harus diposisi ke dua
        }

        if (activeDialogueClip.activeSelf)
        {
            endMarker = endMarkerClip.moveMagic;
            if (!endMarker)
            {
                endMarker = false;
                dialogueManager.dialogueEnded = false;
                Destroy(usedCutscene);
            }
        }
    }
}
