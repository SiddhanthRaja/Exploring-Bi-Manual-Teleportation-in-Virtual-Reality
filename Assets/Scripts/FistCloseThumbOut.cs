using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FistCloseThumbOut : CustomGesture
{

    [SerializeField]
    GameObject Global;

    private Pose indexTip;
    private Pose middleTip;
    private Pose pinkyTip;
    private Pose thumbFingerTip;

    private Pose Wrist;

    public override bool GestureDetected()
    {
        IHand ActiveHand = (handedness == Handedness.Right) ? RHand : LHand;

        if ((ActiveHand != null)
                &&
            ActiveHand.GetJointPose(HandJointId.HandIndexTip, out indexTip)
                 &&
                 ActiveHand.GetJointPose(HandJointId.HandMiddleTip, out middleTip)
                 &&
                 ActiveHand.GetJointPose(HandJointId.HandPinkyTip, out pinkyTip)
                 &&
                 ActiveHand.GetJointPose(HandJointId.HandThumbTip, out thumbFingerTip)
                 &&
                 ActiveHand.GetJointPose(HandJointId.HandWristRoot, out Wrist)
                 )
        {
            float WristToIndexDist = Vector3.Distance(Wrist.position, indexTip.position);
            float WristToMiddleDist = Vector3.Distance(Wrist.position, middleTip.position);
            float WristToPinkyDist = Vector3.Distance(Wrist.position, pinkyTip.position);
            float WristToThumbDist = Vector3.Distance(Wrist.position, thumbFingerTip.position);


            //Global.GetComponent<Global>().appendToDebugLog = "WI: " + WristToIndexDist + " - WM: " + WristToMiddleDist 
            //    + "\n WP: " + WristToPinkyDist + " - WT: " + WristToThumbDist;

            if (WristToIndexDist <= 0.13 && WristToMiddleDist <= 0.13&& WristToPinkyDist <= 0.12 && WristToThumbDist > 0.13)
            {
                return true;
            }
        }

        return false;
    }
}
