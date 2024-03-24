using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSounds : MonoBehaviour
{
    [SerializeField] List<AudioSource> audioSources;
    [SerializeField] List<string> soundsName;

    public void PlaySound(string sound)
    {
        for (int i = 0; i < soundsName.Count; i++)
        {
            if (soundsName[i] == sound)
            {
                if (audioSources[i].isPlaying)
                {
                    return;
                }
                audioSources[i].Play();
                return;
            }
        }
    }

    public void StopSound(string sound)
    {
        for (int i = 0; i < soundsName.Count; i++)
        {
            if (soundsName[i] == sound)
            {
                audioSources[i].Stop();
                return;
            }
        }
    }
}
