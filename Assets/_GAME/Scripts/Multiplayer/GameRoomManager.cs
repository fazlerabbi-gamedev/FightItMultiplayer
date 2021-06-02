using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;
using System.IO;

public class GameRoomManager : MonoBehaviourPunCallbacks
{
    public static GameRoomManager Instance;

    public GameObject _playerPrefab;
    public GameObject[] SpawnPoint;
    public GameObject m_player { get; set; }
    Vector3 t;
    Quaternion r;

    private string playerName;
    void Awake()
    {
        Instance = this;
        // if(Instance)
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        // DontDestroyOnLoad(gameObject);
        // Instance = this;
        
        SpawnPlayer();
    }
    
    
    // void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    // {
    //     
    //     if(scene.buildIndex == 2) // We're in the game scene
    //     {
    //
    //     }
    // }

    void SpawnPlayer()
    {
        
        t = SpawnPoint[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position;
        r = Quaternion.identity;
        if (GlobalPlayerData.Player1Prefab != null)
        {
            playerName = GlobalPlayerData.Player1Prefab.name;
        }
        else
        {
            playerName = _playerPrefab.name;
        }
        m_player = PhotonNetwork.Instantiate(playerName, t, r, 0);
        
        
    }
}
