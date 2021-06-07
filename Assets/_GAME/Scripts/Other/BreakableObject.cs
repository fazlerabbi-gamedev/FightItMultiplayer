﻿using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BreakableObject : MonoBehaviour, IDamagable<DamageObject> {

	public string hitSFX = "";

	[Header ("Gameobject Destroyed")]
	public GameObject destroyedGO;
	public bool orientToImpactDir;

	[Header ("Spawn an item")]
	public GameObject spawnItem;
	public float spawnChance = 100;

	[Space(10)]
	public bool destroyOnHit;

	void Start(){
		gameObject.layer = LayerMask.NameToLayer("DestroyableObject");
	}

	//this object was Hit
	public void Hit(DamageObject DO)
	{
		//play hit sfx
		if (hitSFX != "") 
		{
			GlobalAudioPlayer.PlaySFXAtPosition (hitSFX, transform.position);
		}

		//spawn destroyed gameobject version
		if (destroyedGO != null) 
		{
			GameObject BrokenGO = null;
			//	GameObject BrokenGO = GameObject.Instantiate (destroyedGO);
			if (GameStateManager.Instance._gameState == GameState.Singleplayer)
            {
				BrokenGO = GameObject.Instantiate(destroyedGO);
            }
            else
            {
				BrokenGO = PhotonNetwork.Instantiate(destroyedGO.name, transform.position, Quaternion.identity);
			}
				
			//BrokenGO.transform.position = transform.position;

			//chance direction based on the impact direction
			if (orientToImpactDir && DO.inflictor != null) {
				float dir = Mathf.Sign(DO.inflictor.transform.position.x - transform.position.x);
				BrokenGO.transform.rotation = Quaternion.LookRotation(Vector3.forward * dir);
			}
		}

		//spawn an item
		if (spawnItem != null) 
		{
			if (Random.Range (0, 100) < spawnChance) {
				// GameObject item = GameObject.Instantiate (spawnItem);
				// item.transform.position = transform.position;
				GameObject item = PhotonNetwork.Instantiate(spawnItem.name, transform.position, Quaternion.identity);

				//add up force to object
				//item.GetComponent<Rigidbody>().velocity = Vector3.up * 8f;
			}
		}
		//destroy 
		if (destroyOnHit) 
		{
			if (GameStateManager.Instance._gameState == GameState.Singleplayer)
            {
				Destroy(this.gameObject);
			}
            else
            {
				PhotonNetwork.Destroy(this.gameObject);
			}
				
		}
	}
	
	
}