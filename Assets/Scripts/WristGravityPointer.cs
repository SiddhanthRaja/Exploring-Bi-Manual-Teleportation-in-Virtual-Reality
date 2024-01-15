
using Oculus.Interaction.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WristGravityPointer : HandTrackGravityPointer
{
    public float angleMultiplier = 5;

    public float maxDF;
    public override void Update()
    {
        base.Update();

    }

    void Start()
    {
        //Debug.Log("### WristGravityPointer - INIT BASE");
        base.init();
        maxDF = distanceFactorMax;

    }

    protected override void CalculatePointerVectors()
    {
        OriginPoint = GetCurrentOriginPoint();

        //Debug.Log("### CalculatePointerVectors - MiddleKnucklePose.position = " + MiddleKnucklePose.position);
        //Debug.Log("### CalculatePointerVectors - WristPose.position = " + WristPose.position);
        OriginRotationVector = MiddleKnucklePose.position - WristPose.position;

        /*
        ActiveHand.GetJointPoseFromWrist(HandJointId.HandMiddle1, out Pose MiddleKnuckleFromWristPose);
        ActiveHand.GetPalmPoseLocal(out Pose PalmPose);

        Debug.Log("MiddleKnucklePose.position - WristPose.position = " + (MiddleKnucklePose.position - WristPose.position));
        Debug.Log("MiddleKnuckleFromWristPose = " + (MiddleKnuckleFromWristPose.position));
        Debug.Log("PalmPose = " + (PalmPose.position));
        */
        //OriginRotationVector = Wrist.forward;

        PointingVectorAuto = false;
        Quaternion tmp = Quaternion.LookRotation(OriginRotationVector, Vector3.forward);

        float angle = tmp.eulerAngles.x;

        if (personalizeWristAngle)
        {
            float ogAngle = angle;
            angle = FromMagicAngle(
                ConvertRange(ToMagicAngle(angle), ToMagicAngle(minWristAngle), ToMagicAngle(maxWristAngle), 
                    ToMagicAngle(defaultMinWristAngle), ToMagicAngle(defaultMaxWristAngle),
                    curveWristAngle)
                    );

            float k1modL = ConvertRange(getCurrentDistanceFactor(), distanceFactorMax, distanceFactorMin, 90, 0.0001f, curveArmDistance, true);
            float k1modH = ConvertRange(calculateCameraAdjustedArmHeight(MiddleKnucklePose.position), maxArmHeight, minArmHeight, 90, 0.0001f, curveArmHeight, true);
            float k1modA = ConvertRange(ToMagicAngle(angle), ToMagicAngle(minWristAngle), ToMagicAngle(maxWristAngle), 90, 0.0001f, curveWristAngle, true);

            //Debug.Log("k1modL " + k1modL + ", k1modH " + k1modH + ", k1modA" + k1modA);

            if (curveArmDistance == "none")
            {
                k1modL = 0;
            }
            if (curveArmHeight == "none")
            {
                k1modH = 0;
            }
            if (curveWristAngle == "none")
            {
                k1modA = 0;
            }

            if (k1modL + k1modH + k1modA != 0)
            {
                k1r = default_k1r * System.Math.Max(System.Math.Max(k1modL, k1modH), k1modA);
            }

            float k2mod = ConvertRange(ToMagicAngle(angle), ToMagicAngle(minWristAngle), ToMagicAngle(maxWristAngle), 90.999f, 0.000001f, curveWristAngle, true);
            //Debug.Log(k2mod);
            if (curveWristAngle != "none")
            {
                k2r = default_k2r * k2mod;
            }
            

            // compensate for other factors
            if (angle < 310 && angle > 180)
            {
                distanceFactorMax = 15;
            }
            else
            {
                distanceFactorMax = maxDF;
            }

        }

        //Debug.Log("F " + angle);

        PointingVectorProxy.transform.rotation = Quaternion.Euler(angle, tmp.eulerAngles.y, tmp.eulerAngles.z);
        //Debug.Log("### WristGravityPointer - Angle = " + PointingVectorProxy.transform.rotation);
    }

    public static float ToMagicAngle(float angle)
    {
        if (angle < 80 && angle >= 0)
        {
            angle = 360 + angle;
        }

        return angle;
    }

    public static float FromMagicAngle(float magicAngle)
    {
        if(magicAngle > 360)
        {
            magicAngle -= 360;
        }
        return magicAngle;
    }

}