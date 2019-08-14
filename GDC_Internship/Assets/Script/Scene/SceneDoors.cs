using UnityEngine;

public class SceneDoors : MonoBehaviour
{
    public GameObject i_player;
    public GameObject targetPosition;

    public GameObject dialogueBox;

    private InteractOnButton2D interactOnButton2D;
    private bool inCollider;

    private void Start()
    {
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
