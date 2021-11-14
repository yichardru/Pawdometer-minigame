﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WEBGL
#else
using Firebase.Database;
using Firebase.Auth;
#endif
using TMPro;

public class FirebaseLeaderboard : MonoBehaviour
{
    #if UNITY_WEBGL
    #else
    private DatabaseReference dbr;
    private static bool isCurrentlyReading = false;
    public GameObject scorePrefab;
    public Transform content;

    async void Start()
    {
        dbr = FirebaseDatabase.DefaultInstance.RootReference;
    }

    public void RunDisplayScore()
    {
        var children = new List<GameObject>();
        foreach (Transform child in content) children.Add(child.gameObject);
        children.ForEach(child => Destroy(child));
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
            //Debug.LogWarning($"Failed to register task with {task.Exception}");
        }
        else
        {
            DataSnapshot snapshot = task.Result;
            //List<>
            foreach(var user in snapshot.Children)
            {
                string userInfo = $"{user.Key}-{user.Value}";
                //Debug.Log(userInfo);
                GameObject GO = Instantiate(scorePrefab, content);
                GO.GetComponentInChildren<TextMeshProUGUI>().text = userInfo;
                GO.transform.SetAsFirstSibling();
            }
            //for()
        }

    }
    #endif
}
