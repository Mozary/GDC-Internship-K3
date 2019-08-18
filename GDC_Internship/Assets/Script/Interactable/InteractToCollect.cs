using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractToCollect : MonoBehaviour
{
    public DialogueManager dialogueManager;
    public PlayerController i_player;
    public int herbAmount;

    public void CollectHerb()
    {
        if (dialogueManager.dialogueEnded)
        {
            for (int i = 0; i <= herbAmount; i++)
            {
                i_player.AddCollectedHerb();
            }
        }
    }
    public void DroppedHerb()
    {
        for (int i = 0; i <= herbAmount; i++)
        {
            i_player.AddCollectedHerb();
        }
    }
}
