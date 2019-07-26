using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public class TimeMachineBehaviour : PlayableBehaviour
{
	public TimeMachineAction action;
	public Condition condition;
	public string markerToJumpTo, markerLabel;
	public float timeToJumpTo;

    //public Platoon platoon;
    public DialogueManager dialogueManager;
    private bool dialogueEnded = false;

	[HideInInspector]
	public bool clipExecuted = false; //the user shouldn't author this, the Mixer does

	public bool ConditionMet()
	{
		switch(condition)
		{
			case Condition.Always:
				return true;
            
            /*
            case Condition.PlatoonIsAlive:
            //The Timeline will jump to the label or time if a specific Platoon still has at least 1 unit alive

            if(platoon != null)
            {
                return !platoon.CheckIfAllDead();
            }
            else
            {
                return false;
            }
            */

            case Condition.runningDialogue:

                if (dialogueEnded)
                {
                    Debug.Log("udah masuk ke _______condition");

                    //dialogueManager.dialogueEnded = false;      //cari tempat untuk toogle ini
                    //dialogueEnded = false;
                    return false;
                }
                else
                {
                    return true;
                }

            case Condition.Never:
			default:
				return false;
		}
	}

	public enum TimeMachineAction
	{
		Marker,
		JumpToTime,
		JumpToMarker,
		//Pause,
	}

	public enum Condition
	{
		Always,
		Never,
		//PlatoonIsAlive,
        runningDialogue,
	}

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);

        if (condition == Condition.runningDialogue)
        {
            dialogueEnded = dialogueManager.dialogueEnded;
        }
    }
}
