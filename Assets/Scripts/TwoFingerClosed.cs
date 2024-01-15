using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Oculus.Interaction.Input;

public class TwoFingerClosed: CustomGesture
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
        if (handedness == Handedness.Left && leftGestureActive)
        {
            leftGestureActive = false;
            Debug.Log("leftGestureActive - Handedness = " + handedness);

            return true;
        }
        else if (handedness == Handedness.Right && rightGestureActive)
        {
            rightGestureActive = false;
            Debug.Log("rightGestureActive - Handedness = " + handedness);

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

    /*
    public void unsetLeft()
    {
        leftGestureActive = false;
    }
    public void unsetRight()
    {
        rightGestureActive = false;
    }
    */
}
