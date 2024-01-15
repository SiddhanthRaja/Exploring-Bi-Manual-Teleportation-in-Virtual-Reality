using Firebase;
using Firebase.Firestore;
using Firebase.Analytics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using System;

public class FireStore : MonoBehaviour
{
    FirebaseFirestore db;
    
    [SerializeField]
    TechniqueSwitcher ts;

    // Start is called before the first frame update
    void Start()
    {
        db = FirebaseFirestore.DefaultInstance;
        
        //getCurrentState();

        var test = new StudyDataLog { 
            ParticipantId = 1,
            timestamp = System.DateTime.Now,
            t = 3.2f,
            height = 3,
            distance = 10,
            angle = 75,
            technique = "ARM",
            gestureSet = "TEST",
            dhCombos = "12,15,1,51,351,6541,56"
        };

        db.Document("test_data/"+ test.ParticipantId).SetAsync(test);
    }


    void getCurrentState()
    {
        DocumentReference docRef = db.Collection("state_management").Document("Store");

        docRef.GetSnapshotAsync().ContinueWithOnMainThread(task =>
        {
            DocumentSnapshot snapshot = task.Result;
            if (snapshot.Exists)
            {
                Debug.Log(String.Format("Document data for {0} document:", snapshot.Id));

                Dictionary<string, object> Store = snapshot.ToDictionary();
                foreach (KeyValuePair<string, object> pair in Store)
                {
                    Debug.Log(String.Format("{0}: {1}", pair.Key, pair.Value));
                }
            }
            else
            {
                Debug.Log(String.Format("Document {0} does not exist!", snapshot.Id));
            }
        });
    }

}
