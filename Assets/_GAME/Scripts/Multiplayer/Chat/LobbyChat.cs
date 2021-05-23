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
    [Space(10)]
    public bool isVisible = true;
    public int MaxMsn = 7;
    
    [Space(10)]
    [HideInInspector] public List<string> messages = new List<string>();
    public static readonly string ChatRPC = "Chat";
    private float m_alpha = 2f;
    private bool isChat = false;
    private string inputLine = "";
    
    
    
    /// <summary>
    /// Simple Chat
    /// </summary>
    /// <param name="msn"></param>
    // public static void SendChat(string sender, string msn)
    // {
    //     CacheChat.text += " ["+ bl_CoopUtils.CoopColorStr(sender) + "] " + msn;
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

    void Closet()
    {
        isChat = false;
        GUI.FocusControl("");
    }

    [PunRPC]
    void AddChat(string text,PhotonMessageInfo p)
    {
        m_alpha = 7;
        string senderName = "anonymous";

        if (p.Sender != null)
        {
            if (!string.IsNullOrEmpty(p.Sender.NickName))
            {
                senderName = p.Sender.NickName;
            }
            else
            {
                senderName = "player " + p.Sender.UserId;
            }
        }
        
        this.messages.Add(" (" + bl_CoopUtils.CoopColorStr(senderName) + "): " + text);
        if (messages.Count > MaxMsn)
        {
            messages.RemoveAt(0);
        }
        
        ChatText.text = "";
        foreach (string m in messages)
        {
            ChatText.text += m + "\n";
        }
        
        //ChatText.text += "\n (" + bl_CoopUtils.CoopColorStr(p.Sender.NickName) + "): " + text;
        
    }
    
    public void AddLine(string newLine)
    {
        m_alpha = 7;
        this.messages.Add(newLine);
        if (messages.Count > MaxMsn)
        {
            messages.RemoveAt(0);
        }
    }
    public void Refresh()
    {
        ChatText.text = "";
        foreach (string m in messages)
            ChatText.text += m + "\n";
    }
    
    
}
