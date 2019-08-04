using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingCloud : MonoBehaviour
{
    private Vector3 cloudPos;
    private Vector3 cloudScale;

    private Vector3 oriPosition;
    private Vector3 oriScale;

    private float pos_x;
    private float scale_x;
    private float scale_y;
    private float oripos_x;
    private float oriscale_x;
    private float oriscale_y;

    public float speedPos = 0.01f;
    public float speedScale = 0.0002f;

    public bool constraintPos = false;
    public bool constraintScale = false;
    
    [Space]
    public bool reverseDirection = false;
    private int direction = -1;
    void Start()
    {
        oriPosition = gameObject.transform.position;
        oriScale = gameObject.transform.localScale;

        pos_x = oriPosition.x;
        scale_x = oriScale.x;
        scale_y = oriScale.y;
        oripos_x = oriPosition.x;
        oriscale_x = oriScale.x;
        oriscale_y = oriScale.y;

        if (reverseDirection)
        {
            direction *= -1;
        }
    }
    void Update()
    {
        cloudPos = new Vector3(pos_x, oriPosition.y, oriPosition.z);
        gameObject.transform.position = cloudPos;

        cloudScale = new Vector3(scale_x, scale_y, oriScale.z);
        gameObject.transform.localScale = cloudScale;

        if(!constraintPos)
        {
            pos_x = pos_x + (Time.deltaTime * speedPos * -direction);

            if (pos_x >= oripos_x * 1.5)
            {
                constraintPos = true;
            }
        }

        if(!constraintScale)
        {
            scale_x = scale_x + (Time.deltaTime * speedScale * -direction);
            scale_y = scale_y + (Time.deltaTime * speedScale * -direction);

            if (scale_x >= oriscale_x * 1.5 || scale_y >= oriscale_y * 1.5)
            {
                constraintScale = true;
            }
        }
    }
}
