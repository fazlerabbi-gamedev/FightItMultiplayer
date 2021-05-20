using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VivoxUnity;
using UnityEngine.Android;

public class VivoxManger : MonoBehaviour
{
    public static VivoxManger Instance;
    
    private VivoxVoiceManager _vivoxVoiceManager;
    Client _client = new Client();
    
    
    
    private bool PermissionsDenied;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            
        }
        DontDestroyOnLoad(Instance);
        _vivoxVoiceManager = VivoxVoiceManager.Instance;
        _client.Uninitialize();
        _client.Initialize();
        
        
        
        _vivoxVoiceManager.OnUserLoggedInEvent += OnUserLoggedIn;
        _vivoxVoiceManager.OnUserLoggedOutEvent += OnUserLoggedOut;
        

        if (_vivoxVoiceManager.LoginState == VivoxUnity.LoginState.LoggedIn)
        {
            //On User Login
        }
        else
        {
            //On User Logged out
        }
        
        
        
    }
    

    private void OnDestroy()
    {
        _vivoxVoiceManager.OnUserLoggedInEvent -= OnUserLoggedIn;
        _vivoxVoiceManager.OnUserLoggedOutEvent -= OnUserLoggedOut;
        
    }


    private void OnApplicationQuit()
    {
        if (_vivoxVoiceManager.LoginState == VivoxUnity.LoginState.LoggedIn)
        {
            _vivoxVoiceManager.Logout();
        }
        _client.Uninitialize();
    }


    void Start()
    {
        
    }

    #region LOGIN

    public void LoginToVivoxService(TMP_InputField name)
    {
        if (Permission.HasUserAuthorizedPermission(Permission.Microphone))
        {
            // The user authorized use of the microphone.
            LoginToVivox(name);
        }
        else
        {
            // Check if the users has already denied permissions
            if (PermissionsDenied)
            {
                PermissionsDenied = false;
                LoginToVivox(name);
            }
            else
            {
                PermissionsDenied = true;
                // We do not have permission to use the microphone.
                // Ask for permission or proceed without the functionality enabled.
                Permission.RequestUserPermission(Permission.Microphone);
                LoginToVivox(name);
            }
        }
    }
    
    private void LoginToVivox(TMP_InputField name)
    {


        if (string.IsNullOrEmpty(name.text))
        {
            Debug.LogError("Please enter a display name.");
            return;
        }
        _vivoxVoiceManager.Login(name.text);
    }

    #endregion

    #region JOIN LOBBY

    public void JoinLobbyChannel(string lobbyName)
    {
        _vivoxVoiceManager.JoinChannel(lobbyName, ChannelType.NonPositional, VivoxVoiceManager.ChatCapability.TextAndAudio);
    }

    #endregion
    

    #region Vivox Callbacks

    private void OnUserLoggedIn()
    {
        Debug.Log("User Get Logged In");
        if (Launcher.Instance)
        {
            Launcher.Instance.ShowTitleMenu();
        }
    }

    private void OnUserLoggedOut()
    {
        
    }

    #endregion
}
