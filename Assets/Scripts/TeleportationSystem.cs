using PointSystem;
using UnityEditor;
using UnityEngine;

public class TeleportationSystem : MonoBehaviour
{
    [SerializeField]
    public CustomGesture InitializationGesture, ConfirmationGesture, CancellationGesture;

    [SerializeField]
    public GravityPointer gPointer;

    public AudioSource wrong;



    private Vector3 targetPosition = Vector3.zero;
    private Vector3 targetRotation = Vector3.zero;
    public GameObject mySphere;
    public GameObject CheckpointPlate;

    public LayerMask TeleportableLayers;

    public bool IsActive = false;

    public int studyNumber = 1;
    public string technique = "ARM";
    public bool cancellationActive = true;
    public float distanceFromCenter, distanceFromCenterWithSpike;


    public CustomGesture ThumbStickForward, ThumbStickNotForward, SecIndexTrigger;

    CircularBuffer.CircularBuffer<Vector3> positionHistory = new CircularBuffer.CircularBuffer<Vector3>(300);


    Color redC = new Color(1, 0, 0, 0.8f);
    Color greenC = new Color(0, 1, 0, 0.8f);

    // GameObject defining the player space i.e. need to add OVRCameraRig into it
    [SerializeField]
    private GameObject _playerSpace;

    private void Start()
    {
    }
    void Update()
    {
        if (IsActive)
        {
            if ((ConfirmationGesture.isActive && ConfirmationGesture.isNewActivation) || ControllerConfirmation() || (Input.GetButtonDown("Fire3"))) //
            {
                Debug.Log("TELEPORT!!!");
                PerformTeleport();
            }
            else if ((cancellationActive && CancellationGesture.isActive && CancellationGesture.isNewActivation) || ControllerCancellation())
            {
                //disable teleportation
                Debug.Log("DON'T TELEPORT!!!");
                IsActive = false;
            }


            SetLineColor();
        }
        else
        {
            /*
            if ((InitializationGesture.isActive && InitializationGesture.isNewActivation) || ControllerActivation()) {

                Debug.Log("### Detected InitializationGesture");
                IsActive = true;
            }
            */

            if (ControllerActivation())
            {
                //Debug.Log("### Teleportation System - ControllerActivation()");
                IsActive = true;
            }

            if (InitializationGesture.isActive)
            {
                //Debug.Log("### Teleportation System - InitializationGesture.isActive");

                if (InitializationGesture.isNewActivation)
                {
                    //Debug.Log("### Teleportation System - InitializationGesture.isNewActivation");

                    IsActive = true;
                }
            }
        }

        gPointer.isActive = IsActive;

        positionHistory.PushBack(gPointer.touchPosition);

    }

    private bool ControllerActivation()
    {
        return technique == "CONTROLLER" && ThumbStickForward.isActive && ThumbStickForward.isNewActivation;
    }

    private bool ControllerCancellation()
    {
        return technique == "CONTROLLER" && ThumbStickNotForward.isActive && ThumbStickNotForward.isNewActivation;
    }

    private bool ControllerConfirmation()
    {
        return technique == "CONTROLLER" && SecIndexTrigger.isActive && SecIndexTrigger.isNewActivation;
    }

    private void SetLineColor()
    {
        if (!isAllowedToTeleport(gPointer.touchPosition))
        {
            gPointer.SetColors(Color.red, Color.red);

            if (CheckpointPlate != null)
            {
                CheckpointPlate.GetComponent<Renderer>().material.SetColor("_Color", redC);
            }
        }
        else
        {
            if (CheckpointPlate != null)
            {
                CheckpointPlate.GetComponent<Renderer>().material.SetColor("_Color", greenC);
            }
            gPointer.SetColors(Color.green,Color.green);
        }
    }

    private Vector3 calculateLastStablePosition()
    {

        //debug 
        //GameObject lastSphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //GameObject calcCube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        //lastSphere.GetComponent<Collider>().enabled = false;
        //calcCube.GetComponent<Collider>().enabled = false;

        //lastSphere.transform.position = positionHistory.Back();
        //end

        int sampleSize = 30; //5
        float distanceThreshold = 0.5f; //0.3f

        for (int i = positionHistory.Size - 1; i > 0; i -= sampleSize)
        {
            float d = Vector3.Distance(positionHistory[i - sampleSize], positionHistory[i]);
            if(d < distanceThreshold)
            {
                //calcCube.transform.position = positionHistory[i]; //debug
                return positionHistory[i];
            }
               
        }

        return positionHistory.Back();
    }

    private bool isAllowedToTeleport(Vector3 position)
    {
        Collider[] colliders = Physics.OverlapSphere(position, gPointer.collisionRadius, TeleportableLayers);
      
        if (colliders.Length > 0)
        {
            foreach (Collider c in colliders)
            {
                if (c.gameObject.name == "CheckpointPlate")
                {
                    CheckpointPlate = c.gameObject;
                    //CheckpointPlate.GetComponent<Renderer>().material.SetColor("_Color", new Color(0, 1, 0, 0.8f));
                    break;
                }
            }
            return true;
        }

        return false;
    }

    private void PerformTeleport()
    {
        //ConfirmationGesture.reset();
        targetRotation = Vector3.zero;

        Vector3 calcTouchPosition = calculateLastStablePosition();

        targetRotation.y = calcTouchPosition.y;
        targetPosition = calcTouchPosition;

        if (!isAllowedToTeleport(calcTouchPosition))
        {
            Debug.Log("DENIED FROM TELEPORT");
            distanceFromCenter = Vector3.Distance(CheckpointPlate.transform.position, calcTouchPosition);
            if (distanceFromCenter < 2.5)
            {
                GameObject.Find("GameManager").GetComponent<GameManager>().mistakeCount++;
            }
            
            wrong.Play();
            //disable skip position
            //checkPoint.GetComponent<PointSystem.GamePoint>().skipPoint();
            return;
        }

        //targetRotation.y = calcTouchPosition.y;
        //targetPosition = calcTouchPosition;
  
        if (CheckpointPlate.GetComponent<Collider>().bounds.Contains(calcTouchPosition))
        {
             distanceFromCenter = Vector3.Distance(CheckpointPlate.transform.position, calcTouchPosition);
             distanceFromCenterWithSpike = Vector3.Distance(CheckpointPlate.transform.position, gPointer.touchPosition);

            //checkPoint.GetComponent<PointSystem.GamePoint>().manuallyTrigger(distance, distanceWithSpike);
        }


        if (studyNumber == 2)
        {
            Debug.Log("TELEPORTING TO NEW LOC");
            TeleportToPosition(new Vector3(
                CheckpointPlate.transform.position.x, 
                CheckpointPlate.transform.position.y - 0.4f,
                CheckpointPlate.transform.position.z), gPointer.transform.rotation);
        }
        
    }

    private void TeleportToPosition(Vector3 targetPosition, Quaternion rotation)
    { 
        float height = targetPosition.y;

        //Debug.Log(CameraCache.Main.transform.position - MixedRealityPlayspace.Position);
        // targetPosition -= Camera.main.transform.position - MixedRealityPlayspace.Position;

        targetPosition -= Camera.main.transform.position - _playerSpace.transform.position;
        targetPosition.y = height;

        //MixedRealityPlayspace.Position = targetPosition;
        //MixedRealityPlayspace.Rotation = rotation;

        _playerSpace.transform.position = targetPosition;
        _playerSpace.transform.rotation = rotation;

        Camera.main.transform.rotation = rotation;
    }

    public void resetPosition()
    {
        TeleportToPosition(new Vector3(0, 0, 0), new Quaternion());
    }
}
