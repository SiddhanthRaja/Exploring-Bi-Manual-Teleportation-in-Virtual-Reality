using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalmReverseFistClosed : CustomGesture
{

    private Pose Wrist;
    private Pose indexTip, indexMiddle;
    public float angleBetweenWristandIndexThreshold = 70;

    public override bool GestureDetected()
    {
        IHand ActiveHand = (handedness == Handedness.Right) ? RHand : LHand;
        if (
                (ActiveHand != null)
                &&
                ActiveHand.GetJointPose(HandJointId.HandIndexTip, out indexTip)
                 &&
                 ActiveHand.GetJointPose(HandJointId.HandIndex2, out indexMiddle)
                 &&
                 ActiveHand.GetJointPose(HandJointId.HandWristRoot, out Wrist)
                 )
        {
            Vector3 indexFingerLine = indexTip.position - indexMiddle.position;
            float angleBetweenWristandIndex = Vector3.Angle(indexFingerLine, Wrist.forward);


            if (Wrist.up.y >= 0.2 && angleBetweenWristandIndex > angleBetweenWristandIndexThreshold)
            {
                return true;
            }
        }

        return false;
    }
}
