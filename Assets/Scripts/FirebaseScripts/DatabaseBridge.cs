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
using SimpleJSON;

public class DatabaseBridge : MonoBehaviour
{
    public static int currentHighScore;

    #if (UNITY_WEBGL && !UNITY_EDITOR)
    private string userName = "testingacc3@abc_com";
    #else
    private DatabaseReference dbr;
    public FirebaseUser user;
    private string userName => user.Email.Replace(".", "_");
    private static bool isCurrentlyReading = false;
    #endif
     void Start()
    {
        #if (UNITY_WEBGL && !UNITY_EDITOR)
        FirebaseAuth.GetUser(this.name, "SetUserName", "Failure");
        // FirebaseFunctions.PrintToAlert(userName);
        #else
        user = FirebaseAuth.DefaultInstance.CurrentUser;
        //Debug.Log(userName);
        dbr = FirebaseDatabase.DefaultInstance.RootReference;
        //StartCoroutine(GetHighScore());
        if (user != null)
        {
            StartCoroutine(GetHighScore());
        }
        #endif
    }

    #if (UNITY_WEBGL && !UNITY_EDITOR)
    public void SetUserName(string name)
    {
        userName = name.Replace(".", "_");
    }
        public void Failure(string name)
    {
    }
    #endif

    public IEnumerator GetHighScore(string user = "")
    {
        #if (UNITY_WEBGL && !UNITY_EDITOR)
        yield return null;
        #else
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
        #endif
    }

    public IEnumerator ChangeHighScore(int newScore)
    {
        #if (UNITY_WEBGL && !UNITY_EDITOR)
        yield return null;
        #else
        while(isCurrentlyReading)
        {
            yield return new WaitForSecondsRealtime(0.25f);
        }

        if (newScore > currentHighScore)
        {
            currentHighScore = newScore;
            print($"currentHighScore = {currentHighScore}");
            var task = dbr.Child("USERS").Child(userName).SetValueAsync(newScore);
            yield return new WaitUntil(predicate: () => task.IsCompleted);
            if (task.Exception != null)
            {
                Debug.LogWarning($"Failed to register task with {task.Exception}");
            }
            StartCoroutine(GetHighScore());
        }
        #endif
    }

    public float GetComboValue(int dayRange = 1)
    {
        return 0;
    }
}
