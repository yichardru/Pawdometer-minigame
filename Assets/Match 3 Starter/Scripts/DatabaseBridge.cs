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

    async void Start()
    {
        //user = 
        dbr = FirebaseDatabase.DefaultInstance.RootReference;
        if (user != null)
        {
            GetHighScore();
        }
    }

    async void GetHighScore()
    {
        await dbr.GetValueAsync().ContinueWith(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log($"Key: {snapshot.Key}, Value: {snapshot.Value}");
            }
            else if (task.IsFaulted)
            {
                Debug.LogError("An error has occured when getting the Value.");
            }
            else if (task.IsCanceled)
            {
                Debug.LogWarning("Getting Value cancelled!");
            }

        });

        //currentHighScore = dbr.Child("USERS").Child(user.Email);
    }

    public void ChangeHighScore(int newScore)
    {
        if(newScore > currentHighScore)
        {
            dbr.Child("USERS").Child(userName).SetValueAsync(newScore);
        }
    }

    public float GetComboValue(int dayRange = 1)
    {
        return 0;
    }
}
