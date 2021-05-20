using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun;

public class bl_PlayerLobby : MonoBehaviour {

    public Player m_Player;
    public Text PlayerNameText = null;
    public Text ReadyText = null;
    public GameObject isMaster = null;
    [SerializeField]private GameObject OnMaster;
    [HideInInspector]
    public string PlayerName = "";
    [HideInInspector]
    public bool Ready = false;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="n"></param>
    /// <param name="b"></param>
    public void GetInfo(string n, bool b = false)
    {
        if (PhotonNetwork.CurrentRoom == null)
        {
            Debug.Log("Not room for player list");
            Destroy(this.gameObject);
        }

        PlayerName = n;
        PlayerNameText.text = PlayerName;
        ReadyText.text = (b) ? "Ready" : "Not Ready";

        bool m = (PhotonNetwork.PlayerList.GetMasterClient().NickName == n) ? true : false;
        bool Imm = (n != PhotonNetwork.MasterClient.NickName);
        isMaster.SetActive(m);
        OnMaster.SetActive(Imm);
    }

    public void Kick()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Only Masterclient can kick players");
            return;
        }
        PhotonNetwork.CloseConnection(m_Player);
        Destroy(gameObject);
    }
}