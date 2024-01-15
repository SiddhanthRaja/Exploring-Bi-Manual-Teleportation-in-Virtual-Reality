using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbStickForward: CustomGesture
{
    public override bool GestureDetected()
    {
        return OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp);
    }
}
