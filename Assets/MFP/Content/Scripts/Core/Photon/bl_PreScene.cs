using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable; //Replace default Hashtables with Photon hashtables
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client;

public class bl_PreScene : bl_PhotonHelper{

    public Text RoomNameText = null;
    public GameObject StartButton = null;
    public GameObject ReadyButton = null;
    public GameObject EnterButton = null;
    public List<bl_PlayerLobby> PlayersInRoom = new List<bl_PlayerLobby>();
    //
    [Separator("Player List")]
    public Transform PlayerListPanel;
    public GameObject PlayerPrefabs;
    public List<bl_PlayerLobby> Players = new List<bl_PlayerLobby>();

    [Separator("Chat")]
    public Text ChatText = null;
    public static Text CacheChat = null;

    [Separator("References")]
    [SerializeField]private Text TimeText;
    [SerializeField] private Text MaxPlayers;


    public static bool isReady = false;
    private bool isAlredyLoaded = false;
    private bool LocalStarted = false;

    /// <summary>
    /// 
    /// </summary>
    void OnEnable()
    {
        if (!isConnected)
        {
            Debug.Log("Not connected to room, please try again");
            this.enabled = false;
            return;
        }
        //Register in photon events calls
        PhotonNetwork.NetworkingClient.EventReceived += this.OnEventCustom;
        bl_PhotonCallbacks.Instance.OnLeft+=(OnLeftRoom);
        bl_PhotonCallbacks.Instance.OnJoined+=(OnJoinedRoom);
        bl_PhotonCallbacks.Instance.OnPlayerLeft+=(OnPhotonPlayerDisconnected);
    }
    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        //unregister from photon event calls
        PhotonNetwork.NetworkingClient.EventReceived -= this.OnEventCustom;
        CancelInvoke("InvokeList");
        bl_PhotonCallbacks.Instance.OnLeft-=(OnLeftRoom);
        bl_PhotonCallbacks.Instance.OnJoined-=(OnJoinedRoom);
        bl_PhotonCallbacks.Instance.OnPlayerLeft-=(OnPhotonPlayerDisconnected);
    }
    /// <summary>
    /// When is master player you have the control of room
    /// only master client can start the match
    /// </summary>
    void MasterLogic()
    {
        StartButton.SetActive(true);
        ReadyButton.SetActive(false);
        EnterButton.SetActive(false);
        isReady = true;
    }
    /// <summary>
    /// When you are a normal client
    /// you only can wait for start the match
    /// </summary>
    void NormalPlayerLogic()
    {
        StartButton.SetActive(false);
        ReadyButton.SetActive(true);
        EnterButton.SetActive(false);

    }
    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        //verify is level loaded
        if (PhotonNetwork.CurrentRoom.GetRoomState() && !isAlredyLoaded)
        {
            PhotonNetwork.IsMessageQueueRunning = false;
            isAlredyLoaded = true;
            LevelLoaded(0);
        }
    }
    /// <summary>
    /// when master client start match and you are not ready yet
    /// then, you can load the map where you want.
    /// </summary>
    public void LevelLoaded(int state)
    {
        if (state == 0)
        {
            StartButton.SetActive(false);
            ReadyButton.SetActive(false);
            EnterButton.SetActive(true);
        }
        else
        {

            PhotonNetwork.IsMessageQueueRunning = false;
            CancelInvoke("InvokeList");
            bl_UtilityHelper.LoadLevel(PhotonNetwork.CurrentRoom.RoomScene());
        }
    }
    /// <summary>
    /// Update the player list with all photon player in room
    /// </summary>
    void InvokeList()
    {
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
        {
            UpdatPlayerList(PhotonNetwork.PlayerList[i].NickName, PhotonNetwork.PlayerList[i].GetReady());
        }
    }

    /// <summary>
    /// Load and sync level
    /// Only master client can call this function
    /// </summary>
    public void LoadSyncLevel()
    {
        if (LocalStarted)
            return;

        //Start game only when all are ready
        if (PhotonNetwork.PlayerList.AllPlayersReady())
        {
            Hashtable e = new Hashtable();
            e.Add("Level", PhotonNetwork.CurrentRoom.RoomScene());
            SendOptions so = new SendOptions() { Reliability = true };
            PhotonNetwork.RaiseEvent(EventID.LoadSyncLevel, e, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, so);
            LocalStarted = true;
        }
        else
        {
            Debug.Log("Some players are not ready, wait for his");
        }
    }
    /// <summary>
    /// change the state and sync for other players
    /// </summary>
    public void Ready()
    {
        isReady = !isReady;
        PhotonNetwork.LocalPlayer.SendReady(isReady);
        Hashtable e = new Hashtable();
        e.Add("PName",LocalName);
        e.Add("Ready",isReady);
        SendOptions so = new SendOptions() { Reliability = true };
        PhotonNetwork.RaiseEvent(EventID.PlayerJoinPre, e, new RaiseEventOptions() { Receivers = ReceiverGroup.All }, so);  
    }
    /// <summary>
    /// Update the player list
    /// </summary>
    public void UpdatPlayerList(string n,bool ready,bool remove = false)
    {
        if (!remove)
        {
            bool find = false;
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].PlayerName == n)
                {
                    find = true;//then, only update
                    Players[i].GetInfo(n, ready);
                }
            }
            //If a new player
            if (!find)//if player doesn't exist.
            {
                GameObject p = Instantiate(PlayerPrefabs) as GameObject;
                bl_PlayerLobby pl = p.GetComponent<bl_PlayerLobby>();
                pl.GetInfo(n, ready);
                Players.Add(pl);
                p.transform.SetParent(PlayerListPanel, false);
            }
        }
        else
        {
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].PlayerName == n)
                {
                    Destroy(Players[i].gameObject);
                    Players.RemoveAt(i);
                }
            }
        }
    }
    /// <summary>
    /// 
    /// </summary>
    private List<Player> PlayersAvailables
    {
        get
        {
            List<Player> list = new List<Player>();
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                list.Add(p);
            }
            return list;
        }
    }
    // <summary>
    /// Receive events from server
    /// </summary>
    /// <param name="eventCode"></param>
    /// <param name="content"></param>
    /// <param name="senderID"></param>
    public void OnEventCustom(EventData data)
    {
        Hashtable hash = new Hashtable();
        hash = (Hashtable)data.CustomData;
        switch (data.Code)
        {
            case EventID.PlayerJoinPre:
                string mName = (string)hash["PName"];
                bool ready = (bool)hash["Ready"];

                UpdatPlayerList(mName, ready);
                break;
        }
    }
    /// <summary>
    /// Simple Chat
    /// </summary>
    /// <param name="msn"></param>
    public static void SendChat(string sender, string msn)
    {
        CacheChat.text += "\n ["+ bl_CoopUtils.CoopColorStr(sender) + "] " + msn;
    }
    /// <summary>
    /// Add text sync
    /// </summary>
    /// <param name="i"></param>
    public void NewChatMsn(InputField i)
    {
        string t = i.text;
        photonView.RPC("AddChat", RpcTarget.All, t);
        i.text = string.Empty;
    }

    [PunRPC]
    void AddChat(string text,PhotonMessageInfo p)
    {
        ChatText.text += "\n (" + bl_CoopUtils.CoopColorStr(p.Sender.NickName) + "): " + text;
    }
    // We have two options here: we either joined(by title, list or random) or created a room.
    public void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SendRoomState(false);
        }
        bool b = false;
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(PropiertiesKeys.RoomState))
        {
            b = PhotonNetwork.CurrentRoom.GetRoomState();
        }

        if (b)//if scene already loaded
        {
            //load scene
            LevelLoaded(1);
        }
        else//if waiting
        {
            if (RoomNameText != null)
            {
                RoomNameText.text = PhotonNetwork.CurrentRoom.Name;
            }
            //Determine the logic 
            if (IsMaster)
            {
                MasterLogic();
            }
            else
            {
                NormalPlayerLogic();
            }
            CacheChat = ChatText;
            InvokeRepeating("InvokeList", 1, 3);
            GetStatistics();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    void GetStatistics()
    {
        if (TimeText != null)
        {
            int rt = (int)PhotonNetwork.CurrentRoom.CustomProperties[PropiertiesKeys.TimeRoomKey];
            int m_Seconds = Mathf.FloorToInt(rt % 60);
            int m_Minutes = Mathf.FloorToInt((rt / 60) % 60);
            TimeText.text = bl_CoopUtils.GetTimeFormat(m_Minutes, m_Seconds);
        }
        if (MaxPlayers != null)
        {
            MaxPlayers.text = string.Format("{0}/{1}", PhotonNetwork.PlayerList.Length, PhotonNetwork.CurrentRoom.MaxPlayers);
        }
    }

    public void ReturnToLobby() { PhotonNetwork.LeaveRoom(); }
    /// <summary>
    /// 
    /// </summary>
    public void OnLeftRoom()
    {
        this.GetComponent<bl_LobbyUI>().ChangeWindow(6);
        CancelInvoke("InvokeList");
        for (int i = 0; i < Players.Count; i++)
        {
            Destroy(Players[i].gameObject);
        }
        Players.Clear();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="otherPlayer"></param>
    public void OnPhotonPlayerDisconnected(Player otherPlayer)
    {
        Debug.Log(string.Format("Player: {0} is left of room.", otherPlayer.NickName));
        UpdatPlayerList(otherPlayer.NickName, false, true);
    }
}