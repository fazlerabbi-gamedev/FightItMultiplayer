using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerSpawnPoint : MonoBehaviour
{

	public static PlayerSpawnPoint Instance;
	public GameObject defaultPlayerPrefab;
	
	public static int m_ViewID = -1;
	[HideInInspector] public GameObject m_Player { get; set; }
	
	public GameObject[] SpawnPoint;

	void Awake(){

		//get selected player from character selection screen
		if(GlobalPlayerData.Player1Prefab) {
			loadPlayer(GlobalPlayerData.Player1Prefab);
			return;
		}	

		//otherwise load default character
		if(defaultPlayerPrefab) {
			loadPlayer(defaultPlayerPrefab);
		} else {
			Debug.Log("Please assign a default player prefab in the  playerSpawnPoint");
		}
	}

	//load a player prefab
	void loadPlayer(GameObject playerPrefab)
	{
		if (defaultPlayerPrefab == null)
		{
			Debug.Log("Player Prefabs I was not assigned yet!");
			return;
		}

		if (PhotonNetwork.IsConnected)
		{
			// Vector3 p = Vector3.zero;
			// Quaternion r = Quaternion.identity;
			//GetSpawnPoint(out p, out r);
			Vector3 p = SpawnPoint[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.position;
			Quaternion r = SpawnPoint[PhotonNetwork.LocalPlayer.ActorNumber - 1].transform.rotation;
		
			m_Player = PhotonNetwork.Instantiate(playerPrefab.name,p,r, 0);
			m_ViewID = m_Player.GetViewID();
		}
		
		
		
		// GameObject player = GameObject.Instantiate(playerPrefab) as GameObject;
		// player.transform.position = transform.position;
	}
	
	
	
	
	// private int currentSpawnPoint = 0;
	// private void GetSpawnPoint(out Vector3 position, out Quaternion rotation)
	// {
	// 	// if (SpawnPoint.Count < 0)
	// 	// {
	// 	// 	Debug.LogWarning("Doesn't have spawn point in scene");
	// 	// 	position = Vector3.zero;
	// 	// 	rotation = Quaternion.identity;
	// 	// }
	// 	// else
	// 	// {
	// 	//
	// 	// 	int random = Random.Range(0, SpawnPoint.Count);
	// 	// 	Vector3 s = Random.insideUnitSphere * SpawnPoint[random].SpawnRadius;
	// 	// 	Vector3 pos = SpawnPoint[random].transform.position + new Vector3(s.x, 0, s.z);
	// 	//
	// 	// 	position = pos;
	// 	// 	rotation = SpawnPoint[random].transform.rotation;
	// 	// }
	// 	
	// 	
	//
	// }
	
}