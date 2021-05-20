using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  Sirenix.OdinInspector;
using TMPro;
using UnityEngine.UI;

public class UIMultiplayerManager : MonoBehaviour
{
    #region Singleton

    public static UIMultiplayerManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    #endregion

    
    [TabGroup("Connection")] 
    public TextMeshProUGUI _connectionText;
    [TabGroup("Connection")] 
    public GameObject _connectionOkBtn;
    
    [TabGroup("UI Menu")]
    public UI_MultiplayerScreen[] UIMenus;
    
    [TabGroup("Login and Reg")] 
    public TMP_InputField _username;
    [TabGroup("Login and Reg")] 
    public TMP_InputField _password;
    [TabGroup("Login and Reg")] 
    public TMP_InputField _regEmail;
    [TabGroup("Login and Reg")] 
    public Button _loginButton;
    [TabGroup("Login and Reg")] 
    public Button _regButton;
    [TabGroup("Login and Reg")] 
    public GameObject _loadingIcon;



    private string _connectPanel = "Connecting";
    private string _loginPanel = "Login";
    private string _regPanel = "Register";
    private string _characterSelection = "CharacterSelection";
    


     #region Init

    private void Start()
    {
        MDisableAllScreens();
        Init();

        PlayfabAuthManager.OnAfterLogin += AfterLogin;
    }

    private void Init()
    {

        ShowMMenu(_connectPanel);
        
        _loadingIcon.SetActive(false);
        
        ConnectionOkButton(false);
        
        //Login Button
        
        _loginButton.onClick.AddListener(() =>
        {
            PlayfabAuthManager.Instance._Username = _username;
            PlayfabAuthManager.Instance._Password = _password;
            PlayfabAuthManager.Instance.PlayfabAuthOnClick();

            OnLoginButtonClick();
        });
        
        _regButton.onClick.AddListener(() =>
        {
            PlayfabAuthManager.Instance.AuthWithPlayfabRegister(_regEmail.text);
            ShowMMenu(_loginPanel);
        });
        
    }

    #endregion

    #region Connection OK Button
    public void ConnectionOkButton(bool var)
    {
        switch (var)
        {
            case true:
                _connectionOkBtn.SetActive(true);
                break;
            case false:
                _connectionOkBtn.SetActive(false);
                break;
        }
        
    }

    public void ConnectionOkButtonClick()
    {
        ShowMMenu(_loginPanel);
    }

    #endregion

    #region Login Button

    public void OnLoginButtonClick()
    {
        _loginButton.gameObject.SetActive(false);
        _loadingIcon.SetActive(true);
    }

    public void ShowRegPanel()
    {
        ShowMMenu(_regPanel);
    }

    #endregion

    #region UI Menus

    public void ShowMMenu(string name)
    {
        ShowMenuFunc(name, true);
    }
    
    
    private void ShowMenuFunc(string name, bool disableAllScreens){
        if(disableAllScreens) MDisableAllScreens();

        foreach (UI_MultiplayerScreen UI in UIMenus){
            if (UI.UI_Name == name) {
                
                if (UI.UI_Gameobject != null) {
                    UI.UI_Gameobject.SetActive(true);
                    

                } else {
                    Debug.Log ("no menu found with name: " + name);
                }
            }
        }

        //fadeIn
        // if (UI_fader != null) UI_fader.gameObject.SetActive (true);
        // UI_fader.Fade (UIFader.FADE.FadeIn, .5f, .3f);
    }
    
    public void CloseMenu(string name){
        foreach (UI_MultiplayerScreen UI in UIMenus){
            if (UI.UI_Name == name)	UI.UI_Gameobject.SetActive (false);
        }
    }
    
    public void MDisableAllScreens(){
        foreach (UI_MultiplayerScreen UI in UIMenus){ 
            if(UI.UI_Gameobject != null) 
                UI.UI_Gameobject.SetActive(false);
            else 
                Debug.Log("Null ref found in UI with name: " + UI.UI_Name);
        }
    }

    #endregion



    public void AfterLogin()
    {
        ShowMMenu(_characterSelection);
    }
    
    
    
}


[System.Serializable]
public class UI_MultiplayerScreen 
{
    public string UI_Name;
    public GameObject UI_Gameobject;
}