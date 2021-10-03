using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;

public class FBProfileManager : MonoBehaviour
{
    private DatabaseReference dbr;
    private static bool isCurrentlyReading = false;
    // GameObject scorePrefab;
    //public Transform content;
    public string userName = "testingacc3@abc_com";

    async void Start()
    {
        dbr = FirebaseDatabase.DefaultInstance.RootReference;
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
            Debug.Log(snapshot);
            foreach (var data in snapshot.Children)
            {
                Debug.Log($"{data.Key}-{data.Value}");
                foreach (var data2 in snapshot.Child(data.Key).Children)
                {
                    Debug.Log($"{data2.Key}-{data2.Value}");
                }
            }
        }

    }
}
