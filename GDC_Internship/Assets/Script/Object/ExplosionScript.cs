using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScript : MonoBehaviour
{
    Animator animator;
    [SerializeField] private AudioSource Audio;
    private bool DestroyFlag = false;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f && !DestroyFlag)
        {
            DestroyFlag = true;
            Audio.Play();
            gameObject.SetActive(false);
            Destroy(gameObject,0.4f);
        }
    }
}
