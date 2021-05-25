using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class NetworkPlayer : MonoBehaviour, IPunObservable
{
    private PlayerMovement _playerMovement;
    private PhotonView _phView;
    private Rigidbody rb;
    
    public float SmoothingDelay = 5;
    
    private Vector3 m_Position = Vector3.zero;
    private Quaternion m_Rotation = Quaternion.identity;

    private void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Destroy(this);
        }
        
        _playerMovement = GetComponent<PlayerMovement>();
        _phView = GetComponent<PhotonView>();
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        if (!_phView.IsMine)
        {
            m_Position = transform.position;
            m_Rotation = transform.rotation;
        }
    }


    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            if (_phView.IsMine)
            {
                stream.SendNext(transform.position);
                stream.SendNext(transform.rotation);
                stream.SendNext(rb.velocity);
            }
            
            
        }
        else
        {
            if (!_phView.IsMine)
            {
                this.m_Position = (Vector3)stream.ReceiveNext();
                this.m_Rotation = (Quaternion)stream.ReceiveNext();
                rb.velocity = (Vector3)stream.ReceiveNext();
                
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.timestamp));
                this.m_Position += (rb.velocity * lag);
                
                if (Vector3.Distance(transform.position, m_Position) > 20.0f) // more or less a replacement for CheckExitScreen function on remote clients
                {
                    transform.position = m_Position;
                }
            }
            

        }
    }
    
    
    private void FixedUpdate()
    {
        if (_phView == null || PhotonNetwork.IsConnected == false)
            return;

        if (!_phView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, m_Position, Time.fixedDeltaTime);
            transform.rotation = Quaternion.Lerp(transform.rotation, m_Rotation, Time.fixedDeltaTime);
        }
    }
    
    
}
