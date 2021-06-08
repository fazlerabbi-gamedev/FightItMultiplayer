using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startQuit : MonoBehaviour
{

    public GameObject _resumeButton;
    
    
    private void Awake()
    {
        if (!PlayerPrefs.HasKey(GlobalPlayerData.pPrefs_GameSeq))
        {
            PlayerPrefs.SetInt(GlobalPlayerData.pPrefs_GameSeq, 1);
            // Off Resume Button
            _resumeButton.SetActive(false);
        }
        else
        {
            //Show Resume Button
            _resumeButton.SetActive(true);
        }
    }


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadNewGame(string sceneName)
    {
        PlayerPrefs.SetInt(GlobalPlayerData.pPrefs_GameSeq, 1);
        SceneManager.LoadScene(sceneName);
    }

    public void Play()
    {
        SceneManager.LoadScene("Player");
    }
    public void quit()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
