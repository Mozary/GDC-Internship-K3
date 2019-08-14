using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubTilemap : MonoBehaviour
{
    private PlatformEffector2D effector2D;
    private PlayerController player;

    private float rotOffset;
    private float delayOffset;
    void Start()
    {
        effector2D = gameObject.GetComponent<PlatformEffector2D>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    void Update()
    {
        if (player.jumpDown && delayOffset >= 0.298f)
        {
            effector2D.colliderMask |= LayerMask.GetMask("Player");

            player.jumpDown = false;
            delayOffset = 0.0f;
        }
        if (player.jumpDown)
        {
            effector2D.colliderMask &= ~LayerMask.GetMask("Player");

            delayOffset += Time.deltaTime;
        }
    }
}
