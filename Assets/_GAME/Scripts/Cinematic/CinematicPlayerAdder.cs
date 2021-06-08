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
        InitSequence(0);

    }
    private void InitSequence(int i)
    {
        playabledirector.playableAsset = timelines[i];
        playabledirector.Play();
    }
}
