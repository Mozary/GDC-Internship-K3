using UnityEngine;
using UnityEngine.Playables;

public class Cutscene_PlayerAnimator : MonoBehaviour
{
    public GameObject i_player;
    private Animator i_pAnimator;
    private PlayerController i_pController;

    private RuntimeAnimatorController runtimeAnimator;
    public PlayableDirector playableDirector;

    //used in CutsceneManager
    [HideInInspector]
    public bool moveMagic = false;
    private void Awake()
    {
        i_pAnimator = i_player.GetComponent<Animator>();
        i_pController = i_player.GetComponent<PlayerController>();

        runtimeAnimator = i_pAnimator.runtimeAnimatorController;
    }
    private void OnEnable()
    {
        moveMagic = true;
        i_pAnimator.runtimeAnimatorController = null;
        i_pController.enabled = false;
    }
    private void Update()
    {
        if (moveMagic)
        {
            if (playableDirector.state != PlayState.Playing)
            {
                moveMagic = false;
                i_pAnimator.runtimeAnimatorController = runtimeAnimator;
                i_pController.enabled = true;
            }
        }
    }
}
