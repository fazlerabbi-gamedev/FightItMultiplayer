using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MPCharacterSelectUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    [Header("Player Info :")] 
    public GameObject playerPrefab;
    public Sprite playerHUD;
    
    [Header("Player Select HUD :")] 
    public Image Border;
    public Color BorderColorDefault;
    public Color BorderColorOver;
    public Color BorderColorHighlight;
    public bool Selected;


    private void Awake()
    {
        Selected = false;
    }
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        Select();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Deselect();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClick();
    }
    
    
    public void Select(){
        if(Border && !Selected) Border.color = BorderColorOver;
    }

    //deselect
    public void Deselect(){
        if(Border && !Selected) Border.color = BorderColorDefault;
    }


    public void OnClick()
    {
        ResetAllButtons();
        Selected = true;
        if(Border) Border.color = BorderColorHighlight;

        CharacterSelectMP chSelect = GameObject.FindObjectOfType<CharacterSelectMP>();
        chSelect.SelectChar(playerPrefab);

    }
    
    
    
    public void ResetAllButtons(){
        MPCharacterSelectUI[] allButtons = GameObject.FindObjectsOfType<MPCharacterSelectUI>();
        foreach(MPCharacterSelectUI button in allButtons) { 
            button.Border.color = button.BorderColorDefault;
            button.Selected = false;
        }
    }
}
