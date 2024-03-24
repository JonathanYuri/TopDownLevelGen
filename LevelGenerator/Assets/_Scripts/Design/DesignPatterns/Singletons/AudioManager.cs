using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : SingletonMonoBehaviour<AudioManager>
{
    [SerializeField] CharacterSoundsController characterSoundsController;

    [SerializeField] List<GameObject> sounds;
    [SerializeField] List<string> soundsName;

    Dictionary<string, GameObject> objectSounds;

    protected override void Awake()
    {
        base.Awake();
        objectSounds = new();
        for (int i = 0; i < sounds.Count; i++)
        {
            objectSounds.Add(soundsName[i], sounds[i]);
        }
    }

    public void AddCharacterSoundInstance(string characterSoundName, int gameObjectId)
    {
        if (!objectSounds.ContainsKey(characterSoundName))
        {
            return;
        }

        GameObject characterSoundGO = Instantiate(objectSounds[characterSoundName], characterSoundsController.transform);
        characterSoundsController.AddCharacterSound(characterSoundGO.GetComponent<CharacterSounds>(), gameObjectId);
    }

    public void RemoveCharacterSoundInstance(int gameObjectId)
    {
        GameObject characterSoundGO = characterSoundsController.RemoveCharacterSound(gameObjectId);
        Destroy(characterSoundGO);
    }

    public void PlayCharacterSound(string soundName, int gameObjectId)
    {
        characterSoundsController.PlaySoundFromObject(soundName, gameObjectId);
    }

    public void StopCharacterSound(string soundName, int gameObjectId)
    {
        characterSoundsController.StopSoundFromObject(soundName, gameObjectId);
    }
}
