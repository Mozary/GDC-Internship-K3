using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxScrolling : MonoBehaviour
{
    [SerializeField] float backgroundSize = 1600;
    [SerializeField] float ParallaxSpeed;
    [SerializeField] private bool ScrollToRight = false;

    private Transform CameraTransform;
    private Transform[] Layers;
    private float ViewZone = 10;
    private int LeftIndex;
    private int RightIndex;
    private float LastCameraX;

    private float LeftMostPos;
    private float RightMostPos;

    private float MovedDistance = 0;
    
    private void Start()
    {
        CameraTransform = Camera.main.transform;
        transform.position = CameraTransform.position;
        Layers = new Transform[transform.childCount];
        for(int i = 0; i < transform.childCount; i++)
        {
            Layers[i] = transform.GetChild(i);
        }
        LeftIndex = 0;
        RightIndex = Layers.Length - 1;
        
        LeftMostPos = transform.position.x-backgroundSize;
        RightMostPos = transform.position.x+backgroundSize;
    }

    private void Update()
    {
        if (ScrollToRight)
        {
            foreach (Transform image in Layers)
            {
                float NewX = image.position.x + (ParallaxSpeed * Time.deltaTime);
                Vector3 NewPos = new Vector3(NewX, image.position.y);
                image.position = NewPos;
            }
            MovedDistance++;
            if(MovedDistance >= backgroundSize/2)
            {
                MovedDistance = 0;
                ScrollLeft();
            }

        }
        else if (!ScrollToRight)
        {
            foreach (Transform image in Layers)
            {
                float NewX = image.position.x - (ParallaxSpeed * Time.deltaTime);
                Vector3 NewPos = new Vector3(NewX, image.position.y);
                image.position = NewPos;
                
            }
            MovedDistance--;
            if (MovedDistance <= -backgroundSize/2)
            {
                MovedDistance = 0;
                ScrollRight();
            }
        }
        
        
    }
    private void ScrollLeft()
    {
        Debug.Log("Scrolling Left");
        int LastRight = RightIndex;
        Vector3 NewPos = Layers[RightIndex].localPosition;

        Layers[RightIndex].localPosition = new Vector3(LeftMostPos,NewPos.y, NewPos.z);
        LeftIndex = RightIndex;
        RightIndex--;
        if(RightIndex < 0)
        {
            RightIndex = Layers.Length - 1;
        }
    }
    private void ScrollRight()
    {
        Debug.Log("Scrolling Left");
        int LastLeft = LeftIndex;
        Vector3 NewPos = Layers[RightIndex].localPosition;

        Layers[LeftIndex].localPosition = new Vector3(RightMostPos, NewPos.y, NewPos.z);
        RightIndex = LeftIndex;
        LeftIndex++;
        if (LeftIndex == Layers.Length)
        {
            LeftIndex = 0;
        }
    }
}
