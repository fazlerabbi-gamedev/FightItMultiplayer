using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VivoxUnity;

public class PlayerListSpeaking : MonoBehaviour
{
    private VivoxVoiceManager _vivoxVoiceManager;
    
    public IParticipant Participant;

    public Button button;
    public Image ChatStateImage;
    public Sprite MutedImage;
    public Sprite SpeakingImage;
    public Sprite NotSpeakingImage;
    
    
    private bool isMuted;

    public bool IsMuted
    {
        get { return isMuted;}

        private set
        {
            isMuted = value;
            UpdateChatStateImage();
        }
    }
    
    
    
    private bool isSpeaking;
    public bool IsSpeaking
    {
        get { return isSpeaking; }
        private set
        {
            if  (ChatStateImage && !IsMuted)
            {
                isSpeaking = value;
                UpdateChatStateImage();
            }
        }
    }
    

    private void Start()
    {
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            Debug.Log("Add participent: " + IsMuted);
            IsMuted = !IsMuted;
        });
    }

    private void OnDisable()
    {
        button.onClick.RemoveAllListeners();
    }


    private void UpdateChatStateImage()
    {
        if (IsMuted)
        {
            ChatStateImage.sprite = MutedImage;
        }
        else
        {
            if (isSpeaking)
            {
                ChatStateImage.sprite = SpeakingImage;
            }
            else
            {
                ChatStateImage.sprite = NotSpeakingImage;
            }
        }
    }
    
    
    public void SetupPlayerItem(IParticipant participant)
    {
        _vivoxVoiceManager = VivoxVoiceManager.Instance;
        Participant = participant;
        
        IsMuted = participant.IsSelf ? _vivoxVoiceManager.AudioInputDevices.Muted : Participant.LocalMute;
        IsSpeaking = participant.SpeechDetected;
        
        
        
        Participant.PropertyChanged += (obj, args) =>
        {
            switch (args.PropertyName)
            {
                case "SpeechDetected":
                    IsSpeaking = Participant.SpeechDetected;
                    break;
            }
        };
    }
    
    
    
    
}
