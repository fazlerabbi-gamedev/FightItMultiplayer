using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PlayerSync : MonoBehaviourPun, IPunObservable
{

    private UNITSTATE m_movementUnitState;
    private PlayerMovement _playerMovement;


    private void Start()
    {
        _playerMovement = this.GetComponent<PlayerMovement>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        
    }
    
}
