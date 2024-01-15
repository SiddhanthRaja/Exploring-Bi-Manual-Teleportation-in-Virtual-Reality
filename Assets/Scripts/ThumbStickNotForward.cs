using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThumbStickNotForward : CustomGesture
{
    public override bool GestureDetected()
    {
        return !OVRInput.Get(OVRInput.Button.SecondaryThumbstickUp);
    }
}
