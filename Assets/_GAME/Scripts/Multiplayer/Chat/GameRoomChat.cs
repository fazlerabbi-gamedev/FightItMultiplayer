using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;


public class GameRoomChat : bl_PhotonHelper
{
    public GUISkin m_Skin;
    [Space(5)]
    public TMP_Text ChatText;
    public bool IsVisible = true;
    public int MaxMsn = 7;
    
    [Space(10)]
    [HideInInspector] public List<string> messages = new List<string>();
    public static readonly string ChatRPC = "Chat";
    private float m_alpha = 2f;
    private bool isChat = false;
    private string inputLine = "";


    // public void OnGUI()
    // {
    //     if (ChatText == null)
    //         return;
    //     
    //     if (m_alpha > 0.0f && !isChat)
    //     {
    //         m_alpha -= Time.deltaTime / 2;
    //     }
    //     else if (isChat)
    //     {
    //         m_alpha = 10;
    //     }
    //     
    //     
    //     GUI.skin = m_Skin;     
    //     if (!this.IsVisible || !PhotonNetwork.InRoom)
    //     {
    //         return;
    //     }
    //     
    //     if (!string.IsNullOrEmpty(this.inputLine) && isChat && bl_CoopUtils.GetCursorState)
    //     {
    //         this.photonView.RPC("Chat", RpcTarget.All, this.inputLine);
    //         this.inputLine = "";
    //         GUI.FocusControl("");
    //         isChat = false;
    //         return; // printing the now modified list would result in an error. to avoid this, we just skip this single frame
    //     }
    //     else if (!isChat && bl_CoopUtils.GetCursorState)
    //     {
    //         GUI.FocusControl("ChatInput");
    //         isChat = true;
    //     }
    //     else
    //     {
    //         if (isChat)
    //         {
    //             Closet();
    //         }
    //     }
    //     
    //     
    //     
    //     GUI.color = new Color(1, 1, 1, 1);
    //     GUI.SetNextControlName("");
    //     GUILayout.BeginArea(new Rect(Screen.width / 2 - 150, Screen.height - 35, 600, 1200));
    //     GUILayout.Height(1200);
    //     GUILayout.BeginHorizontal();
    //     GUI.SetNextControlName("ChatInput");
    //     inputLine = GUILayout.TextField(inputLine);
    //     if (GUILayout.Button("Send", "box", GUILayout.ExpandWidth(false)))
    //     {
    //         this.photonView.RPC("Chat", RpcTarget.All, this.inputLine);
    //         this.inputLine = "";
    //         GUI.FocusControl("");
    //     }
    //     GUILayout.EndHorizontal();
    //     GUILayout.EndArea();
    //
    // }
    
    public void ChatMsn(TMP_InputField i)
    {
        string t = i.text;
        photonView.RPC("Chat", RpcTarget.All, t);
        i.text = string.Empty;
    }
    
    
    
    void Closet()
    {
        isChat = false;
        GUI.FocusControl("");
    }
    
    
    [PunRPC]
    public void Chat(string newLine, PhotonMessageInfo mi)
    {
        m_alpha = 7;
        string senderName = "anonymous";

        if (mi.Sender != null)
        {
            if (!string.IsNullOrEmpty(mi.Sender.NickName))
            {
                senderName = mi.Sender.NickName;
            }
            else
            {
                senderName = "player " + mi.Sender.UserId;
            }
        }

        this.messages.Add("<color=#478FF5>[" + senderName + "]</color>: " + newLine);
        if (messages.Count > MaxMsn)
        {
            messages.RemoveAt(0);
        }

        ChatText.text = "";
        foreach (string m in messages)
        {
            ChatText.text += m + "\n";
        }
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
