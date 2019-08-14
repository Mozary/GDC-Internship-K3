using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWaterManager : MonoBehaviour
{
    private GameObject[] arrPlacementWater;
    [Range(1,4)] private float flip = 1;

    private void Start()
    {
        arrPlacementWater = new GameObject[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            arrPlacementWater[i] = transform.GetChild(i).gameObject;
        }
    }
    private void PlayerinRange()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            arrPlacementWater[i].SetActive(true);
        }

        flip += 1.0f;
    }
    private void PlayerNOTinRange()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            arrPlacementWater[i].SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            {
                PlayerinRange();
            }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        flip -= 0.5f;
        if (flip == 3)
        {
            flip -= 2;
        }
        if (flip%2 == 0)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerNOTinRange();
            }
        }
    }

}
