using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    #region Singleton

    public static PhotonManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #endregion


    #region VARIABLE
    public string _appVersion = "0.1";
    public int _maxPlayer = 5;
    
    
    
    
    
    private  readonly string connecting = "Connecting......";
    private  readonly string connected = "Connected!!!!";
    private  readonly string disconnected = "Disconnected";
    

    #endregion


    private void Start()
    {
        Init();
    }

    private void Init()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.GameVersion = _appVersion;

        Debug.Log("Initialize Network Connection");
        UIMultiplayerManager.Instance._connectionText.text = connecting;
        
        ConnectToPhoton();
    }

    private void ConnectToPhoton()
    {
        PhotonNetwork.ConnectUsingSettings();
    }


    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Network Connected");
        // Show connected text
        UIMultiplayerManager.Instance._connectionText.text = connected;
        //Show Ok Button
        UIMultiplayerManager.Instance.ConnectionOkButton(true);
    }






    #region DISCONNECT

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        UIMultiplayerManager.Instance._connectionText.text = disconnected;
    }

    public void DisConnectByBackButton()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    

    #endregion
}
