using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapCollisionDetector : MonoBehaviour
{
    CharacterController2D characterController2D;
    bool isJumping;

    TilemapCollider2D kolaider;
    void Start()
    {
        characterController2D = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController2D>();
        isJumping = characterController2D.isJumping;

        kolaider = gameObject.GetComponent<TilemapCollider2D>();
    }
    void Update()
    {
        if(isJumping)
        {
            kolaider.enabled = false;
        }

    }
}
