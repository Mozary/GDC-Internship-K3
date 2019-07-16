using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class CutsceneManager : MonoBehaviour
{
    private Animator i_player;
    public PlayableDirector playableDirector;

    private bool tag1 = true;
    private bool tag2 = true;

    private void Start()
    {
        i_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Animator>();
    }
    private void Update()
    {
        if (gameObject.activeSelf && tag1)
        {
            OnEnable();
        }
        if (playableDirector.state != PlayState.Playing && tag2)
        {
            i_player.enabled = false;
            tag2 = false;
            abababa();
        }
    }
    private void OnEnable()
    {
        i_player.applyRootMotion = true;
        tag1 = false;
        Debug.Log("kepanggil");
    }
    private void abababa()
    {
        /*i_player.applyRootMotion = false;*/
        Debug.Log("kepanggil ondisablenya");
        i_player.enabled = true;
        delayedReset();
    }
    void delayedReset()
    {
        StartCoroutine(delayedReset(0.5f));
    }
    private IEnumerator delayedReset(float s)
    {
        Debug.Log("kepanggil coroutinenya");
        yield return new WaitForSeconds(s);
        i_player.applyRootMotion = false;
        /*gameObject.SetActive(false);*/
    }
}
