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

    bool destroying;

    void Awake()
    {
        foreach (var audio in audioSettings)
        {
            audioNameToAudioSource.Add(audio.audioName, audio.audioSource);
        }
    }

    void OnDestroy()
    {
        destroying = true;
    }

    public void PlaySound(SoundsName soundsName)
    {
        if (!audioNameToAudioSource.TryGetValue(soundsName, out var audioSource))
        {
            return;
        }

        if (destroying)
        {
            return;
        }

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

        if (destroying)
        {
            return;
        }

        audioSource.Stop();
    }
}
