using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    [SerializeField] private Transform TargetDoor;
    private GameObject player;
    private Bounds DoorArea;
    // Start is called before the first frame update
    void Start()
    {
        DoorArea = GetComponent<BoxCollider2D>().bounds;
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void FixedUpdate()
    {
        if (player && Input.GetButtonDown("Interact"))
        {
            if (player.GetComponent<PlayerController>().CanInteract() &&
                player.transform.position.x < DoorArea.center.x + DoorArea.extents.x && player.transform.position.x > DoorArea.center.x - DoorArea.extents.x &&
                player.transform.position.y < DoorArea.center.y + DoorArea.extents.y && player.transform.position.y > DoorArea.center.y - DoorArea.extents.y)
            {
                StartCoroutine(Transfer());
            }
        }
    }
    IEnumerator Transfer()
    {
        player.GetComponent<PlayerController>().Freeze();
        yield return new WaitForSeconds(0.1f);
        player.transform.position = TargetDoor.position;
        player.GetComponent<PlayerController>().UnFreeze();
    }
}
