﻿using UnityEngine;

public class HerbScript : MonoBehaviour
{
    [SerializeField] private AudioSource Audio;
    [SerializeField] private AudioClip GetHerb;
    private new Collider2D collider;
    void Awake()
    {

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Canvas")
        {
            Destroy(this.gameObject);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && gameObject.layer != LayerMask.NameToLayer("UI"))
        {
            Audio.PlayOneShot(GetHerb);
            gameObject.layer = LayerMask.NameToLayer("UI");
            collision.gameObject.GetComponent<PlayerController>().AddCollectedHerb();
            GetComponent<Rigidbody2D>().gravityScale = 0;
        }
    }
}
