using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene()
    {
        string scene = string.Empty;

        if (PlayerPrefs.GetInt(GlobalPlayerData.pPrefs_GameSeq, 0) == 1)
        {
            scene = "Bar_Scene";
        }
        if (PlayerPrefs.GetInt(GlobalPlayerData.pPrefs_GameSeq, 0) == 2)
        {
            scene = "Warehouse";
        }
        if (PlayerPrefs.GetInt(GlobalPlayerData.pPrefs_GameSeq, 0) == 3)
        {
            scene = "JungleScene";
        }

        if (!string.IsNullOrEmpty(scene))
        {
            SceneManager.LoadScene(scene);
        }
         

    }
}
