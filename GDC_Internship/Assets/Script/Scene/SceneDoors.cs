using UnityEngine;

public class SceneDoors : MonoBehaviour
{
    GameObject i_player;
    public GameObject targetPosition;

    GameObject parentCanvas;
    GameObject dialogueBox;

    InteractOnButton2D interactOnButton2D;
    private bool inCollider;

    private void Start()
    {
        i_player = GameObject.FindGameObjectWithTag("Player");

        parentCanvas = GameObject.FindGameObjectWithTag("Canvas");
        dialogueBox = parentCanvas.transform.GetChild(0).gameObject;

        interactOnButton2D = GetComponent<InteractOnButton2D>();
    }

    private void Update()
    {
        inCollider = interactOnButton2D.inCollider;

        if (!dialogueBox.activeSelf && inCollider)
        {
            if (Input.GetKeyDown("e")) 
            i_player.transform.position = targetPosition.transform.position;
        }
    }
}
