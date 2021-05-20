﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

public class bl_GameController : bl_PhotonHelper {

    public static bool isPlaying = false;
    public static int m_ViewID = -1;
    [HideInInspector] public GameObject m_Player { get; set; }

    /// <summary>
    /// Player to instantiate, this need stay in "Resources" folder.
    /// </summary>
    public GameObject PlayerPrefab = null;
    /// <summary>
    /// Scene To return when left of room or disconnect
    /// </summary>
    public string ReturnScene;
    /// <summary>
    /// All SpawnPoint where player can appear in scene / map.
    /// </summary>
    private static List<bl_SpawnPoint> SpawnPoint = new List<bl_SpawnPoint>();
    public SpawnType m_SpawnType = SpawnType.Random;
    public bool InstantSpawn = false; //Spawn right after load scene
    [Space(7)]
    public GameObject RoomCamera = null;
  
    /// <summary>
    /// 
    /// </summary>
    void Awake()
    {
        //When start in this scene, but is not connect, return to lobby to connect
        if (!isConnected || PhotonNetwork.CurrentRoom == null)
        {
            bl_UtilityHelper.LoadLevel(0);
            return;
        }
        StartCoroutine(bl_UIManager.Instance.FadeIn(1.5f));  
        //Available to receive messages from cloud.
        PhotonNetwork.IsMessageQueueRunning = true;
        //Inform to Lobby information, that this scene is playing
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.SendRoomState(true);
        }
        if (InstantSpawn)
        {
            Invoke("SpawnPlayer", 2);
        }
        else
        {
             bl_UIManager.Instance.StartWindow.SetActive(true);
        }
        //Send a new log information
        string logText = LocalName + " joined to the game";
        bl_LogInfo inf = new bl_LogInfo(logText, Color.green);
        bl_EventHandler.OnLogMsnEvent(inf);
    }

    private void OnEnable()
    {
        bl_PhotonCallbacks.Instance.OnLeft+=(OnLeftRoom);
        bl_PhotonCallbacks.Instance.OnPlayerLeft+=(OnPhotonPlayerDisconnected);
        bl_PhotonCallbacks.Instance.OnMasterClientSwitchedEvent+=(OnMasterClientSwitched);
    }

    /// <summary>
    /// 
    /// </summary>
    void OnDisable()
    {
        isPlaying = false;
        bl_PhotonCallbacks.Instance.OnLeft-=(OnLeftRoom);
        bl_PhotonCallbacks.Instance.OnPlayerLeft-=(OnPhotonPlayerDisconnected);
        bl_PhotonCallbacks.Instance.OnMasterClientSwitchedEvent-=(OnMasterClientSwitched);
    }

    /// <summary>
    /// 
    /// </summary>
    public void SpawnPlayer()
    {
        if (PlayerPrefab == null)
        {
            Debug.Log("Player Prefabs I was not assigned yet!");
            return;
        }
        //Sure of have just only player on room
        if (m_Player != null)
        {
            NetworkDestroy(m_Player);
        }

        Vector3 p = Vector3.zero;
        Quaternion r = Quaternion.identity;
        //Get Position and rotation from a spawnPoint
        GetSpawnPoint(out p, out r);

        m_Player = PhotonNetwork.Instantiate(PlayerPrefab.name,p,r, 0);
        m_ViewID = m_Player.GetViewID();
        bl_EventHandler.OnLocalPlayerSpawn(m_Player);
        if (RoomCamera != null)
        {
            RoomCamera.SetActive(false);
        }
        bl_CoopUtils.LockCursor(true);
        isPlaying = true;

    }

    /// <summary>
    /// Get the spawnPoint
    /// </summary>
    private int currentSpawnPoint = 0;
    private void GetSpawnPoint(out Vector3 position, out Quaternion rotation)
    {
            if (SpawnPoint.Count <= 0)
            {
                Debug.LogWarning("Doesn't have spawn point in scene");
                position = Vector3.zero;
                rotation = Quaternion.identity;
            }

            if (m_SpawnType == SpawnType.Random)
            {

                int random = Random.Range(0, SpawnPoint.Count);
                Vector3 s = Random.insideUnitSphere * SpawnPoint[random].SpawnRadius;
                Vector3 pos = SpawnPoint[random].transform.position + new Vector3(s.x, 0, s.z);

                position = pos;
                rotation = SpawnPoint[random].transform.rotation;
            }
            else if (m_SpawnType == SpawnType.RoundRobin)
            {
                if (currentSpawnPoint >= SpawnPoint.Count) { currentSpawnPoint = 0; }
                Vector3 s = Random.insideUnitSphere * SpawnPoint[currentSpawnPoint].SpawnRadius;
                Vector3 v = SpawnPoint[currentSpawnPoint].transform.position + new Vector3(s.x, 0, s.z);
                currentSpawnPoint++;

                position = v;
                rotation = SpawnPoint[currentSpawnPoint].transform.rotation;
            }
            else
            {
                position = Vector3.zero;
                rotation = Quaternion.identity;
            }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    public static void RegisterSpawnPoint(bl_SpawnPoint point)
    {
        SpawnPoint.Add(point);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="point"></param>
    public static void UnRegisterSpawnPoint(bl_SpawnPoint point)
    {
        SpawnPoint.Remove(point);
    }

 
    /// <summary>
    /// 
    /// </summary>
    public void OnLeftRoom()
    {
        if(m_Player != null)
        {  
            PhotonNetwork.Destroy(m_Player);
        }
        bl_CoopUtils.LoadLevel(ReturnScene);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="otherPlayer"></param>
    public void OnPhotonPlayerDisconnected(Player otherPlayer)
    {
        //Send a new log information
        string logText = otherPlayer.NickName + " Left of room";
        //Send this as local because this function is already call in other players in room.
        bl_LogInfo inf = new bl_LogInfo(logText, Color.red,true);
        bl_EventHandler.OnLogMsnEvent(inf);
        //Debug.Log(otherPlayer.name + " Left of room");
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.DestroyPlayerObjects(otherPlayer);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="newMasterClient"></param>
    public void OnMasterClientSwitched(Player newMasterClient)
    {
        //Send a new log information
        string logText = "We have a new Master Client: " + newMasterClient.NickName;
        //Send this as local because this function is already call in other players in room.
        bl_LogInfo inf = new bl_LogInfo(logText, Color.yellow, true);
        bl_EventHandler.OnLogMsnEvent(inf);
    }

    [System.Serializable]
    public enum SpawnType
    {
        Random,
        RoundRobin,
    }
}