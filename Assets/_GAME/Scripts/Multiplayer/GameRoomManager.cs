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
            
        m_player = PhotonNetwork.Instantiate(_playerPrefab.name, t, r, 0);
        
        
    }
}
