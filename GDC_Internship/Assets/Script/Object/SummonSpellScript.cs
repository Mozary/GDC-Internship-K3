using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SummonSpellScript : MonoBehaviour
{
    [SerializeField] private GameObject Summon1;
    [SerializeField] private GameObject Summon2;
    [SerializeField] private GameObject Summon3;

    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }
    private void Awake()
    {
        
    }
    void Update()
    {
        if(animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            Destroy(gameObject);
        }
    }
}
