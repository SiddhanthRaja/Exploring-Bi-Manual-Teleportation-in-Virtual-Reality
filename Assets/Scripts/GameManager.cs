using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Linq;
using System;

namespace PointSystem
{    
    public class GameManager : MonoBehaviour
    {
        public TeleportationSystem TS;

        public string personalizationType;

        public static int[] AngleSet = { +0, +50, +40, -40, -50 }; //, -60,+60, -75, 75, -90, 90
        //public static int[] HeightSet = { 0, 2, 4, 0, 0 }; //0-0, 0-2, 0-4, 2-0, 4-0
        //public static int[] DistanceSet = { 7, 12 }; //7,14,21

        public bool takePosAngle = true;
        int[] posAngleSet, negAngleSet;

        public List<(float height, float distance, float targetSize)> dhCombos;

        public AudioSource confirm, wrong, onseSetComplete;

        // public TextMeshPro pointsText;

        private float _points = 0;
        private float _maxPoints = 0;

        public int currentPosInCombo = -1;
        public bool resetNeeded = false;

        public float targetSize = 1;

        string nextPos = "";

        public GameObject StandingPlatform;

        private RTDB realtimeDB;

        float height, distance;
        int angle;

        public bool trialStarted = false;
        public float globalTimer = 0.0f;
        public float checkpointTimer = 0.0f;
        public float FOVTimer = 0.0f;
        public int mistakeCount = 0;

        public bool userHasLocatedGamePoint = false;

        private GameObject TrashGamePoints;
        private GamePoint nextOne;


        public bool isPractice = true;

        // GameObject defining the player space i.e. need to add OVRCameraRig into it
        [SerializeField]
        private GameObject _playerSpace;

        // Start is called before the first frame update
        void Start()
        {
            TrashGamePoints = GameObject.Find("TrashGamePoints");

            //pointsText.text = "";

            foreach (GamePoint gamePoint in FindObjectsOfType<GamePoint>())
            {
                gamePoint.OnTriggerEnterAction += OnPointScored;
                gamePoint.OnSkipPoint += OnSkipPoint;
                gamePoint.OnVisibleInFov += OnVisibleInFov;


                
                //_maxPoints += gamePoint.points;
            }

            takePosAngle = UnityEngine.Random.Range(0, 1) == 1;
            posAngleSet = (from a in AngleSet where a >= 0 select a).ToArray();
            negAngleSet = (from a in AngleSet where a <= 0 select a).ToArray();

        }

        void OnVisibleInFov(GamePoint gamePoint)
        {
            if(nextOne != gamePoint)
            {
                return;
            }
            userHasLocatedGamePoint = true;
        }

        void logCheckpoint(float d, float dws, bool isError  = false)
        {
            if (isPractice)
            {
                return;
            }


            var log = new StudyDataLog
            {
                ParticipantId = realtimeDB.ParticipantId,
                timestamp = System.DateTime.Now,
                t = checkpointTimer,
                gt = globalTimer,
                fovt = FOVTimer,
                height = height,
                distance = distance,
                distanceFromCenter = d,
                distanceFromCenterWithSpike = dws,
                angle = angle,
                technique = realtimeDB.Technique,
                gestureSet = realtimeDB.gestureSet,
                pointerType = realtimeDB.pointerType,
                checkpointIndex = currentPosInCombo,
                mistakeCount = mistakeCount,
                personalizationType = personalizationType,
                pointerHand = realtimeDB.pointerHand,
                confirmationType = realtimeDB.confirmationType,
                targetSize = targetSize.ToString()
            };


            if (TS.gPointer is HandTrackGravityPointer)
            {
                HandTrackGravityPointer htp = (HandTrackGravityPointer)TS.gPointer;
                log.armDistance = htp.getArmDistance();
                log.armHeight = htp.getArmHeight();
                log.wristAngle = htp.getWristAngle();
            }


                StartCoroutine(RTDB.AddDataLog("trial", log));
        }


        void logTrialSetting()
        {
            if (isPractice)
            {
                return;
            }

            string dhCombosString = "";
            foreach ((float h, float d, float ts) combo in dhCombos)
            {
                dhCombosString += "h: " + combo.h + " - d: " + combo.d + " - ts: " + combo.ts + ", ";
            }

            var log = new StudyDataLog
            {
                ParticipantId = realtimeDB.ParticipantId,
                timestamp = System.DateTime.Now,
                technique = realtimeDB.Technique,
                gestureSet = realtimeDB.gestureSet,
                pointerType = realtimeDB.pointerType,
                dhCombos = dhCombosString,
                checkpointIndex = currentPosInCombo
            };


            StartCoroutine(RTDB.LogTrialSetting("trial", log));
        }

        void OnSkipPoint(GamePoint gamePoint)
        {
            //return;
            //wrong.Play();
            //if (checkpointIsInBasePosition)
            //{
            //    return;
            //}
            //else
            //{
            //    logCheckpoint(0, 0, true);
            //}
            
            //checkpointTimer = 0.0f;

            //currentPosInCombo--;

            //(float height, float distance) p = dhCombos[currentPosInCombo];
            //dhCombos.RemoveAt(currentPosInCombo);
            //dhCombos.Add(p);

            //NextLocation(gamePoint, true);
        }

        void OnPointScored(GamePoint gamePoint, float distanceFromCenter, float distanceFromCenterWithSpike)
        {

            confirm.Play();
            SetCheckpointStatus(gamePoint.gameObject, false);
            //Debug.Log((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds + "POINT SCORED" + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);

            if (currentPosInCombo >= dhCombos.Count && trialStarted)
            {
                Debug.Log("IN HERE, RESTARTING");
                nextPos = "One set complete, please wait for instruction.";

                distanceFromCenter = TS.distanceFromCenter;
                distanceFromCenterWithSpike = TS.distanceFromCenterWithSpike;

                logCheckpoint(distanceFromCenter, distanceFromCenterWithSpike);

                restart();
                finishedSet();
                
            }
            else
            {
                Vector3 cam = Camera.main.transform.position - _playerSpace.transform.position;
                StandingPlatform.transform.position = new Vector3(cam.x, cam.y - 0.5f, cam.z);

                if (currentPosInCombo < 0 && trialStarted == false)
                {
                    //Debug.Log("STARTING TRIAL" + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);

                    trialStarted = true;
                    currentPosInCombo = 0;

                    logTrialSetting();
                }
                else
                {
                    Debug.Log("CONT TRIAL" + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
                    if (!checkpointIsInBasePosition)
                    {
                        distanceFromCenter = TS.distanceFromCenter;
                        distanceFromCenterWithSpike = TS.distanceFromCenterWithSpike;
                        
                        logCheckpoint(distanceFromCenter, distanceFromCenterWithSpike);
                        _points += gamePoint.points;
                    }

                    checkpointTimer = 0.0f;
                    FOVTimer = 0.0f;
                    mistakeCount = 0;
                    userHasLocatedGamePoint = false;
                }

                //Debug.Log(currentPosInCombo + " , --- : " + dhCombos.Count);

                NextLocation(gamePoint);
                TS.gPointer.resetKalmanFilters();
                //Destroy(gamePoint.gameObject);

            }

        }

        private void finishedSet()
        {
            onseSetComplete.Play();
            resetCheckpoint(true);
        }

        int prevAngle = 0;
        private bool checkpointIsInBasePosition = true;
        private readonly bool resetEveryTime = false;

        void setRandomAngle()
        {
            if (takePosAngle)
            {
                angle = posAngleSet[UnityEngine.Random.Range(0, posAngleSet.Length)];
                takePosAngle = false;
            }
            else
            {
                angle = negAngleSet[UnityEngine.Random.Range(0, negAngleSet.Length)];
                takePosAngle = true;
            }

            if(prevAngle == angle)
            {
                takePosAngle = !takePosAngle;
                setRandomAngle();
            }
        }

        private void NextLocation(GamePoint gamePoint, bool skip = false)
        {
            //Debug.Log((int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds +"NEXT LOCATION TRIGGERED" + currentPosInCombo
            //    + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name );
            if (dhCombos == null)
            {
                return;
            }

            if (resetEveryTime)
            {
                if (resetNeeded)
                {
                    resetCheckpoint();
                    return;
                }
                else
                {
                    resetNeeded = true;
                }
            }

            height = dhCombos[currentPosInCombo].height;
            distance = dhCombos[currentPosInCombo].distance;

            targetSize = dhCombos[currentPosInCombo].targetSize;
            setRandomAngle();

            //nextPos = "h: " + height + " - d: " + distance + " - a:" + angle;
            nextPos = "";




            TranslateGamePoint(gamePoint, height, distance, angle);
            currentPosInCombo++;
        }

        internal void restart()
        {
            TS.resetPosition();

            trialStarted = false;
            globalTimer = 0.0f;
            checkpointTimer = 0.0f;

            currentPosInCombo = -1;
            _points = 0;

            Debug.Log("ts " + trialStarted);

            resetCheckpoint();
        }

        void resetCheckpoint(bool checkpointFarAway = false)
        {

            StandingPlatform.transform.position = new Vector3(0,-10,-10);
            foreach (Transform child in transform)
            {
                if(child.name == "Checkpoint")
                {
                    if (checkpointFarAway)
                    {
                        child.position = new Vector3(-20, -20, -40);
                    }
                    else
                    {
                        child.position = new Vector3(0, 0, 4);
                    }


                    SetCheckpointStatus(child.gameObject, true);


                    TS.CheckpointPlate = child.transform.Find("CheckpointPlate").gameObject;
                        
                }

            }


            resetNeeded = false;
            checkpointIsInBasePosition = true;

            foreach (Transform child in TrashGamePoints.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        private void TranslateGamePoint(GamePoint gamePoint, float height, float distance, int angle)
        {
            Vector3 oldPos = gamePoint.transform.position;

            oldPos.y = 0;

            //gamePoint.transform.position = oldPos;
            //Vector3 camDir = CameraCache.Main.transform.forward;

            Vector3 camDir = gamePoint.gameObject.transform.forward.normalized;

            if (realtimeDB.studyNum == 1)
            {
                oldPos.x = 0;
                oldPos.y = 0;
                oldPos.z = 0;

                camDir = StandingPlatform.transform.forward.normalized;
            }

            camDir = Quaternion.Euler(0, angle, 0) * camDir;
            camDir.y = 0;

            //Debug.Log("Instantiating" + (new System.Diagnostics.StackTrace()).GetFrame(1).GetMethod().Name);
            GameObject newGamePoint = GameObject.Instantiate(gamePoint.gameObject);
            newGamePoint.transform.parent = TrashGamePoints.transform;
            //newGamePoint.name = "Checkpoint_" + currentPosInCombo;


            newGamePoint.transform.position = oldPos;
            newGamePoint.GetComponent<GamePoint>().OnTriggerEnterAction += OnPointScored;
            newGamePoint.GetComponent<GamePoint>().OnSkipPoint += OnSkipPoint;
            newGamePoint.GetComponent<GamePoint>().OnVisibleInFov += OnVisibleInFov;

            nextOne = newGamePoint.GetComponent<GamePoint>();

            SetCheckpointStatus(newGamePoint.gameObject, true);

            if (realtimeDB.studyNum == 1)
            {
                newGamePoint.transform.position = camDir * distance + Vector3.up * height;
            }
            else
            {
                newGamePoint.transform.Translate(camDir * distance);
                newGamePoint.transform.Translate(Vector3.up * height);
            }


            newGamePoint.transform.localScale = new Vector3(targetSize, targetSize, targetSize);


            checkpointIsInBasePosition = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (trialStarted)
            {
                float t = Time.deltaTime;
                globalTimer += t;
                checkpointTimer += t;

                if (!userHasLocatedGamePoint)
                {
                    FOVTimer += t;
                }

            }

            if (realtimeDB && realtimeDB.hudEnabled)
            {
                /*
                pointsText.gameObject.SetActive(true);
                pointsText.text = nextPos
                + "\n" + "Checkpoints Left: " + (_maxPoints - _points);
                */
                //+ "\nCheckpoints Complete: " + _points
                //+ "\n GT: " + globalTimer.ToString("F3") + " _ CT: " + checkpointTimer.ToString("F3") + " _ FOVT: " + FOVTimer.ToString("F3");
            }
            else
            {
                //pointsText.gameObject.SetActive(false);
            }

        }

        public void SetGameManagerData(List<(float height, float distance, float targetSize)> _dhCombos, RTDB r)
        {
            realtimeDB = r;
            dhCombos = _dhCombos;
            _maxPoints = dhCombos.Count;
        }

        public static void SetCheckpointStatus(GameObject checkpoint, bool status)
        {
            checkpoint.gameObject.GetComponent<Collider>().enabled = status;
            checkpoint.gameObject.transform.Find("Arrow").gameObject.SetActive(status);
            checkpoint.gameObject.transform.Find("CheckpointPlate").GetComponent<Renderer>().enabled = status;

            checkpoint.gameObject.transform.Find("CheckpointPlate").GetComponent<MeshCollider>().enabled = status;
            checkpoint.gameObject.transform.Find("CheckpointPlate").GetComponent<BoxCollider>().enabled = status;

        }

    }
}
