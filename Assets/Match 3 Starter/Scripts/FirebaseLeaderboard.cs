using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;
using TMPro;

public class FirebaseLeaderboard : MonoBehaviour
{
    private DatabaseReference dbr;
    private static bool isCurrentlyReading = false;
    public GameObject scorePrefab;
    public Transform content;

    async void Start()
    {
        dbr = FirebaseDatabase.DefaultInstance.RootReference;
        StartCoroutine(DisplayScores());
    }

    public IEnumerator DisplayScores()
    {
        isCurrentlyReading = true;
        var task = dbr.Child("USERS").OrderByValue().GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        isCurrentlyReading = false;
        if (task.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {task.Exception}");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            //List<>
            foreach(var user in snapshot.Children)
            {
                string userInfo = $"{user.Key}-{user.Value}";
                Debug.Log(userInfo);
                GameObject GO = Instantiate(scorePrefab, content);
                GO.GetComponentInChildren<TextMeshProUGUI>().text = userInfo;
                GO.transform.SetAsFirstSibling();
            }
            //for()
        }

    }
}
