using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TechniqueSwitcher : MonoBehaviour
{
    public TeleportationSystem TS;

    public GravityPointer WristPointer,ArmPointer,ControllerPointer;
    public string TechniqueActive = "ARM";
    public TextMeshPro Title;

    public void onArmButtonPressed()
    {
        DeactiveAll();
        TechniqueActive = "ARM";
        Title.text = "Active: " + TechniqueActive;

        TS.gPointer = ArmPointer;
    }

    public void onWristButtonPressed()
    {
        DeactiveAll();
        TechniqueActive = "WRIST";
        Title.text = "Active: " + TechniqueActive;

        //Debug.Log("onWristButtonPressed");
        TS.gPointer = WristPointer;
    }

    public void onControllerButtonPressed()
    {
        DeactiveAll();
        TechniqueActive = "CONTROLLER";
        Title.text = "Active: " + TechniqueActive;

        TS.gPointer = ControllerPointer;
    }

    private void DeactiveAll()
    {
        WristPointer.isActive = false;
        ArmPointer.isActive = false;
        ControllerPointer.isActive = false;
    }
}
