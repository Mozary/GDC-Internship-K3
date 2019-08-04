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
        if (effector2D.rotationalOffset != 0.0f && delayOffset >= 0.298f)
        {
            Debug.Log("Masuk?? atass");
            effector2D.rotationalOffset = 0.0f;
            player.jumpDown = false;
            delayOffset = 0.0f;
        }
        if (player.jumpDown)
        {
            Debug.Log("Masuk??");
            effector2D.rotationalOffset = 180.0f;
            delayOffset += Time.deltaTime;
        }
    }
}
