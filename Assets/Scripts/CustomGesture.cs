
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

using Oculus.Interaction.Input;
using Oculus.Interaction.Hands;
using Oculus.Interaction;
using UnityEditor;

public abstract class CustomGesture : MonoBehaviour
{
    public bool isActive = false;
    
    public bool isNewActivation = false;
    public float activeFor = 0.0f;
    protected float sightedFor = 0.0f;

    [SerializeField]
    private float sightedForTimeout = 0.15f;

    public Handedness handedness = Handedness.Right;

    public TextMeshPro DebugText;

    [SerializeField, Interface(typeof(IHand))]
    private MonoBehaviour _rHand;
    public IHand RHand;

    [SerializeField, Interface(typeof(IHand))]
    private MonoBehaviour _lHand;
    public IHand LHand;

    //public IHand ActiveHand;

    private void Start()
    {
        RHand = _rHand as IHand;
        LHand = _lHand as IHand;

        //Debug.Log("### CustomGesture -  Left Hand ACTIVE");
        //ActiveHand = _lHand as IHand;
       
        /*
        if (handedness == Handedness.Left)
        {
            Debug.Log("### CustomGesture - Left Hand ACTIVE - Handedness = " + handedness);
            ActiveHand = _lHand as IHand;
        }
        else
        {
            Debug.Log("### CustomGesture - Right Hand ACTIVE - Handedness = " + handedness);
            ActiveHand = _rHand as IHand;
        }
        */
    }

    public void reset()
    {
        isActive = false;
        isNewActivation = false;
        activeFor = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (GestureDetected())
        {
            if(isActive)
            {
                activeFor += Time.deltaTime;
                isNewActivation = false;
            }
            else
            {
                sightedFor += Time.deltaTime;
                if (sightedFor > sightedForTimeout)
                {
                    sightedFor = 0;
                    activeFor = 0.0f;
                    isActive = true;
                    isNewActivation = true;
                }
            }
        }
        else
        {
            sightedFor = 0;
            isActive = false;
            isNewActivation = false;
        }
    }

    public void onGestureDetection()
    {
        if (isActive)
        {
            activeFor += Time.deltaTime;
            isNewActivation = false;
        }
        else
        {
            sightedFor += Time.deltaTime;
            if (sightedFor > sightedForTimeout)
            {
                sightedFor = 0;
                activeFor = 0.0f;
                isActive = true;
                isNewActivation = true;
            }
        }
    }

    abstract public bool GestureDetected();
}
