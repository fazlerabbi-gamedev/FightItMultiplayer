using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startQuit : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
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
