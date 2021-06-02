using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.PlayerLoop;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine;
using VivoxUnity;
using System.Linq;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher Instance;
    
    
    public string _appVersion = "0.1";
    public int _maxPlayer = 3;
    public int _gameSceneIndex;
    
    [Space(25)]
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject PlayerListItemPrefab;
    [SerializeField] GameObject startGameButton;

    [Header("UI")] 
    [SerializeField] private TMP_InputField _roomNameInputField;
    [SerializeField] private TMP_Text _errorText;
    [SerializeField] private TMP_Text _roomNameText;


    private VivoxVoiceManager _vivoxVoiceManager;


    
    

    #region UNITY METHOD

    private void Awake()
    {
        Instance = this;
        
        _vivoxVoiceManager = VivoxVoiceManager.Instance;
        _vivoxVoiceManager.OnParticipantAddedEvent += OnParticipantAdded;
        _vivoxVoiceManager.OnParticipantRemovedEvent += OnParticipantRemoved;
        
        
    }
    

    private void OnDestroy()
    {
        _vivoxVoiceManager.OnParticipantAddedEvent -= OnParticipantAdded;
        _vivoxVoiceManager.OnParticipantRemovedEvent -= OnParticipantRemoved;
    }


    void Start()
    {
        Init();
        startGameButton.SetActive(false);
    }
    


    private void Init()
    {
        PhotonNetwork.ConnectUsingSettings();
    }



    #endregion

    #region PUBLIC METHOD
    
    public void StartGame()
    {
        PhotonNetwork.LoadLevel(_gameSceneIndex);
    }

    public void JoinLobbyOnClick(TMP_InputField inputFieldNameText)
    {
        
         PhotonNetwork.JoinLobby();
         PhotonNetwork.NickName = inputFieldNameText.text;
    }
    
    
    public void CreateRoom()
    {
        if(string.IsNullOrEmpty(_roomNameInputField.text))
            return;
        
        RoomOptions ro = new RoomOptions();
        ro.IsOpen = true;
        ro.IsVisible = true;
        ro.MaxPlayers = (byte)_maxPlayer;
        
        PhotonNetwork.CreateRoom(_roomNameInputField.text, ro, TypedLobby.Default);
        VivoxManger.Instance.JoinLobbyChannel(_roomNameInputField.text);
        
        MenuManager.Instance.OpenMenu("Loading");
    }
    
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Loading");
    }
    
    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        VivoxManger.Instance.JoinLobbyChannel(info.Name);
        MenuManager.Instance.OpenMenu("Loading");
    }
    
    #endregion


    #region PHOTON CALLBACKS

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.GameVersion = _appVersion;
        
        MenuManager.Instance.OpenMenu("EnterName");
       // PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        MenuManager.Instance.OpenMenu("Loading");
        
        
    }

    public void ShowTitleMenu()
    {
        MenuManager.Instance.OpenMenu("TitleMenu");
    }

    public override void OnJoinedRoom()
    {
        MenuManager.Instance.OpenMenu("RoomMenu");
        _roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        
        Player[] players = PhotonNetwork.PlayerList;

        foreach(Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        for(int i = 0; i < players.Length; i++)
        {
            Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(players[i]);
        }
        

       // startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void ShowStartButton()
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }
    
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        _errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("ErrorRoom");
    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("TitleMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach(Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }
        
        for(int i = 0; i < roomList.Count; i++)
        {
            if(roomList[i].RemovedFromList)
                continue;
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(roomList[i]);
        }
    }
    
    
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(PlayerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }
    
    
    #endregion

    #region VIVOX

    void UpdateParticipantRoster(IParticipant participant, ChannelId channel, bool isAddParticipant)
    {
        if (isAddParticipant)
        {
            PlayerListItemPrefab.GetComponentInChildren<PlayerListSpeaking>().SetupPlayerItem(participant);
        }
        else
        {
            
        }
    }
    

    void OnParticipantAdded(string userName, ChannelId channel, IParticipant participant)
    {
        UpdateParticipantRoster(participant, channel, true);
    }

    void OnParticipantRemoved(string userName, ChannelId channel, IParticipant participant)
    {
        UpdateParticipantRoster(participant, channel, false);
    }
    #endregion
    
    
    
}
