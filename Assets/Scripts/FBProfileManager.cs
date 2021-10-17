using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;
using UnityEngine.Events;

public class FBProfileManager : MonoBehaviour
{
    private DatabaseReference dbr;
    private static bool isCurrentlyReading = false;
    // GameObject scorePrefab;
    //public Transform content;
    public string userName = "testingacc3@abc_com";
    private Dictionary<string, int> stepsDictionary = new Dictionary<string, int>();
    public Dictionary<string, int> getSteps => stepsDictionary;
    public UnityEvent onStepsUpdate = new UnityEvent();

    async void Start()
    {
        dbr = FirebaseDatabase.DefaultInstance.RootReference;
        userName = FirebaseAuth.DefaultInstance.CurrentUser.DisplayName.Replace(".", "_");
    }

    public void Display()
    {
        /**
        var children = new List<GameObject>();
        foreach (Transform child in content) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        */
        StartCoroutine(DisplayGraph());
    }

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
                //key = date
                //value = snapshot
                
                foreach (var data2 in snapshot.Child(data.Key).Children)
                {
                    //key = time
                    //value = snapshot
                    //Debug.Log($"{data2.Key}-{data2.Value}");
                    foreach (var data3 in snapshot.Child(data.Key).Child(data2.Key).Children)
                    {
                        //key = steps
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
}
