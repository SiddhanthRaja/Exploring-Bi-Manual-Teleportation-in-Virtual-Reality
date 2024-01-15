using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeoutGesture: CustomGesture
{
    [SerializeField]
    GravityPointer gPointer;

    private Vector3 holdingAt;

    public override bool GestureDetected()
    {
        //DebugText.text = "TIMEOUT: sf: " + sightedFor + " af: " + activeFor;
        if (Vector3.Distance(gPointer.touchPosition, holdingAt) < 0.3f)
        {
            return true;
        }

  
        holdingAt = gPointer.touchPosition;
        
        return false;
    }
}
