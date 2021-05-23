using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text playercountText;
    public RoomInfo info;
    
    public void SetUp(RoomInfo _info)
    {
        info = _info;
        text.text = _info.Name;
        GetPlayerInfo(_info);
    }
    
    public void OnClick()
    {
        Launcher.Instance.JoinRoom(info);
    }

    public void GetPlayerInfo(RoomInfo r)
    {
        playercountText.text = r.PlayerCount + " / " + r.MaxPlayers;
    }
}
