using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioSource stepSound;
    public bool ShouldPlayStepSound { get; set; }

    void Update()
    {
        if (stepSound.isPlaying && !ShouldPlayStepSound)
        {
            stepSound.Stop();
        }
        if (!stepSound.isPlaying && ShouldPlayStepSound)
        {
            stepSound.Play();
        }
    }
}
