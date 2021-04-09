using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Auth;

public class DatabaseBridge : MonoBehaviour
{
    private DatabaseReference dbr;
    public static int currentHighScore;
    public FirebaseUser user;
    private string userName => user.Email.Replace(".", "_");
    private static bool isCurrentlyReading = false;

    async void Start()
    {
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        //Debug.Log(userName);
        dbr = FirebaseDatabase.DefaultInstance.RootReference;
        //StartCoroutine(GetHighScore());
        if (user != null)
        {
            StartCoroutine(GetHighScore());
        }
    }

    public IEnumerator GetHighScore(string user = "")
    {
        isCurrentlyReading = true;
        var task = dbr.GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        isCurrentlyReading = false;
        if (task.Exception != null)
        {
            Debug.LogWarning($"Failed to register task with {task.Exception}");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            Debug.Log($"Key: {snapshot.Key}");
            Debug.Log($"Value: {snapshot.Child("USERS").Child(userName).GetRawJsonValue()}");
            if (!int.TryParse(snapshot.Child("USERS").Child(userName).GetRawJsonValue(), out currentHighScore))
            {
                currentHighScore = 0;
            }
        }
        print(currentHighScore);
        
    }

    public IEnumerator ChangeHighScore(int newScore)
    {
        while(isCurrentlyReading)
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }

        if (newScore > currentHighScore)
        {
            var task = dbr.Child("USERS").Child(userName).SetValueAsync(newScore);
            yield return new WaitUntil(predicate: () => task.IsCompleted);
            if (task.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {task.Exception}");
            }
        }
    }

    public float GetComboValue(int dayRange = 1)
    {
        return 0;
    }
}
