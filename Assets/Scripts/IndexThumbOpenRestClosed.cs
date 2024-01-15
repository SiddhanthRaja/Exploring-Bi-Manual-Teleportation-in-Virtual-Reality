using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexThumbOpenRestClosed : CustomGesture
{

    [SerializeField]
    GameObject Global;

    private Pose indexTip, indexMiddle;
    private Pose middleTip, middleMiddle;
    private Pose pinkyTip, pinkyMiddle;

    private Pose Wrist;

    private bool leftGestureActive, rightGestureActive = false;
    public override bool GestureDetected()
    {
        /*
        IHand ActiveHand = (handedness == Handedness.Right) ? RHand : LHand;
   
        if (
                (ActiveHand != null)
                &&
                ActiveHand.GetJointPose(HandJointId.HandIndexTip, out indexTip)
                &&
             ActiveHand.GetJointPose(HandJointId.HandIndex1, out indexMiddle)
             &&
             ActiveHand.GetJointPose(HandJointId.HandMiddleTip, out middleTip)
             &&
             ActiveHand.GetJointPose(HandJointId.HandMiddle1, out middleMiddle)
             &&
             ActiveHand.GetJointPose(HandJointId.HandPinkyTip, out pinkyTip)
             &&
             ActiveHand.GetJointPose(HandJointId.HandPinky1, out pinkyMiddle)
             &&
             ActiveHand.GetRootPose(out Wrist)
             )
        {
            ActiveHand.GetJointPoseFromWrist(HandJointId.HandIndexTip, out Pose indexTipFromWrist);
        
            //Debug.Log("### IndexThumbOpenRestClosed - if cond True");
            float indexWristAngle = Vector3.Angle(indexTip.position - indexMiddle.position, Wrist.forward);
            float middleWristAngle = Vector3.Angle(middleTip.position - middleMiddle.position, Wrist.forward);
            float pinkyWristAngle = Vector3.Angle(pinkyTip.position - pinkyMiddle.position, Wrist.forward);

            float indexTipWristAngle = Vector3.Angle(indexTip.position, Wrist.forward);
            float indexRelativeWristAngle = Vector3.Angle(indexTipFromWrist.position, Wrist.forward);

            if (indexWristAngle < 80 && middleWristAngle > 80 && pinkyWristAngle > 80)
            {
                //Debug.Log("### IndexThumbOpenRestClosed - Angles - " + indexWristAngle + " , " + middleWristAngle + " , " + pinkyWristAngle);
                Debug.Log("### IndexThumbOpenRestClosed - detected - ActivatePointer");

                Debug.Log("indexWristAngle = " + indexWristAngle);
                Debug.Log("indexTipWristAngle = " + indexTipWristAngle);
                Debug.Log("indexRelativeWristAngle = " + indexRelativeWristAngle);

                //Debug.Log("### IndexThumbOpenRestClosed - Angles - " + indexWristAngle + " , " + middleWristAngle + " , " + pinkyWristAngle);
                return true;
            }
        }

    
        //Debug.Log("### IndexThumbOpenRestClosed - if cond False");
        return false;
    */

        if (handedness == Handedness.Left && leftGestureActive)
        {
            //leftGestureActive = false;
            return true;
        }
        else if (handedness == Handedness.Right && rightGestureActive)
        {
            //rightGestureActive = false;
            return true;
        }

        return false;
    }

    public void setLeft()
    {
        leftGestureActive = true;
    }
    public void setRight()
    {
        rightGestureActive = true;
    }
    
    public void unsetLeft()
    {
        leftGestureActive = false;
    }
    public void unsetRight()
    {
        rightGestureActive = false;
    }

}
