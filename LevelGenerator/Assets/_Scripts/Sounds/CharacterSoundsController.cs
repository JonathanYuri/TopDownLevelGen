using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundsController : MonoBehaviour
{
    Dictionary<int, CharacterSounds> gameObjectsSounds;

    void Awake()
    {
        gameObjectsSounds = new();
    }

    public void AddCharacterSound(CharacterSounds characterSounds, int gameObjectId) => gameObjectsSounds.Add(gameObjectId, characterSounds);
    public GameObject RemoveCharacterSound(int gameObjectId)
    {
        if (!gameObjectsSounds.ContainsKey(gameObjectId))
        {
            return null;
        }

        CharacterSounds characterSound = gameObjectsSounds[gameObjectId];
        gameObjectsSounds.Remove(gameObjectId);

        if (characterSound == null)
        {
            return null;
        }
        return characterSound.gameObject;
    }

    public void PlaySoundFromObject(string soundName, int gameObjectId)
    {
        if (!gameObjectsSounds.ContainsKey(gameObjectId))
        {
            return;
        }

        CharacterSounds characterSounds = gameObjectsSounds[gameObjectId];
        characterSounds.PlaySound(soundName);
    }

    public void StopSoundFromObject(string soundName, int gameObjectId)
    {
        if (!gameObjectsSounds.ContainsKey(gameObjectId))
        {
            return;
        }

        CharacterSounds characterSounds = gameObjectsSounds[gameObjectId];
        characterSounds.StopSound(soundName);
    }
}
