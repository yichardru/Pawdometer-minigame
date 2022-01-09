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
using SimpleJSON;

public class FirebaseLeaderboard : MonoBehaviour
{
    private static bool isCurrentlyReading = false;
    public GameObject scorePrefab;
    public Transform content;
    #if (UNITY_WEBGL && !UNITY_EDITOR)
    #else
    private DatabaseReference dbr;

    void Start()
    {
        dbr = FirebaseDatabase.DefaultInstance.RootReference;
    }
    #endif
    public void RunDisplayScore()
    {
        var children = new List<GameObject>();
        foreach (Transform child in content) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
        StartCoroutine(DisplayScores());
    }
    
    public IEnumerator DisplayScores()
    {
#if (UNITY_WEBGL && !UNITY_EDITOR)
    FirebaseDatabase.GetJSON("USERS", this.gameObject.name, "ParseUserData", "Failure");
    yield break;
#else
        isCurrentlyReading = true;
        var task = dbr.Child("USERS").OrderByValue().GetValueAsync();
        yield return new WaitUntil(predicate: () => task.IsCompleted);
        isCurrentlyReading = false;
        if (task.Exception != null)
        {
            //Debug.LogWarning($"Failed to register task with {task.Exception}");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            //List<>
            foreach(var user in snapshot.Children)
            {
                GenerateScoreInfo(user.Key, user.Value.ToString());
            }
            //for()
        }
    #endif
    }
#if (UNITY_WEBGL && !UNITY_EDITOR)
public void ParseUserData(string json)
{
    var snapshot = JSON.Parse(json);
    //FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToAlert("Part 1: " + snapshot.ToString());
    foreach (var User in snapshot.Keys)
    {
        //FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToAlert("Part 2: " + User);
        string Score = snapshot.GetValueOrDefault(User, null)?.ToString()??"0";
        GenerateScoreInfo(User, Score);
        //FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToAlert(User + ": " + Score);
    }
}
#endif
    public void GenerateScoreInfo(string username, string value)
    {
        string userInfo = $"{username}-{value}";
        //Debug.Log(userInfo);
        GameObject GO = Instantiate(scorePrefab, content);
        GO.GetComponentInChildren<TextMeshProUGUI>().text = userInfo;
        GO.transform.SetAsFirstSibling();
    }
    
    public void Failure(string error)
    {
        FirebaseUtility.Failure(error);
    }
}
