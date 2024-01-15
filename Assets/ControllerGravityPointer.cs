
using UnityEngine;

public class ControllerGravityPointer : GravityPointer
{

    public float offset = 0;
    public Transform MRPlayspace;

    void Start()
    {
        base.init();
        
    }

    protected override void CalculatePointerVectors()
    {
        PointingVectorProxy.transform.rotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

       
            OriginPoint = MRPlayspace.TransformPoint(OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch));

            OriginPoint.z += offset;

            OriginRotationVector = PointingVectorProxy.transform.forward;

    }
}
