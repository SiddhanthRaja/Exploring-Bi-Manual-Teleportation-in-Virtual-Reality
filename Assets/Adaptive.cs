using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


using Oculus.Interaction.Input;
using Oculus.Interaction.Hands;
using Oculus.Interaction;
using UnityEditor;


public class Adaptive : MonoBehaviour
{
    // protected IMixedRealityHandJointService handJointService;
    private Pose Wrist, MiddleKnuckle, indexTip, indexTipL, MFtip;
    // public TextMeshPro AdaptiveText;

    [SerializeField]
    public CustomGesture ActionGesture;

    public string step = "arm_hmax";
    public float armHMax, armHMin, armLMax, armLMin, wristMax, wristMin;

    public TeleportationSystem teleportationSystem;

    public float gestureTimer = 5.0f;

    public int addExtraPercentage = 30;
    internal int addExtraPercentageWrist = 30;

    HandTrackGravityPointer htp;

    void Start()
    {
        ActionGesture.handedness = Handedness.Right;

        /*
        handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        Wrist = handJointService.RequestJointTransform(TrackedHandJoint.Wrist, Handedness.Right);
        MiddleKnuckle = handJointService.RequestJointTransform(TrackedHandJoint.MiddleKnuckle, Handedness.Right);

        indexTip = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Right);

        indexTip = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Right);
        indexTipL = handJointService.RequestJointTransform(TrackedHandJoint.IndexTip, Handedness.Left);

        MFtip = handJointService.RequestJointTransform(TrackedHandJoint.MiddleTip, Handedness.Right);
        */

        if (teleportationSystem.gPointer is HandTrackGravityPointer)
        {
            htp = (HandTrackGravityPointer)teleportationSystem.gPointer;
        }
    }


void Update()
    {
        IHand ActiveHand = (htp.handedness == Handedness.Right) ? htp.RHand : htp.LHand;

        if (ActiveHand != null)
        {
            if (ActiveHand.GetRootPose(out Pose handRootPose))
            {
                Wrist = handRootPose;
            }

            if (ActiveHand.GetJointPose(HandJointId.HandMiddle1, out Pose middleProximalPose))
            {
                MiddleKnuckle = middleProximalPose;
                //Debug.Log("### Adaptive - SET MiddleKnuckle = " + MiddleKnuckle.position);

            }

            if (ActiveHand.GetJointPose(HandJointId.HandIndexTip, out Pose indexTipPose))
            {
                indexTip = indexTipPose;
            }

            if (htp.LHand.GetJointPose(HandJointId.HandIndexTip, out Pose indexTipLPose))
            {
                indexTipL = indexTipLPose;
            }

            if (ActiveHand.GetJointPose(HandJointId.HandMiddleTip, out Pose middleTipPose))
            {
                MFtip = middleTipPose;
            }
        }

        PersonalDataCollectionProcess();

        if (step == "done")
        {
            if (teleportationSystem.gPointer is HandTrackGravityPointer)
            {
                htp = (HandTrackGravityPointer)teleportationSystem.gPointer;

                htp.cameraToMaxWristDistance = armLMax + addExtra(HandTrackGravityPointer.defaultCameraToMaxWristDistance, armLMax); ;
                htp.cameraToMinWristDistance = armLMin + addExtra(HandTrackGravityPointer.defaultCameraToMinWristDistance, armLMin); ;

                htp.maxArmHeight = armHMax + addExtra(HandTrackGravityPointer.defaultMaxArmHeight, armHMax);
                htp.minArmHeight = armHMin + addExtra(HandTrackGravityPointer.defaultMinArmHeight, armHMin); ;


                htp.maxWristAngle = wristMax + addExtra(HandTrackGravityPointer.defaultMaxWristAngle, wristMax, true);
                htp.minWristAngle = wristMin + addExtra(HandTrackGravityPointer.defaultMinWristAngle, wristMin, true);
            }
        }

    }
    void PersonalDataCollectionProcess()
    {
        gestureTimer += Time.deltaTime;

        Quaternion tmp = Quaternion.LookRotation(MiddleKnuckle.position - Wrist.position, Vector3.forward);

        if (gestureTimer < 2.5)
        {
            //AdaptiveText.text = "Recorded";
            return;
        }

        if (step == "arm_hmax")
        {
            //AdaptiveText.text = "Please raise your ARM to the MAXIMUM comfortable height";

            if (GoNextStep())
            {
                gestureTimer = 0;
                armHMax = HandTrackGravityPointer.calculateCameraAdjustedArmHeight(MiddleKnuckle.position);

                //armHMax += addExtra(HandTrackGravityPointer.defaultMaxArmHeight, armHMax);

                step = "arm_hmin";
            }
        }
        else if (step == "arm_hmin")
        {
             //AdaptiveText.text = "Please raise your ARM to the MINIMUM comfortable height";

            if (GoNextStep())
            {
                gestureTimer = 0;
                armHMin = HandTrackGravityPointer.calculateCameraAdjustedArmHeight(MiddleKnuckle.position);

                //armHMin += addExtra(HandTrackGravityPointer.defaultMinArmHeight, armHMin);

                step = "arm_lmax";
            }
        }
        else if (step == "arm_lmax")
        {
            //AdaptiveText.text = "Please EXTEND your ARM to the MAXIMUM comfortable distance";

            if (GoNextStep())
            {
                gestureTimer = 0;
                armLMax = HandTrackGravityPointer.calculateArmLength(MiddleKnuckle.position);

                //armLMax += addExtra(HandTrackGravityPointer.defaultCameraToMaxWristDistance, armLMax);

                step = "arm_lmin";
            }
        }
        else if (step == "arm_lmin")
        {
            //AdaptiveText.text = "Please bring your ARM to the MINIMUM comfortable distance";

            if (GoNextStep())
            {
                gestureTimer = 0;
                armLMin = HandTrackGravityPointer.calculateArmLength(MiddleKnuckle.position);

                //armLMin += addExtra(HandTrackGravityPointer.defaultCameraToMinWristDistance, armLMin);

                step = "wrist_max";
            }
        }
        else if (step == "wrist_max")
        {
            //AdaptiveText.text = "Please angle your wrist to maximum comfortable angle";

            if (GoNextStep())
            {
                gestureTimer = 0;
                wristMax = tmp.eulerAngles.x;


                //wristMax += addExtra(HandTrackGravityPointer.defaultMaxWristAngle, wristMax);

                step = "wrist_min";
            }
        }
        else if (step == "wrist_min")
        {
            //AdaptiveText.text = "Please angle your wrist to minimum comfortable angle";

            if (GoNextStep())
            {
                gestureTimer = 0;
                wristMin = tmp.eulerAngles.x;

                //wristMin += addExtra(HandTrackGravityPointer.defaultMinWristAngle, wristMin);

                step = "done";
            }
        }
        else if (step == "done")
        {
            /*
            AdaptiveText.text = "L: " + armLMin + "," + armLMax
             + "\nH: " + armHMax + "," + armHMin
            + "\nW: " + wristMin + "," + wristMax;
            */
        }

        float a = Quaternion.LookRotation(MiddleKnuckle.position - Wrist.position, Vector3.forward).eulerAngles.x;
        //AdaptiveText.text += "\nA:" + WristGravityPointer.ToMagicAngle(a)
        //+"A: " + a +"\n L:" + HandTrackGravityPointer.calculateArmLength(MiddleKnuckle.position);

        //AdaptiveText.text += "\nA:" + WristGravityPointer.ToMagicAngle(a) + "\n --- " + htp.k1r + "\n" + htp.k2r;
    }

    public bool recordPersonalization = false;
    bool GoNextStep()
    {
        if(ActionGesture.isActive && ActionGesture.isNewActivation)
        {
            return true;
        }
        return recordPersonalization;
    }


    public float addExtra(float stdVal, float currentVal, bool wrist = false)
    {
        float diff = stdVal - currentVal;
        float p = addExtraPercentage;
        if (wrist)
        {
            p = addExtraPercentageWrist;
        }
        return diff * (p / 100.0f);
    }
}
