using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWater : MonoBehaviour
{
    public Vector3 waterPos;
    public Vector3 waterScale;

    private Vector3 oriPosition;
    private Vector3 oriScale;

    private float pos_x;
    public float pos_x_offset;

    private float scale_x;
    private float scale_y;
    public float scale_x_offset;
    public float scale_y_offset;

    private float oripos_x;
    private float oriscale_x;
    private float oriscale_y;

    private int resetValue_a = -1;
    private int resetValue_b = -1;
    private int resetValue_c = -1;

    public bool reverseDirection = false;
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
            resetValue_a *= -1;
            resetValue_b *= -1;
            resetValue_c *= -1;
        }
    }
    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            waterPos = new Vector3(pos_x, oriPosition.y, oriPosition.z);
            gameObject.transform.position = waterPos;

            waterScale = new Vector3(scale_x, scale_y, oriScale.z);
            gameObject.transform.localScale = waterScale;

            //// position
            if (true)
            {
                pos_x = pos_x + (pos_x_offset * Time.deltaTime * -resetValue_a);
                if (pos_x >= oripos_x + pos_x_offset || pos_x <= oripos_x - pos_x_offset)
                {
                    resetValue_a *= -1;
                }
            }

            //// scale
            if (true)
            {
                scale_x = scale_x + (scale_x_offset * Time.deltaTime * -resetValue_b);
                if (scale_x >= oriscale_x + scale_x_offset || scale_x <= oriscale_x - scale_x_offset)
                {
                    resetValue_b *= -1;
                }
                
            }
            if (true)
            {
                scale_y = scale_y + (scale_y_offset * Time.deltaTime * -resetValue_c);
                if (scale_y >= oriscale_y + scale_y_offset || scale_y <= oriscale_y - scale_y_offset)
                {
                    resetValue_c *= -1;
                }
            }
        }
    }
}
