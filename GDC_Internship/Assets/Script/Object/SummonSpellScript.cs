using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSpellScript : MonoBehaviour
{

    private Animator animator;
    [SerializeField] private AudioSource Audio;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        Audio.Play();
    }
    void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !Audio.isPlaying)
        {
            Destroy(gameObject);
        }
    }
}
