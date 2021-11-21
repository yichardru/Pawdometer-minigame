using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
#if (UNITY_WEBGL && !UNITY_EDITOR)
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;
#else
using Firebase;
using Firebase.Auth;
using Firebase.Database;
#endif

public class ProfileDisplay : MonoBehaviour
{
    
    public TextMeshProUGUI userProfileText;
    public GameObject logInButton;
    public GameObject logOutButton;
    public bool ActiveButton = false;

    void Start()
    {
    #if (UNITY_WEBGL && !UNITY_EDITOR)
    #else
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
    #endif
    }

    public void SignOut()
    {
        #if (UNITY_WEBGL && !UNITY_EDITOR)
        FirebaseAuth.SignOut(gameObject.name, "success", "failure");
        #else
        FirebaseAuth.DefaultInstance.SignOut();
        #endif
        userProfileText.text = "Not Logged In";
        logInButton.SetActive(true);
        logOutButton.SetActive(ActiveButton);
         
    }


    #if (UNITY_WEBGL && !UNITY_EDITOR)
    void success(string output)
    {
        //FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToAlert(output);
        FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToConsole(output);
    }
    void failure(string output)
    {
        FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToAlert(output);
        FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToConsole(output);
    }
    #endif
}
