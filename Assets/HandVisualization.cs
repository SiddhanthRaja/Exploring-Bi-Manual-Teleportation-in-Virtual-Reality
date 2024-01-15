
using UnityEngine;

public class HandVisualization : MonoBehaviour
{
    public GameObject ArmViz;
    GameObject rHand;
    //private MixedRealityPose Wrist;
    GameObject tmp;

    public string TechniqueActive = "ARM";
    private bool TechniqueChanged = false;
    Color grey,green;

    public void SetTechnique(string tech)
    {
        TechniqueActive = tech;
        TechniqueChanged = true;
    }

    private void Start()
    {
        grey = Color.grey;
        green = Color.green;
        green.a = 0.4f;
    }

    GameObject currentArm;
    private GameObject rWrist;


    void Update()
    {
        if (!rHand)
        {
            rHand = GameObject.Find("R_Hand");
        }

        if (rHand && tmp != null)
        {
            if (TechniqueChanged)
            {
                if (TechniqueActive == "ARM")
                {
                    rHand.GetComponent<Renderer>().material.SetColor("_RimColor", grey);
                    foreach (Transform child in tmp.transform)
                    {
                        child.GetComponent<Renderer>().material.SetColor("_Color", green);
                    }

                }
                else if(TechniqueActive == "WRIST_ARM")
                {
                    rHand.GetComponent<Renderer>().material.SetColor("_RimColor", green);
                    foreach (Transform child in tmp.transform)
                    {
                        child.GetComponent<Renderer>().material.SetColor("_Color", green);
                    }
                }
                else if(TechniqueActive == "WRIST")
                {
                    rHand.GetComponent<Renderer>().material.SetColor("_RimColor", green);
                    foreach (Transform child in tmp.transform)
                    {
                        child.GetComponent<Renderer>().material.SetColor("_Color", grey);
                    }
                }
                TechniqueChanged = false;
            }
        }

        rWrist = GameObject.Find("R_Wrist");

        if (rWrist)
        {
            if (rHand && tmp == null)
            {
                tmp = GameObject.Instantiate(ArmViz);
                tmp.transform.position = new Vector3();
                tmp.SetActive(true);
                tmp.transform.position = GameObject.Find("R_Wrist").transform.position;

                TechniqueChanged = true;
            }

            if (tmp)
            {
                tmp.transform.position = rWrist.transform.position;

                tmp.transform.forward = rWrist.transform.forward;
                //Vector3 forward = rWrist.transform.forward;
                //forward = forward - CameraCache.Main.transform.position;

                //float angle = Vector3.Angle(CameraCache.Main.transform.up, rWrist.transform.up);
                //if (angle < 90.0f)
                //{
                //    tmp.transform.forward = -forward;
                //}
                //else
                //{
                //    tmp.transform.forward = forward;
                //}

                //Transform MiddleKnuckle = CoreServices.GetInputSystemDataProvider<IMixedRealityHandJointService>()
                //                    .RequestJointTransform(TrackedHandJoint.MiddleKnuckle, Handedness.Right);
                //float h = HandTrackGravityPointer.calculateCameraAdjustedArmHeight(MiddleKnuckle.position);
            }
        }
           
    }
}
