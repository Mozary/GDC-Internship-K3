using UnityEngine;

public class SceneDoors_ch2 : SceneDoors
{
    private bool p_PosAbove = false;
    private bool e_PosAbove = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (p_PosAbove && !e_PosAbove || !p_PosAbove && e_PosAbove)
        {
            if (collision.gameObject == enemy)
            {
                collision.transform.position = targetPosition.transform.position;
            }
        }
    }
}
