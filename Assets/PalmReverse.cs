using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmReverse : CustomGesture
{
    private Pose Wrist;

    public override bool GestureDetected()
    {

        IHand ActiveHand = (handedness == Handedness.Right) ? RHand : LHand;
            if (
            (ActiveHand != null)
            &&
            ActiveHand.GetJointPose(HandJointId.HandWristRoot, out Wrist)

            )
        {
            if(Wrist.up.y >= 0.2)
            {
                return true;
            }
        }

        return false;
    }
}
