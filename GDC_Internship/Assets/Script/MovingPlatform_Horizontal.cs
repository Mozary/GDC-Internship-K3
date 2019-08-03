using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform_Horizontal : MonoBehaviour
{
    private Vector3 parentTransform;
    private Vector3 platformPos;
    private float pos_x;

    private float moveSpeed = 1f;
    private int direction = 1;

    public float leftEdge;
    public float rightEdge;
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

    private void Start()
    {
        parentTransform = gameObject.transform.parent.position;
        pos_x = parentTransform.x;
    }
    private void Update()
    {
        platformPos = new Vector3(pos_x, parentTransform.y, parentTransform.z);
        gameObject.transform.parent.position = platformPos;     //dont use parentTransform because it is not changing the original value

        pos_x = pos_x + (moveSpeed * Time.deltaTime * -direction);

        if(pos_x >= rightEdge || pos_x <= leftEdge)
        {
            direction = direction * -1;
        }
    }
}
