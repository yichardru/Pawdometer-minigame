using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if UNITY_WEBGL
#else
using Firebase;
using Firebase.Auth;
using Firebase.Database;
#endif

public class ProfileDisplay : MonoBehaviour
{
    #if UNITY_WEBGL
    #else
    public TextMeshProUGUI userProfileText;
    public GameObject logInButton;
    public GameObject logOutButton;
    public bool ActiveButton = false;

    void Start()
    {
        FirebaseUser user  = FirebaseAuth.DefaultInstance.CurrentUser;
        //Is user empty?
        if (user == null || user.DisplayName == "")
        {
            userProfileText.text = "Not Logged In";
            logInButton.SetActive(true);
            logOutButton.SetActive(ActiveButton);
        }
        else
        {
            userProfileText.text = $"Logged as: {user.DisplayName}";
            logInButton.SetActive(ActiveButton);
            logOutButton.SetActive(true);

            DatabaseReference database = FirebaseDatabase.DefaultInstance.RootReference;
            Debug.Log($"{database.ToString()}");
        }
    }

    public void SignOut()
    {
        FirebaseAuth.DefaultInstance.SignOut();
        userProfileText.text = "Not Logged In";
        logInButton.SetActive(true);
        logOutButton.SetActive(ActiveButton);
         
    }
    #endif
}
