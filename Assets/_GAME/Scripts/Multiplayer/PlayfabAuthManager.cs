using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using UnityEngine.UI;
using TMPro;

public class PlayfabAuthManager : MonoBehaviour
{
    #region Singleton

    public static PlayfabAuthManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #endregion
    
    
    
    
    #region Var

    [HideInInspector]
    public TMP_InputField _Username;
    [HideInInspector]
    public TMP_InputField _Password;

    
    //Accessbility for PlayFab ID & Session Tickets
    public static string PlayFabId { get { return _playFabId; } }
    private static string _playFabId;
    public static string SessionTicket { get { return _sessionTicket; } }
    private static string _sessionTicket;
    
    private string _playFabPlayerIdCache = "";
    
    
    public delegate void LoginSuccessFull();

    public static event LoginSuccessFull OnAfterLogin;
    
    private string playerName;

    #endregion
    
    
    
    public void PlayfabAuthOnClick()
    {
        AuthWithPlayfabLogin();
        
    }

    private void AuthWithPlayfabLogin()
    {
        PlayFabClientAPI.LoginWithPlayFab(new LoginWithPlayFabRequest()
            {
                TitleId = PlayFabSettings.TitleId,
                Username = _Username.text,
                Password = _Password.text

            },
            (result) =>
            {
                RequestToken(result);
                
                //store identity and session
                _playFabId = result.PlayFabId;
                _sessionTicket = result.SessionTicket;
                
                //Login Successful
                OnLoginSuccess();
            } , 
            OnErrorAuthPlayfabLogin);
    }
    
    
    
    public void AuthWithPlayfabRegister(string email)
    {
        PlayFabClientAPI.RegisterPlayFabUser(new RegisterPlayFabUserRequest()
        {
            //Email = "zaynax"+ Random.Range(0,100000)+Random.Range(0,100000) +"@playfab.com",
            Email = email,
            Username = _Username.text,
            Password = _Password.text
            
        }, (regresult) =>
        {
            AuthWithPlayfabLogin();
        }, OnErrorAuthPlayfabRegister);
    }
    
    
    void RequestToken(LoginResult result)
    {
        
        _playFabPlayerIdCache = result.PlayFabId;
        GetPhotonAuthenticationTokenRequest request = new GetPhotonAuthenticationTokenRequest();

        request.PhotonApplicationId = PhotonNetwork.PhotonServerSettings.AppSettings.AppIdRealtime;
        PlayFabClientAPI.GetPhotonAuthenticationToken(request, AuthWithPhoton,OnErrorToken);
    }
    
    void AuthWithPhoton(GetPhotonAuthenticationTokenResult result)
    {
        var customAuth = new AuthenticationValues { AuthType = CustomAuthenticationType.Custom };
        customAuth.AddAuthParameter("username", _playFabPlayerIdCache);
        customAuth.AddAuthParameter("token", result.PhotonCustomAuthenticationToken);
        
        PhotonNetwork.AuthValues = customAuth;
        // PhotonManager.ConnectToPhoton();
    }
    
    
    
    public void OnLoginSuccess()
    {
        UIMultiplayerManager.Instance.MDisableAllScreens();
        OnAfterLogin?.Invoke();

    }
    
    
    void OnErrorAuthPlayfabLogin(PlayFabError error)
    {
        switch (error.Error)
        {
            case PlayFabErrorCode.InvalidUsername:
            case PlayFabErrorCode.InvalidPassword:
            case PlayFabErrorCode.InvalidUsernameOrPassword:
                //ProgressBar.UpdateLabel("Invalid Email or Password");
                break;

            case PlayFabErrorCode.AccountNotFound:
                //Register
                //AuthWithPlayfabRegister();
                Debug.Log("Show Reg Panel");
                UIMultiplayerManager.Instance.ShowRegPanel();
                return;
            default:
                
                break;
        }
    }
    
    
    public string GetPlayerInfo()
    {
        
        GetAccountInfoRequest request = new GetAccountInfoRequest();
        PlayFabClientAPI.GetAccountInfo(request, (result) =>
        {
            playerName = result.AccountInfo.Username;

        }, (error) =>
        {
            playerName = "";
        });
        return playerName;
    }
    
    
    
    
    
    
    
    void OnErrorAuthPlayfabRegister(PlayFabError error)
    {
        
    }
    
    void OnErrorToken(PlayFabError error)
    {

    }
    
    
    
    
}
