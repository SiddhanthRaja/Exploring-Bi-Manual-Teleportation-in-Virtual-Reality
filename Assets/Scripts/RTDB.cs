using ExtensionMethods;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using Oculus.Interaction.Input;
using PointSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.JsonUtility;

public class RTDB : MonoBehaviour
{
    DatabaseReference RealtimeDB;
    // public DatabaseReference dbReference;
    
    [SerializeField]
    TechniqueSwitcher ts;

    [SerializeField]
    public string Technique, Activity, confirmationType, pointerHand;

    public string gestureSet = "GS1";

    [SerializeField]
    public int ParticipantId;
    bool resetLocation,restart;


    public ArmGravityPointer ArmPointer;
    public GravityPointer WristPointer, ControllerPointer;

    // distance: set, random
    // height: set, disabled

    public List<float> distanceSetCloud, heightSetCloud, targetSizeCloud;

    public int studyNum = 1;
    public int repeatTimes = 5;
    public int ARM_Angle_Multiplier = 160;
    public float HandJitterThreshold = 0.0035f;

    public bool hudEnabled = true;
    public string pointerType = "S";  // S / G
    //public float targetSize;
    public bool cancellationActive = true;

    public TeleportationSystem TeleSystem;
    public GameManager GM;
    public HandVisualization HV;

    public Adaptive adaptive;

    public static string[] dataLogArr;

    public CustomGesture FistCloseGesture, IndexThumbOpenRestClosed, TwoFingerClosed,
        FistCloseThumbOutGesture, PalmReverse, PalmReverseFistClosed, TimeOut;

    static String curveCombo = "C0";

    public string bimanualCombo = "RHRG";

    /*
    public IEnumerator LogDetails(string key)
    {
        Debug.Log("Entered LogDetails");
        string logStr = "helloworld";
        var DBtask = dbReference.Child("LogInfoforDavid").Child("" + ParticipantId).SetValueAsync(logStr);
        yield return new WaitUntil(predicate: () => DBtask.IsCompleted);
    }
    */

    void Start()
    {
        RealtimeDB = FirebaseDatabase.DefaultInstance.RootReference.Child("State");
        RealtimeDB.ValueChanged += HandleStateChanged;
        StartCoroutine(GetCurrentState());
    }

    void HandleStateChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError == null)
        {
            StartCoroutine(GetCurrentState());
        }
    }
    System.Collections.IEnumerator GetCurrentState()
    {
        var DBTask = RealtimeDB.GetValueAsync();

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

        DataSnapshot snapshot = DBTask.Result;
        if (snapshot.Exists)
        {
            restart = (bool)snapshot.Child("restart").Value;
            if (restart) {
                TeleSystem.resetPosition();
                GM.restart();

                restart = false;
                resetLocation = false;

                StartCoroutine(UpdateState());
            }

            Technique = snapshot.Child("technique").Value.ToString();
            Activity = snapshot.Child("activity").Value.ToString();
            ParticipantId = int.Parse(snapshot.Child("participant_id").Value.ToString());
            gestureSet = snapshot.Child("gestureSet").Value.ToString();
            pointerType = snapshot.Child("pointerType").Value.ToString();

            confirmationType = snapshot.Child("confirmationType").Value.ToString();
            pointerHand = snapshot.Child("pointerHand").Value.ToString();



            //targetSize = float.Parse(snapshot.Child("IVS").Child("targetSize").Value.ToString());

            //GM.targetSize = targetSize;


            studyNum = int.Parse(snapshot.Child("IVS").Child("studyNum").Value.ToString());
            repeatTimes = int.Parse(snapshot.Child("IVS").Child("repeatTimes").Value.ToString());


            targetSizeCloud = new List<float>();
            foreach (var c in snapshot.Child("IVS").Child("targetSizes").Children)
            {
                targetSizeCloud.Add(float.Parse(c.Value.ToString()));
            }

            heightSetCloud = new List<float>();
            foreach (var c in snapshot.Child("IVS").Child("heights").Children)
            {
                heightSetCloud.Add(float.Parse(c.Value.ToString()));
            }

            distanceSetCloud = new List<float>();
            foreach (var c in snapshot.Child("IVS").Child("distances").Children)
            {
                distanceSetCloud.Add(float.Parse(c.Value.ToString()));
            }

            HandJitterThreshold = float.Parse(snapshot.Child("Vars").Child("HandJitterThreshold").Value.ToString());
            ArmPointer.jitterThreshold = HandJitterThreshold;
            WristPointer.jitterThreshold = HandJitterThreshold;

            if(pointerType == "S")
            {
                TeleSystem.gPointer.isGravityActive = false;
            }
            else
            {
                TeleSystem.gPointer.isGravityActive = true;
            }

            TeleSystem.studyNumber = studyNum;
            TeleSystem.technique = Technique;
            setGestureSet();


            
            if (TeleSystem.gPointer is HandTrackGravityPointer)
            {
                HandTrackGravityPointer htp = (HandTrackGravityPointer)TeleSystem.gPointer;

                string newPT = (string)snapshot.Child("personalizationType").Value;

                if(bimanualCombo != (string)snapshot.Child("bimanualCombo").Value)
                {
                    bimanualCombo = (string)snapshot.Child("bimanualCombo").Value;
                    StartCoroutine(UpdateBimanualTech());
                }


                if(curveCombo != (string)snapshot.Child("curveCombo").Value)
                {
                    curveCombo = ((string)snapshot.Child("curveCombo").Value);
                    curveCombo = curveCombo.ToUpper();

                    StartCoroutine(UpdateCurveCombo());
                }

                if (GM.personalizationType != newPT)
                {
                    GM.personalizationType = newPT;
                    StartCoroutine(UpdatePersonalizationType());
                }

                htp.isEnabledWristDistanceForce = (bool)snapshot.Child("Vars").Child("isEnabledWristDistanceForce").Value;
                htp.personalizeArmDistance = (bool)snapshot.Child("Vars").Child("personalizeArmDistance").Value;
                htp.personalizeArmHeight = (bool)snapshot.Child("Vars").Child("personalizeArmHeight").Value;
                htp.personalizeWristAngle = (bool)snapshot.Child("Vars").Child("personalizeWristAngle").Value;


                htp.curveWristAngle = (string)snapshot.Child("Vars").Child("curveWristAngle").Value;
                htp.curveArmDistance = (string)snapshot.Child("Vars").Child("curveArmDistance").Value;
                htp.curveArmHeight = (string)snapshot.Child("Vars").Child("curveArmHeight").Value;

                htp.activeFilter = (string)snapshot.Child("activeFilter").Value;

                adaptive.addExtraPercentage = int.Parse(snapshot.Child("Vars").Child("personalizationAddExtraPercent").Value.ToString());
                adaptive.addExtraPercentageWrist = int.Parse(snapshot.Child("Vars").Child("personalizationAddExtraWrist").Value.ToString());




            }


            ARM_Angle_Multiplier = int.Parse(snapshot.Child("Vars").Child("ARM_Angle_Multiplier").Value.ToString());
            ArmPointer.angleMultiplier = ARM_Angle_Multiplier;

            hudEnabled = (bool)snapshot.Child("hudEnabled").Value;
            TeleSystem.cancellationActive = (bool)snapshot.Child("cancellationActive").Value;

            CreateSets();

            resetLocation = (bool)snapshot.Child("resetLocation").Value;

            if(snapshot.Child("activity").Value.ToString() == "practice")
            {
                GM.isPractice = true;
            }
            else
            {
                GM.isPractice = false;
            }

            if (resetLocation)
            {
                TeleSystem.resetPosition();
                resetLocation = false;

                StartCoroutine(UpdateState());
            }

            if ((bool)snapshot.Child("resetPersonalization").Value){
                adaptive.step = "arm_hmax";
                StartCoroutine(UpdateState());
            }

            //record
            if ((bool)snapshot.Child("recordPersonalization").Value)
            {
                adaptive.recordPersonalization = true;
                StartCoroutine(UpdateState());
            }
            else
            {
                adaptive.recordPersonalization = false;
            }

            if (Technique == "ARM")
            {
                ts.onArmButtonPressed();
                HV.SetTechnique(Technique);
            }
            else if (Technique == "WRIST" || Technique == "WRIST_ARM")
            {
                ts.onWristButtonPressed();
                HV.SetTechnique(Technique);
            }
            else if (Technique == "CONTROLLER")
            {
                ts.onControllerButtonPressed();
                HV.SetTechnique(Technique);
            }
        }
    }
    

    System.Collections.IEnumerator UpdateCurveCombo()
    {
        string AH = "none";
        string AL = "none";
        string WA = "none";

        if (curveCombo == "C0")
        {
            AH = "none";
            AL = "none";
            WA = "none";
        }
        else if (curveCombo == "C1")
        {
            AH = "sigmoid";
            AL = "sigmoid";
            WA = "bell";
        }
        else if(curveCombo == "C2")
        {
            AH = "isigmoid";
            AL = "isigmoid";
            WA = "bell";
        }
        else if (curveCombo == "C3")
        {
            AH = "sigmoid";
            AL = "isigmoid";
            WA = "bell";
        }
        else if (curveCombo == "C4")
        {
            AH = "isigmoid";
            AL = "sigmoid";
            WA = "bell";
        }


        var DBTask1 = RealtimeDB.Child("Vars").Child("curveArmDistance").SetValueAsync(AL);
        var DBTask2 = RealtimeDB.Child("Vars").Child("curveArmHeight").SetValueAsync(AH);
        var DBTask3 = RealtimeDB.Child("Vars").Child("curveWristAngle").SetValueAsync(WA);
        
        yield return new WaitUntil(predicate: () => DBTask1.IsCompleted && DBTask2.IsCompleted && DBTask3.IsCompleted);
    }

    System.Collections.IEnumerator UpdateBimanualTech()
    {
        string tech = bimanualCombo.ToUpper();

        switch (tech)
        {
            case "RPRG":
                pointerHand = "righthand";
                confirmationType = "righthand";
                break;
            case "RPLG":
                pointerHand = "righthand";
                confirmationType = "lefthand";
                break;
            case "RPDW":
                pointerHand = "righthand";
                confirmationType = "timeout";
                break;
            case "LPLG":
                pointerHand = "lefthand";
                confirmationType = "lefthand";
                break;
            case "LPRG":
                pointerHand = "lefthand";
                confirmationType = "righthand";
                break;
        }

        var DBTask1 = RealtimeDB.Child("pointerHand").SetValueAsync(pointerHand);
        var DBTask2 = RealtimeDB.Child("confirmationType").SetValueAsync(confirmationType);

        yield return new WaitUntil(predicate: () => DBTask1.IsCompleted && DBTask2.IsCompleted);
    }

    System.Collections.IEnumerator UpdatePersonalizationType()
    {
        string tech;
        if (GM.personalizationType.StartsWith("A"))
        {
            tech = "ARM";
        }
        else
        {
            tech = "WRIST_ARM";
        }

        bool height = false;
        bool dist = false;
        bool wangle = false;
        bool disforce = false;

        switch (GM.personalizationType)
        {
            case "A1":
                break;
            case "A2":
                disforce = true;
                break;
            case "A3":
                disforce = true;
                dist = true;
                break;
            case "A4":
                height = true;
                break;
            case "A5":
                disforce = true;
                dist = true;
                height = true;
                break;

            case "W1":
                break;
            //case "W2":
            //    disforce = true;
            //    break;
            case "W2":
                disforce = true;
                dist = true;
                break;
            //case "W4":
            //    wangle = true;
            //    break;
            case "W3":
                disforce = true;
                dist = true;
                wangle = true;
                break;
        }

        var DBTask1 = RealtimeDB.Child("Vars").Child("isEnabledWristDistanceForce").SetValueAsync(disforce);
        var DBTask2 = RealtimeDB.Child("Vars").Child("personalizeArmDistance").SetValueAsync(dist);
        var DBTask3 = RealtimeDB.Child("Vars").Child("personalizeArmHeight").SetValueAsync(height);
        var DBTask4 = RealtimeDB.Child("Vars").Child("personalizeWristAngle").SetValueAsync(wangle);

        var DBTask5 = RealtimeDB.Child("technique").SetValueAsync(tech);

        

        yield return new WaitUntil(predicate: () => DBTask1.IsCompleted && DBTask2.IsCompleted && DBTask3.IsCompleted && DBTask4.IsCompleted && DBTask5.IsCompleted);
    }

    public static System.Collections.IEnumerator AddDataLog(string studyId, StudyDataLog log)
    {
        DatabaseReference RealtimeDB = FirebaseDatabase.DefaultInstance.RootReference.Child("studylogs");

        string ts = log.timestamp.ToString().Replace("/","-");
        log.curveCombo = curveCombo;

        var DBTask = RealtimeDB.Child(studyId).Child("participant_"+ log.ParticipantId).Child(ts).SetValueAsync(log.toCSVString());

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);
    }

    public static System.Collections.IEnumerator LogTrialSetting(string studyId, StudyDataLog log)
    {
        DatabaseReference RealtimeDB = FirebaseDatabase.DefaultInstance.RootReference.Child("studylogs");

        string ts = log.timestamp.ToString().Replace("/", "-");

        var DBTask = RealtimeDB.Child(studyId).Child("participant_" + log.ParticipantId + "_settings").Child(ts).SetValueAsync(log.toCSVString() + "," + StudyDataLog.wrap(log.dhCombos));

        yield return new WaitUntil(predicate: () => DBTask.IsCompleted);

    }

    System.Collections.IEnumerator UpdateState()
    {
        //IDictionary<string, object> State = new Dictionary<string, object>();

        var DBTask1 = RealtimeDB.Child("resetLocation").SetValueAsync(resetLocation);
        var DBTask2 = RealtimeDB.Child("restart").SetValueAsync(restart);
        var DBTask3 = RealtimeDB.Child("resetPersonalization").SetValueAsync(false);
        var DBTask4 = RealtimeDB.Child("recordPersonalization").SetValueAsync(false);

        yield return new WaitUntil(predicate: () => DBTask1.IsCompleted && DBTask2.IsCompleted && DBTask3.IsCompleted && DBTask4.IsCompleted);
    }

    public void CreateSets()
    {
        IEnumerable<(float height, float distance, float targetSize)> dhCombos;

        //int[] heightSet = GameManager.HeightSet.ToArray();  // copy by value
        //int[] distanceSet = GameManager.DistanceSet.ToArray(); // copy by value

        float[] heightSet = heightSetCloud.ToArray();
        float[] distanceSet = distanceSetCloud.ToArray();
        float[] targetSizeSet = targetSizeCloud.ToArray();

        dhCombos = (
                from h in heightSet
                from d in distanceSet
                from ts in targetSizeSet
                select (height: h, distance: d, targetSize: ts)
        );

        List<(float height, float distance, float targetSize)> _dhCombos = new List<(float height, float distance, float targetSize)>();

        for(int i = 0; i < repeatTimes; i++)  
        {
            _dhCombos.AddRange(dhCombos.ToList());
        }

        _dhCombos.Shuffle();


        List<(float height, float distance, float targetSize)> _dhCombosZero = new List<(float height, float distance, float targetSize)>();
        List<(float height, float distance, float targetSize)> _dhCombosNonZero = new List<(float height, float distance, float targetSize)>();

        foreach ((float height, float distance, float targetSize)  item in _dhCombos)
        {
            if(item.height == 0)
            {
                _dhCombosZero.Add(item);
            }
            else
            {
                _dhCombosNonZero.Add(item);
            }
        }

        List<(float height, float distance, float targetSize)> _dhCombosBalanced = new List<(float height, float distance, float targetSize)>();


        while (_dhCombosNonZero.Count > 0)
        {
            _dhCombosBalanced.Add(_dhCombosZero.First());
            _dhCombosZero.RemoveAt(0);

            _dhCombosBalanced.Add(_dhCombosNonZero.First());
            _dhCombosNonZero.RemoveAt(0);
        }

        while (_dhCombosZero.Count > 0)
        {
            int index = UnityEngine.Random.Range(0, _dhCombosBalanced.Count);
            if (_dhCombosBalanced[index].height == 0)
            {
                _dhCombosBalanced.Insert(index,_dhCombosZero.First());
                _dhCombosZero.RemoveAt(0);
            }
        }


        //for (int i = 0; i < _dhCombosBalanced.Count(); i++)
        //{
        //    Debug.Log(Time.deltaTime * 1000 + " - " + i + " - " + _dhCombosBalanced[i].height 
        //        + " , " + _dhCombosBalanced[i].distance + " , " + _dhCombosBalanced[i].targetSize);
        //}

        GM.SetGameManagerData(_dhCombosBalanced, this);
    }

    private void setGestureSet()
    {
        if (gestureSet == "GS1")
        {
            TeleSystem.InitializationGesture = PalmReverse;
            TeleSystem.ConfirmationGesture = PalmReverseFistClosed;
            TeleSystem.CancellationGesture = FistCloseGesture;
        }
        else if (gestureSet == "GS2")
        {
            TeleSystem.InitializationGesture = IndexThumbOpenRestClosed;
            TeleSystem.ConfirmationGesture = FistCloseGesture;
            TeleSystem.CancellationGesture = TwoFingerClosed;
        }
        else if (gestureSet == "GS3")
        {
            TeleSystem.InitializationGesture = IndexThumbOpenRestClosed;
            TeleSystem.ConfirmationGesture = FistCloseThumbOutGesture;
            TeleSystem.CancellationGesture = FistCloseGesture;
        }

        if (confirmationType == "timeout")
        {
            TeleSystem.ConfirmationGesture = TimeOut;
            TeleSystem.InitializationGesture.handedness = Handedness.Right;
            TeleSystem.ConfirmationGesture.handedness = Handedness.Right;
            TeleSystem.CancellationGesture.handedness = Handedness.Right;
        }
        else if (confirmationType == "lefthand")
        {
            TeleSystem.InitializationGesture.handedness = Handedness.Left;
            TeleSystem.ConfirmationGesture.handedness = Handedness.Left;
            TeleSystem.CancellationGesture.handedness = Handedness.Left;
        }
        else
        {
            TeleSystem.InitializationGesture.handedness = Handedness.Right;
            TeleSystem.ConfirmationGesture.handedness = Handedness.Right;
            TeleSystem.CancellationGesture.handedness = Handedness.Right;
        }

        if (TeleSystem.gPointer is HandTrackGravityPointer hPointer)
        {
            if (pointerHand == "lefthand")
            {
                Debug.Log("### RTDB - INIT LEFT HAND");
                hPointer.handedness = Handedness.Left;
                hPointer.init();
            }
            else
            {
                Debug.Log("### RTDB - INIT RIGHT HAND");
                hPointer.handedness = Handedness.Right;
                hPointer.init();
            }
        }
        
    }

}
