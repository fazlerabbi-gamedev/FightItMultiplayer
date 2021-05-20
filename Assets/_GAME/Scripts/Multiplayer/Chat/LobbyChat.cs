using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Hashtable = ExitGames.Client.Photon.Hashtable; //Replace default Hashtables with Photon hashtables
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client;
using TMPro;

public class LobbyChat : bl_PhotonHelper
{
    [Header("Chat")]
    public TMP_Text ChatText = null;
    public static Text CacheChat = null;
    
    
    
    
    
    
    /// <summary>
    /// Simple Chat
    /// </summary>
    /// <param name="msn"></param>
    // public static void SendChat(string sender, string msn)
    // {
    //     CacheChat.text += "\n ["+ bl_CoopUtils.CoopColorStr(sender) + "] " + msn;
    // }
    /// <summary>
    /// Add text sync
    /// </summary>
    /// <param name="i"></param>
    public void NewChatMsn(TMP_InputField i)
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
    
    
    
    
}
