﻿using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
#if (UNITY_WEBGL && !UNITY_EDITOR)
using FirebaseWebGL.Scripts.FirebaseBridge;
using FirebaseWebGL.Scripts.Objects;

#else
using Firebase;
using Firebase.Auth;
#endif

public class AuthManager : MonoBehaviour
{
    #if (UNITY_WEBGL && !UNITY_EDITOR)
    #else
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;    
    public FirebaseUser User;
    #endif

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    [Header("Events")]
    public UnityEvent OnLogin = new UnityEvent();


    private GameManager currentGM;
    
    void Awake()
    {
    #if (UNITY_WEBGL && !UNITY_EDITOR)
    #else
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
        #endif
    }

    private void Start()
    {
        //currentGM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
    #if (UNITY_WEBGL && !UNITY_EDITOR)
    #else
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
        #endif
    }
    
    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
    #if (UNITY_WEBGL && !UNITY_EDITOR)
        FirebaseAuth.SignInWithEmailAndPassword(emailLoginField.text, passwordLoginField.text, gameObject.name, "OnLoginSucess", "Failure");
    #else
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
        #endif
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
    #if (UNITY_WEBGL && !UNITY_EDITOR)
        FirebaseAuth.CreateUserWithEmailAndPassword(emailRegisterField.text, passwordRegisterField.text, gameObject.name, "OnRegisterSuccess", "Failure");
    #else
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
        #endif
    }
    #if (UNITY_WEBGL && !UNITY_EDITOR)
    void Success(string output)
    {
        //FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToAlert(output);
        // FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToConsole(output);
    }
    void Failure(string output)
    {
        // FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToAlert(output);
        // FirebaseWebGL.Scripts.FirebaseBridge.FirebaseFunctions.PrintToConsole(output);
    }
    #else
    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            //User is now logged in
            //Now get the result
            User = LoginTask.Result;
            Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
            warningLoginText.text = "";
            confirmLoginText.text = "Logged In";
            //currentGM?.ReturnToMenu();
            OnLoginSucess();
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        //if (_username == "")
        //{
            //If the username field is blank show a warning
          //  warningRegisterText.text = "Missing Username";
        //}
        if(passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else 
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile{DisplayName = _email};

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        //Now return to login screen
                        OnRegisterSuccess();
                    }
                }
            }
        }
    }
    #endif

    private void OnRegisterSuccess(string output = "")
    {
        UIManager.instance.LoginScreen();                        
        warningRegisterText.text = "";
    }

    private void OnLoginSucess(string output = "")
    {
        OnLogin?.Invoke();
    }
}
