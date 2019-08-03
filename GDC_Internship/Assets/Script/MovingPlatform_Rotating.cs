using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_Rotating : MonoBehaviour
{
    private GameObject rotatingPlatform;
    public Vector3 platformAngle;

    private float rotateDelay;
    public float rotateSpeed = 5.0f;
    public bool reverseDirection = false;

    private void Start()
    {
        rotatingPlatform = gameObject.transform.parent.gameObject;

        platformAngle = rotatingPlatform.transform.eulerAngles;
        platformAngle.z = 0.0f;

        if (reverseDirection)
        {
            rotateSpeed = -rotateSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.parent = gameObject.transform;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.transform.parent = null;
        }
    }

    private void Update()
    {
        rotateDelay += Time.deltaTime;

        if (rotateDelay >= 2.0f)
        {
            rotatingPlatform.transform.Rotate(0.0f, 0.0f, rotateSpeed, Space.Self);
            gameObject.transform.Rotate(0.0f, 0.0f, -rotateSpeed, Space.Self);
            platformAngle.z += rotateSpeed;

            if (platformAngle.z == 0 || platformAngle.z == 90 || platformAngle.z == 180 || platformAngle.z == 270 || platformAngle.z == 360 ||
                platformAngle.z == -90 || platformAngle.z == -180 || platformAngle.z == -270 || platformAngle.z == -360)
            {
                rotateDelay = 0.0f;

                if (platformAngle.z >= 360 || platformAngle.z <= -360)
                {
                    platformAngle.z = 0.0f;
                }
            }
        }
    }
}
