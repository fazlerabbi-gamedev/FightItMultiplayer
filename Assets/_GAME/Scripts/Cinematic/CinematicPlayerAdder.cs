using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class CinematicPlayerAdder : MonoBehaviour
{
    public List<TimelineAsset> timelines;

    private PlayableDirector playabledirector;

    private void Start()
    {
        playabledirector = GetComponent<PlayableDirector>();
        InitSequence(PlayerPrefs.GetInt(GlobalPlayerData.pPrefs_GameSeq, 0));

    }
    private void InitSequence(int i)
    {
        
        if (i == 1)
        {
            playabledirector.playableAsset = timelines[i];
            playabledirector.Play();
        }
        if (i == 2)
        {
            playabledirector.playableAsset = timelines[i];
            playabledirector.Play();
        }
        if (i == 3)
        {
            playabledirector.playableAsset = timelines[i];
            playabledirector.Play();
        }
    }
}
