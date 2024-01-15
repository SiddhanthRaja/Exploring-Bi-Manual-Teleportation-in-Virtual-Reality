using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GravityPointer : MonoBehaviour
{
    public bool isSmoothEnableActive = true;
    public bool isActive = false;

    public bool isGravityActive = true;

    protected LineRenderer lineRenderer;

    // Number of points on the line
    public int numPoints = 300;
    public float distanceFactorMin = 12.0f;
    public float distanceFactorMax = 12.0f;
    public GameObject PointingVectorProxy;
    protected bool PointingVectorAuto = true;

    public float jitterThreshold = 0.006f;
    public float collisionRadius = 0.01f;

    // distance between those points on the line
    public float timeBetweenPoints = 0.008f;

    // The physics layers that will cause the line to stop being drawn
    public LayerMask CollidableLayers;
    
    public Vector3 touchPosition;
    protected Vector3 oldPosition;

    protected Vector3 OriginPoint, OriginRotationVector;

    CircularBuffer.CircularBuffer<Vector3> originHistory = new CircularBuffer.CircularBuffer<Vector3>(4);
    CircularBuffer.CircularBuffer<Vector3> rotationHistory = new CircularBuffer.CircularBuffer<Vector3>(6);

    KalmanFilterVector3 kalmanV3Origin;
    KalmanFilterVector3 kalmanV3Rotation;

    public float k1q = 0.0001f;
    public float k2q = 0.00001f;
    public float k1r = 0.1f;
    public float k2r = 0.000001f;

    [HideInInspector]
    public float default_k1q = 0.0001f;
    [HideInInspector]
    public float default_k2q = 0.00001f;
    [HideInInspector]
    public float default_k1r = 0.1f;
    [HideInInspector]
    public float default_k2r = 0.00001f;


    public float kalmanQSense = 1;
    public float kalmanRSense = 1;

    public bool revertToKalman = false;
    public void resetKalmanFilters()
    {
        activeFilter = "mean";
        Debug.Log("RESETTING!!!");

        kalmanV3Origin = new KalmanFilterVector3(k1q, k1r);
        kalmanV3Rotation = new KalmanFilterVector3(k2q, k2r);

        originHistory = new CircularBuffer.CircularBuffer<Vector3>(4);
        rotationHistory = new CircularBuffer.CircularBuffer<Vector3>(6);

        activeFilter = "kalman";

    }

    protected void init()
    {
        //default_k1q = k1q;
        //default_k2q = k2q;
        //default_k1r = k1r;
        //default_k2r = k2r;

        lineRenderer = GetComponent<LineRenderer>();
        //Debug.Log("### Gravity Pointer - INIT - reset Kalman filters");
        resetKalmanFilters();
    }

    public virtual void Update()
    {
        if (isActive)
        {
            CalculatePointerVectors();

            DrawPointer();
        }
        else
        {
            List<Vector3> points = new List<Vector3>();
            lineRenderer.positionCount = 0;
        }

    }

    protected abstract void CalculatePointerVectors();

    public string activeFilter = "mean";
    private void DrawPointer()
    {
        //if (revertToKalman && activeFilter != "kalman")
        //{
        //    revertToKalman = false;
        //    activeFilter = "kalman";
        //}

        //Debug.Log("### GravityPointer - DRAW POINTER");

        if (isSmoothEnableActive)
        {
            originHistory.PushBack(OriginPoint);
            rotationHistory.PushBack(OriginRotationVector);

            if (activeFilter == "kalman")
            {
                OriginPoint = kalmanV3Origin.Update(originHistory.Back(), k1q, k1r); 
                OriginRotationVector = kalmanV3Rotation.Update(rotationHistory.Back(), k2q, k2r);
            }
            else
            {
                OriginPoint = GetMeanVector(originHistory.ToArray());
                OriginRotationVector = GetMeanVector(rotationHistory.ToArray());
            }

        }

        /*
        if (Vector3.Distance(oldPosition, OriginPoint) < jitterThreshold)
        {
            Debug.Log("### GravityPointer - JITTER = " + Vector3.Magnitude(oldPosition - OriginPoint));
            return;
        }
        */
        
        lineRenderer.positionCount = (int)numPoints;
        List<Vector3> points = new List<Vector3>();

        transform.position = OriginPoint;

        if (PointingVectorAuto)
        {
            PointingVectorProxy.transform.rotation = Quaternion.LookRotation(OriginRotationVector, Vector3.up);
        }

        Vector3 startingPosition = PointingVectorProxy.transform.position;

        //Debug.Log("### GravityPointer - PointingVectorProxy.transform.forward = " + PointingVectorProxy.transform.forward);
        //Debug.Log("### GravityPointer - getCurrentDistanceFactor = " + getCurrentDistanceFactor());

        Vector3 startingVelocity = PointingVectorProxy.transform.forward * getCurrentDistanceFactor();

        //Debug.Log("### GravityPointer - numPoints = " + numPoints);
         //Debug.Log("### GravityPointer - startingPosition = " + startingPosition);
        //Debug.Log("### GravityPointer - startingVelocity = " + startingVelocity);
        //Debug.Log("### GravityPointer - timeBetweenPoints = " + timeBetweenPoints);

        for (float t = 0; t < numPoints; t += timeBetweenPoints)
        {
            Vector3 newPoint = startingPosition;

            newPoint.x = startingPosition.x + t * startingVelocity.x;
            newPoint.z = startingPosition.z + t * startingVelocity.z;
            newPoint.y = startingPosition.y + t * startingVelocity.y + (isGravityActive ? (Physics.gravity.y / 2f) * t * t : 0);
            
            points.Add(newPoint);

            Collider[] colliders = Physics.OverlapSphere(newPoint, collisionRadius, CollidableLayers);
            if (colliders.Length > 0)
            {
                //Debug.Log("### GravityPointer - newPoint = " + newPoint);
                //Debug.Log("### GravityPointer - positionCount = " + points.Count);
                touchPosition = newPoint;
                lineRenderer.positionCount = points.Count;
                break;
            }

            foreach(Collider collider in colliders)
            {
                if(collider.gameObject.name == "CheckpointPlate")
                {
                    Debug.Log("### GravityPointer - CheckpointPlate");
                    collider.gameObject.GetComponent<Renderer>().material.color = new Color(0, 1, 0, 0.8f);
                }
            }
        }

        //Debug.Log("### GravityPointer - No of points = " + points.Count);
        lineRenderer.SetPositions(points.ToArray());

        oldPosition = OriginPoint;
    }

    public virtual float getCurrentDistanceFactor()
    {
        Debug.Log("base df method here");
        return 12.0f;
    }

    public void ActivatePointer()
    {
        isActive = true;
    }

    public void DeactivatePointer()
    {
        isActive = false;
    }

    public void SetColors(Color startColor, Color endColor)
    {
        lineRenderer.startColor = startColor;
        lineRenderer.endColor = endColor;
    }

    private Vector3 get(Vector3[] positions)
    {
        if (positions.Length == 0)
            return Vector3.zero;
        float x = 0f;
        float y = 0f;
        float z = 0f;
        foreach (Vector3 pos in positions)
        {
            x += pos.x;
            y += pos.y;
            z += pos.z;
        }
        return new Vector3(x / positions.Length, y / positions.Length, z / positions.Length);
    }


    private Vector3 GetMeanVector(Vector3[] positions)
    {
        if (positions.Length == 0)
            return Vector3.zero;
        float x = 0f;
        float y = 0f;
        float z = 0f;
        foreach (Vector3 pos in positions)
        {
            x += pos.x;
            y += pos.y;
            z += pos.z;
        }
        return new Vector3(x / positions.Length, y / positions.Length, z / positions.Length);
    }
}