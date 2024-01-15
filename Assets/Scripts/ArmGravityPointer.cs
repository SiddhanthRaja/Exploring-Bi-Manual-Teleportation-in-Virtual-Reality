
using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmGravityPointer : HandTrackGravityPointer
{
    //public TMPro.TextMeshPro AdaptiveText;
    public float angleMultiplier = 90;
    private Vector3 indexFinger;
        // Transform Wrist, MiddleKnuckle;
    // Pose WristPose, MiddleKnucklePose;
    public bool isEnabledMultiplerExponentialGrowth = false;

    void Start()
    {
        //Debug.Log("### ArmGravityPointer - INIT BASE");

        base.init();

        /*
        if (handJointService != null)
        {
            Wrist = handJointService.RequestJointTransform(TrackedHandJoint.Wrist, Handedness.Right);
            MiddleKnuckle = handJointService.RequestJointTransform(TrackedHandJoint.MiddleKnuckle, Handedness.Right);
        }
        */

        // Might want to update Wrist and Knuckle before using it
        /*
        if (ActiveHand.GetRootPose(out Pose handRootPose))
        {
            WristPose = handRootPose;
        }

        if (ActiveHand.GetJointPose(HandJointId.HandMiddle1, out Pose middleProximalPose))
        {
            MiddleKnucklePose = middleProximalPose;
        }
        */

    }


    protected override void CalculatePointerVectors()
    {
        OriginPoint = GetCurrentOriginPoint();

        float offsetHandHeight = calculateCameraAdjustedArmHeight(MiddleKnucklePose.position);
        if (personalizeArmHeight)
        {
            offsetHandHeight = ConvertRange(offsetHandHeight, maxArmHeight, minArmHeight, defaultMaxArmHeight, defaultMinArmHeight, curveArmHeight);
        }
        //Debug.Log(offsetHandHeight);

        OriginRotationVector = WristPose.forward;

        PointingVectorAuto = false;
        
        Quaternion tmp = Quaternion.LookRotation(OriginRotationVector, Vector3.forward);

        // exponential growth


        if (isEnabledMultiplerExponentialGrowth)
        {
            offsetHandHeight = offsetHandHeight * 1000;
            offsetHandHeight = offsetHandHeight * System.Math.Abs(offsetHandHeight * 1000);
            offsetHandHeight = offsetHandHeight / 50000000;
        }

        float angle = -(angleMultiplier * offsetHandHeight);
        if(angle >= 71)
        {
            angle = 70;
        }
        if(angle < -90)
        {
            angle = -89;
        }

        //AdaptiveText.text = "A: " + angle;

        tmp = Quaternion.Euler(angle, tmp.eulerAngles.y, 0);
        PointingVectorProxy.transform.rotation = tmp;

        
    }
}