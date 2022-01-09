using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if (UNITY_WEBGL && !UNITY_EDITOR)
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
#else
using Firebase.Database;
using Firebase.Auth;
#endif
using TMPro;
using UnityEngine.Events;
using SimpleJSON;

public class FBProfileManager : MonoBehaviour
{
    private static bool isCurrentlyReading = false;
    // GameObject scorePrefab;
    //public Transform content;
    public string userName = "testingacc3@abc_com";
    private Dictionary<string, int> stepsDictionary = new Dictionary<string, int>();
    public Dictionary<string, int> getSteps => stepsDictionary;
    public UnityEvent onStepsUpdate = new UnityEvent();
#if (UNITY_WEBGL && !UNITY_EDITOR)
    void Start(){
        FirebaseAuth.GetUser(this.name, "SetUserName", "Failure").Replace(".", "_");
        //FirebaseFunctions.PrintToAlert($"USER: {FirebaseAuth.GetUser(this.name, "Y", "X").Replace(".", "_")}\nEMAIL:{FirebaseAuth.GetUserEmail(this.name, "Y", "X").Replace(".", "_")}");
        // FirebaseFunctions.PrintToAlert(userName);
    }

    public void SetUserName(string name)
    {
        userName = userName;
    }
#else
    private DatabaseReference dbr;
    void Start()
    {
        dbr = FirebaseDatabase.DefaultInstance.RootReference;
        userName = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName.Replace(".", "_");
    }
    #endif

    public void Display()
    {
        /**
        var children = new List<GameObject>();
        foreach (Transform child in content) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        */
#if (UNITY_WEBGL && !UNITY_EDITOR)
        FirebaseDatabase.GetJSON($"DATA/{userName}", this.gameObject.name, "ParseUserData", "Failure");
#else
        StartCoroutine(DisplayGraph());
        #endif
    }

#if (UNITY_WEBGL && !UNITY_EDITOR)
    public void ParseUserData(string json)
    {
        var snapshot = JSON.Parse(json);
        FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToAlert("Full User Data: " + snapshot.ToString());
        foreach (var date in snapshot.Keys){
            int totalsteps = 0;
            foreach (var time in snapshot[date].Keys){
                foreach (var steps in snapshot[date][time].Keys){
                    //TODO: add currentsteps to totalsteps
                    if(int.TryParse(steps, out int numSteps))
                    {
                        totalsteps += numSteps;
                    }
                }
            }
            //TODO: add date and totalsteps to dictionary
            stepsDictionary[date] = totalsteps;
        }
        onStepsUpdate?.Invoke();

    }

    public void Failure(string error)
    {
        FirebaseFunctions.PrintToAlert(error);
    }


#else
    public IEnumerator DisplayGraph()
    {
        isCurrentlyReading = true;
        var task = dbr.Child("DATA").Child(userName).OrderByValue().GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        isCurrentlyReading = false;
        if (task.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {task.Exception}");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            stepsDictionary.Clear();
            foreach (var data in snapshot.Children)
            {
                int stepsTotal = 0;
                //key = "date"
                //value = snapshot
                
                foreach (var data2 in snapshot.Child(data.Key).Children)
                {
                    //key = "time"
                    //value = snapshot
                    //Debug.Log($"{data2.Key}-{data2.Value}");
                    foreach (var data3 in snapshot.Child(data.Key).Child(data2.Key).Children)
                    {
                        //key = "steps"
                        //value = num
                        //Debug.Log($"{data3.Key}-{data3.Value}");
                        if (int.TryParse(data3.Value.ToString(), out int result)){
                            stepsTotal += result;
                        }
                        
                    }
                }
                Debug.Log($"{data.Key}:{stepsTotal}");
                stepsDictionary[data.Key] = stepsTotal;
            }
            onStepsUpdate?.Invoke();
        }

    }
    #endif
}
