using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSelectMP : MonoBehaviour
{


    public void SelectChar(GameObject player)
    {
        GlobalPlayerData.Player1Prefab = player;
        Launcher.Instance.ShowStartButton();
    }
}
