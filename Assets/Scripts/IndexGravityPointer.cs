using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexGravityPointer : HandTrackGravityPointer
{
    
    private Vector3 indexFinger;
    Pose IndexTip, IndexDJ;
    void Start()
    {
        Debug.Log("### IndexGravityPointer - INIT BASE");
        base.init();

        /*
        if (handJointService != null)
        {
            IndexTip = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Right);
            //IndexDJ = handJointService.RequestJointTransform(TrackedHandJoint.IndexDistalJoint, Handedness.Right);
        }
        */

        IHand ActiveHand = (handedness == Handedness.Right) ? RHand : LHand;

        if (ActiveHand.GetJointPose(HandJointId.HandIndexTip, out Pose indexTipPose))
        {
            IndexTip = indexTipPose;
        }
    }

    protected override void CalculatePointerVectors()
    {
        OriginPoint = IndexTip.position;
        OriginRotationVector = IndexTip.forward;
    }
}