
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


using Oculus.Interaction.Input;
using Oculus.Interaction.Hands;
using Oculus.Interaction;
using UnityEditor;

public abstract class HandTrackGravityPointer : GravityPointer
{
    public bool personalizeArmHeight = false;
    public bool personalizeArmDistance = false;
    public bool personalizeWristAngle = false;

    public string curveArmDistance = "none";
    public string curveArmHeight = "none";
    public string curveWristAngle = "none";

    public float cameraToMaxWristDistance = 0.54f;
    public float cameraToMinWristDistance = 0.11f;

    public float maxArmHeight = 0.49f;
    public float minArmHeight = -0.4f;

    public float maxWristAngle = 280f;
    public float minWristAngle = 55f;


    public const float defaultCameraToMaxWristDistance = 0.5f;
    public const float defaultCameraToMinWristDistance = 0.15f;

    public const float defaultMaxWristAngle = 270f; 
    public const float defaultMinWristAngle = 70f;  


    public static float cameraHandOffset = -0.15f;


    public const float defaultMaxArmHeight = +0.6f;
    public const float defaultMinArmHeight = -0.5f;

    // protected IMixedRealityHandJointService handJointService;
    public bool isEnabledWristDistanceForce = false;

    public Pose WristPose { get; private set; }
    public Pose MiddleKnucklePose { get; private set; }

    public bool dontTouchK1 = false;

    public Handedness handedness = Handedness.Right;

    [SerializeField, Interface(typeof(IHand))]
    private MonoBehaviour _rHand;
    public IHand RHand;

    [SerializeField, Interface(typeof(IHand))]
    private MonoBehaviour _lHand;
    public IHand LHand;

    //public IHand ActiveHand;

    void Start()
    {
        Debug.Log("### HandTrackGravityPointer - Start");
    }
    public new virtual void Update()
    {
        base.Update();

        IHand ActiveHand = (handedness == Handedness.Right) ? RHand : LHand;

        if (ActiveHand != null)
        {
            if (ActiveHand.GetRootPose(out Pose handRootPose))
            {
                WristPose = handRootPose;
            }

            // consider using HandVisual > OculusHand > Index knuckle marker
            if (ActiveHand.GetJointPose(HandJointId.HandMiddle1, out Pose middleProximalPose))
            {
                MiddleKnucklePose = middleProximalPose;
                //Debug.Log("### SET MiddleKnucklePose = " + MiddleKnucklePose.position);
            }
        }
    }

    public new void init()
    {
        base.init();

        RHand = _rHand as IHand;
        LHand = _lHand as IHand;

        //Debug.Log("### HandTrackGravityPointer - Left Hand ACTIVE");
        //ActiveHand = _lHand as IHand;

        /*
        if (handedness == Handedness.Left)
        {
            Debug.Log("### HandTrackGravityPointer - Left Hand ACTIVE - Handedness = " + handedness);
            ActiveHand = _lHand as IHand;
        }
        else
        {
            Debug.Log("### HandTrackGravityPointer - Right Hand ACTIVE - Handedness = " + handedness);
            ActiveHand = _rHand as IHand;
        }
        */

        /*
        if (ActiveHand != null)
        {
            if (ActiveHand.GetRootPose(out Pose handRootPose))
            {
                WristPose = handRootPose;
            }

            // consider using HandVisual > OculusHand > Index knuckle marker
            if (ActiveHand.GetJointPose(HandJointId.HandMiddle1, out Pose middleProximalPose))
            {
                MiddleKnucklePose = middleProximalPose;
                Debug.Log("### SET MiddleKnucklePose = " + MiddleKnucklePose.position);
            }
        }
        */

        //StartCoroutine(updateWristPose());
        //StartCoroutine(updateMiddleKnucklePose());

        /*
        handJointService = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>();
        Wrist = handJointService.RequestJointTransform(TrackedHandJoint.Wrist, handedness);
        MiddleKnuckle = handJointService.RequestJointTransform(TrackedHandJoint.MiddleKnuckle, handedness);
        */

        cameraToMaxWristDistance = defaultCameraToMaxWristDistance;
        cameraToMinWristDistance = defaultCameraToMinWristDistance;
        maxArmHeight = defaultMaxArmHeight;
        minArmHeight = defaultMinArmHeight;
        maxWristAngle = defaultMaxWristAngle;
        minWristAngle = defaultMinWristAngle;

        //GetCurrentOriginPoint();
    }

    IEnumerator updateWristPose()
    {
        IHand ActiveHand = (handedness == Handedness.Right) ? RHand : LHand;

        if (ActiveHand != null)
        {
            Pose handRootPose;
            while (!ActiveHand.GetRootPose(out handRootPose))
            {
                yield return null;
            }

            WristPose = handRootPose;
            Debug.Log("### SET WristPose = " + WristPose.position);
        }
    }

    IEnumerator updateMiddleKnucklePose()
    {
        IHand ActiveHand = (handedness == Handedness.Right) ? RHand : LHand;

        if (ActiveHand != null)
        {
            Pose middleProximalPose;

            // consider using HandVisual > OculusHand > Index knuckle marker
            while (!ActiveHand.GetJointPose(HandJointId.HandMiddle1, out middleProximalPose))
            {
                yield return null;
            }

            MiddleKnucklePose = middleProximalPose;
            Debug.Log("### SET MiddleKnucklePose = " + MiddleKnucklePose.position);
        }
    }
    public Vector3 GetCurrentOriginPoint()
    {
        IHand ActiveHand = (handedness == Handedness.Right) ? RHand : LHand;

        Vector3 op = MiddleKnucklePose.position;

        /*
        Debug.Log("op = " + op);

        ActiveHand.GetJointPose(HandJointId.HandMiddleTip, out Pose middleTipPose);
        ActiveHand.GetJointPose(HandJointId.HandMiddle1, out Pose middle1Pose);
        ActiveHand.GetJointPose(HandJointId.HandMiddle2, out Pose middle2Pose);
        ActiveHand.GetJointPose(HandJointId.HandMiddle3, out Pose middle3Pose);

        Debug.Log("middleTipPose.position = " + middleTipPose.position);
        Debug.Log("middle1Pose.position = " + middle1Pose.position);
        Debug.Log("middle2Pose.position = " + middle2Pose.position);
        Debug.Log("middle3Pose.position = " + middle3Pose.position);
        */

        //if (personalizeArmHeight)
        //{
        //    op = op + MiddleKnuckle.forward * 0.15f;
        //}
        return op;
    }

    public override float getCurrentDistanceFactor()
    {
        float defaultDF = 12.0f;
        if (!isEnabledWristDistanceForce)
        {
            return defaultDF;
        }

        float dist = calculateArmLength(MiddleKnucklePose.position);
        if (personalizeArmDistance)
        {
            float ah = calculateCameraAdjustedArmHeight(MiddleKnucklePose.position);

            if (ah > (95 / 100) * maxArmHeight && dist > (95/100) * cameraToMaxWristDistance)
            {
                dist = Math.Min(dist + ah, cameraToMaxWristDistance);
            }

            float df = ConvertRange(dist, cameraToMaxWristDistance, cameraToMinWristDistance, distanceFactorMax, distanceFactorMin, curveArmDistance);

            return df;
        }

        return System.Math.Min(
            ConvertRange(dist, defaultCameraToMaxWristDistance, defaultCameraToMinWristDistance, distanceFactorMax, distanceFactorMin),
            defaultDF+1
            );
    }

    public float getWristAngle()
    {
        Quaternion tmp = Quaternion.LookRotation(OriginRotationVector, Vector3.forward);

        return tmp.eulerAngles.x;
    }

    public float getArmDistance()
    {
        return calculateArmLength(MiddleKnucklePose.position);
    }

    public float getArmHeight()
    {
        return calculateCameraAdjustedArmHeight(MiddleKnucklePose.position);
    }

    // ONLY WORKS WHEN USER is on the FLOOR
    private static float calculateArmHeight(Vector3 middleKnuckle)
    {
        return middleKnuckle.y;
    }

    // As user will not always be in same height in respect to ground
    // this trick has to be used to always adjust for users current height
    // THIS DOES NOT GIVE REAL ARM HEIGHT
    public static float calculateCameraAdjustedArmHeight(Vector3 middleKnuckle)
    {
        return (float)(middleKnuckle.y - (Camera.main.transform.position.y + cameraHandOffset));
    }

    public static float calculateArmLength(Vector3 middleKnuckle)
    {
        middleKnuckle.y = 0;
        Vector3 tmp = Camera.main.transform.position;
        tmp.y = 0;
        return Vector3.Distance(middleKnuckle, tmp);
    }

    // curveType = none, sigmoid, isigmoid, bell, ibell
    public float ConvertRange(float OldValue, float OldMax, float OldMin, float NewMax, float NewMin, string curveType = "none", bool senseCurve = false)
    {
        if (curveType != "none" && senseCurve)  //stopping all other curving
        {
            float curvable, curved;
            //if (!senseCurve && (curveType == "bell" || curveType == "ibell"))
            //{
            //    curvable = ConvertRangeFlat(OldValue, OldMax, OldMin, 160, 0);


            //    if(curvable <= 120 && curvable >= 40)
            //    {
            //        curved = ConvertRangeFlat(curvable, 40, 120, 100, 0);

            //        curved = Curvifier(curved, curveType);

            //        curved = ConvertRangeFlat(curved, 100, 0, 120, 40);
            //    }
            //    else
            //    {
            //        curved = curvable;
            //    }
                
            //    return ConvertRangeFlat(curved, 160, 0, NewMax, NewMin);
            //}

            curvable = ConvertRangeFlat(OldValue, OldMax, OldMin, 100, 0);

            if (senseCurve)
            {
                curved = SenseCurvifier(curvable, curveType);
            }
            else
            {
                curved = Curvifier(curvable, curveType);
            }

            return ConvertRangeFlat(curved, 100, 0, NewMax, NewMin);
        }
        else
        {
            return ConvertRangeFlat(OldValue, OldMax, OldMin, NewMax, NewMin);
        }

    }

    static double NthRoot(double A, int N)
    {
        return Math.Pow(A, 1.0 / N);
    }

    private float SenseCurvifier(float x, string curve)
    {
        double converted = x;

        float p = 5; //p
        float c1 = 0.1f; //c1

        float h = 50;
        float a = 0.04f;
        float k = 50;


        if (curve == "isigmoid")
        {
            converted = (100 / (1 + Math.Exp(p - c1 * x)));
        }
        else if (curve == "sigmoid")
        {
            converted = 100 - (100 / (1 + Math.Exp(p - c1 * x)));
        }
        else if (curve == "ibell")
        {
            converted = - a * Math.Pow((x - h), 2) + k;
        }
        else if (curve == "bell")
        {
            k = 0;
            converted = a * Math.Pow((x - h), 2) + k;
        }
        return (float)converted;
    }

    private float Curvifier(float x, string curve)
    {
        double converted = x;

        float p = 100; //p
        float c1 = 14; //c1
        float c2 = 50; // c2
        int b = 10; //b
        float a = 1.5f;

        float v,u,r;

        if (curve == "sigmoid")
        {
            converted = p * (1 / (1 + Math.Exp(-(1 / c1) * (x - c2)) ) );
        }
        else if (curve == "isigmoid")
        {
            converted = -(c1 * Math.Log((p / x) - 1) ) + c2;
        }
        else if (curve == "oldbell")
        {
            converted = NthRoot((x * Math.Pow(45, b + 1)) / a, b);
        }
        else if (curve == "bell")
        {
            v = -50;
            u = 149;
            r = 157;
            converted = v + Math.Sqrt(Math.Pow(r,2) - Math.Pow((x - u),2));
        }
        else if (curve == "ibell")
        {
            v = 149;
            u = -50;
            r = 157;
            converted = u + Math.Sqrt(Math.Pow(r, 2) - Math.Pow((x - v), 2));
        }
        else if (curve == "exp1")
        {
            if (x >= 40 && x <= 100)
            {
                p = 101; //p
                c1 = 7.6f; //c1
                c2 = 64.5f; // c2
                converted = -(c1 * Math.Log((p / x) - 1)) + c2;
            }
            else if(x >= 0 && x < 40 )
            {
                v = -130;
                u = 267;
                r = 297;
                converted = v + Math.Sqrt(Math.Pow(r, 2) - Math.Pow((x - u), 2));
            }
        }
        else if (curve == "exp2")
        {
            if (x > 30 && x <= 100)
            {
                p = 0.55f;
                float q = 45.0f;
                converted = p * x + q;
            }
            else if (x >= 0 && x <= 30)
            {
                v = -79;
                u = 244;
                r = 256;
                converted = v + Math.Sqrt(Math.Pow(r, 2) - Math.Pow((x - u), 2));
            }
        }

        return (float)converted;
    }

    public float ConvertRangeFlat(float OldValue, float OldMax, float OldMin, float NewMax, float NewMin)
    {
        float OldRange = (OldMax - OldMin);
        float NewRange = (NewMax - NewMin);

        if(OldValue >= OldMax)
        {
            return NewMax;
        }
        if (OldValue <= OldMin)
        {
            return NewMin;
        }


        float NewValue = NewMin;
        if (OldRange != 0)
        {
            NewValue = (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
        }

        return NewValue;
    }

}