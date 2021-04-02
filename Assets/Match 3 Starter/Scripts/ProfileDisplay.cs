using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Firebase;
using Firebase.Auth;
using Firebase.Database;

public class ProfileDisplay : MonoBehaviour
{
    public TextMeshProUGUI userProfileText;
    public GameObject logInButton;
    public GameObject logOutButton;

    void Start()
    {
        FirebaseUser user  = FirebaseAuth.DefaultInstance.CurrentUser;
        //Is user empty?
        if (user == null || user.DisplayName == "")
        {
            userProfileText.text = "Not Logged In";
            logInButton.SetActive(true);
            logOutButton.SetActive(false);
        }
        else
        {
            userProfileText.text = $"Logged as: {user.DisplayName}";
            logInButton.SetActive(false);
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
        logOutButton.SetActive(false);
    }
}
