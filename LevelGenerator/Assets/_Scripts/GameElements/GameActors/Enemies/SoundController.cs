using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct AudioSettings
{
    public SoundsName audioName;
    public AudioSource audioSource;
}

public class SoundController : MonoBehaviour
{
    [SerializeField] AudioSettings[] audioSettings;

    Dictionary<SoundsName, AudioSource> audioNameToAudioSource = new();

    void Awake()
    {
        foreach (var audio in audioSettings)
        {
            audioNameToAudioSource.Add(audio.audioName, audio.audioSource);
        }
    }

    public void PlaySound(SoundsName soundsName)
    {
        if (!audioNameToAudioSource.TryGetValue(soundsName, out var audioSource))
        {
            return;
        }
        //Debug.Log("PLAY AUDIO: " + soundsName);
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    public void StopSound(SoundsName soundsName)
    {
        if (!audioNameToAudioSource.TryGetValue(soundsName, out var audioSource))
        {
            return;
        }

        audioSource.Stop();
    }
}
