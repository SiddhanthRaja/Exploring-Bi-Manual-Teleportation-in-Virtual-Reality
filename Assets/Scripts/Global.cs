 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

public class Global : MonoBehaviour
{
    //[SerializeField]
    //GameObject DebugBoard;

    [SerializeField]
    GameObject FistClosedGesture;
    [SerializeField]
    GameObject IndexThumbOpenRestClosed;

    private FistClose FistCloseScript;
    private IndexThumbOpenRestClosed IndexThumbOpenRestClosedScript;


    public string appendToDebugLog = "";

    // Start is called before the first frame update
    void Start()
    {
        //PointerUtils.SetHandRayPointerBehavior(PointerBehavior.AlwaysOff, Microsoft.MixedReality.Toolkit.Utilities.Handedness.Right);
       
        // MeshRenderer 
        /*
        MixedRealityHandTrackingProfile handProfile = Microsoft.MixedReality.Toolkit.CoreServices.InputSystem.InputSystemProfile.HandTrackingProfile;
        handProfile.EnableHandMeshVisualization = true;

        Microsoft.MixedReality.Toolkit.CoreServices.DiagnosticsSystem.ShowProfiler = false;
        */

        FistCloseScript = FistClosedGesture.GetComponent<FistClose>();
        IndexThumbOpenRestClosedScript = IndexThumbOpenRestClosed.GetComponent<IndexThumbOpenRestClosed>();
        //Microsoft.MixedReality.Toolkit.Teleport.TeleportPointer

    }

    // Update is called once per frame
    void Update()
    {

        string log = "FC isActive: " + FistCloseScript.isActive + "\n"
            + "FC isNewActivation: " + FistCloseScript.isNewActivation + "\n"
            + "FC Duration: " + FistCloseScript.activeFor + "\n"
            +
            "IC isActive: " + IndexThumbOpenRestClosedScript.isActive + "\n"
            + "IC isNewActivation: " + IndexThumbOpenRestClosedScript.isNewActivation + "\n"
            + "IC Duration: " + IndexThumbOpenRestClosedScript.activeFor + "\n";

        //DebugBoard.GetComponent<TextMeshPro>().text = log + appendToDebugLog;

    }



}

