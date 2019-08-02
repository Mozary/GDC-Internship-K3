using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealScript : MonoBehaviour
{

    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem Emitor;

    private Vector3 OriginScale;
    private Collider2D collitor;

    private void Start()
    {
        OriginScale = transform.localScale;
        //transform.localScale = Vector3.zero;
        collitor = GetComponent<Collider2D>();
    }
    private void Update()
    {
        if (transform.localScale.x < OriginScale.x)
        {
            float AnimTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            transform.localScale = new Vector3(AnimTime * OriginScale.x, AnimTime * OriginScale.x, 0);
        }
    }
    public void Disperse()
    {
        Debug.Log("HEAL DISPERSING");
        Emitor.Play();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Attack")
        {
            Destroy(gameObject);
        }
    }
}
