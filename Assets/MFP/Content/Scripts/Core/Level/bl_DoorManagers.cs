using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(PhotonView))]
public class bl_DoorManagers : bl_PhotonHelper
{
    public List<bl_SimpleDoor> MapDoors = new List<bl_SimpleDoor>();
    public List<int> DoorsStates = new List<int>();

    void Awake()
    {
        for(int i = 0; i < MapDoors.Count; i++) { DoorsStates.Add(0); }
    }

    private void OnEnable()
    {
        bl_PhotonCallbacks.Instance.OnPlayerEnter+=(OnPhotonPlayerConnected);
    }

    private void OnDisable()
    {
        bl_PhotonCallbacks.Instance.OnPlayerEnter-=(OnPhotonPlayerConnected);
    }

    public void ChangeDoorState(bl_SimpleDoor door, int state)
    {
        if (MapDoors.Contains(door))
        {
            int index = MapDoors.FindIndex(x => x == door);
            DoorsStates[index] = state;
            //we don't need buffer the states
            photonView.RPC("RpcDoorState", RpcTarget.Others, index, state);
        }
        else
        {
            Debug.LogWarning("Door is not register in this map door list.");
        }
    }

    [PunRPC]
    void RpcDoorState(int doorId, int state)
    {
        DoorsStates[doorId] = state;
        MapDoors[doorId].ApplyState(state);
    }

    [PunRPC]
    void RpcAllDoorState(int[] state)
    {
        for (int i = 0; i < state.Length; i++)
        {
            DoorsStates[i] = state[i];
            MapDoors[i].ApplyStateInsta(state[i]);
        }
    }

    public void OnPhotonPlayerConnected(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            //send the door states to the new player
            photonView.RPC("RpcAllDoorState", newPlayer, DoorsStates.ToArray());
        }
    }
}