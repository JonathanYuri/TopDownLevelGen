using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioSource> sounds;
    [SerializeField] List<string> soundsName;

    public Dictionary<string, float> SoundsDuration { get; private set; }
    public Dictionary<string, bool> ShouldPlaySound { get; set; }
    public Dictionary<string, bool> SoundPlayingState { get; private set; }

    void Awake()
    {
        SoundsDuration = new();
        ShouldPlaySound = new();
        SoundPlayingState = new();

        for (int i = 0; i < soundsName.Count; i++)
        {
            SoundsDuration.Add(soundsName[i], sounds[i].clip.length);
            ShouldPlaySound.Add(soundsName[i], false);
            SoundPlayingState.Add(soundsName[i], false);
        }
    }

    void Update()
    {
        for (int i = 0; i < sounds.Count; i++)
        {
            if (sounds[i].isPlaying && !ShouldPlaySound[soundsName[i]])
            {
                sounds[i].Stop();
            }
            if (!sounds[i].isPlaying && ShouldPlaySound[soundsName[i]])
            {
                sounds[i].Play();
            }

            SoundPlayingState[soundsName[i]] = sounds[i].isPlaying;
        }
    }
}
