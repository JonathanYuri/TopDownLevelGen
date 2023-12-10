using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SpawnRoomObjects.SpawnAll;
using System;
using Random = UnityEngine.Random;

namespace SpawnRoomObjects.SpawnFloor
{
    [RequireComponent(typeof(RoomObjectSpawner))]
    public class FloorSpawner : MonoBehaviour
    {
        [SerializeField] GameObject floor;
        SpriteRenderer floorSpriteRenderer;

        [Header("Spawn Possibities")]

        [SerializeField] List<Sprite> spritesFloor;
        [SerializeField] List<float> spriteFloorProbabilities;

        List<float> spriteFloorTransformedProbabilities;

        RoomObjectSpawner roomObjectSpawner;

        void Awake()
        {
            roomObjectSpawner = GetComponent<RoomObjectSpawner>();
            floorSpriteRenderer = floor.GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            spriteFloorTransformedProbabilities = new();
            float prob = 0;
            for (int i = 0; i < spriteFloorProbabilities.Count; i++)
            {
                prob += spriteFloorProbabilities[i];
                spriteFloorTransformedProbabilities.Add(prob);
            }

            if (prob != 1)
            {
                Debug.LogError("A soma das probabilidades do chao deve ser 1");
            }
        }

        Sprite ChooseFloorSprite()
        {
            float value = Random.value;
            for (int i = 0; i < spriteFloorTransformedProbabilities.Count; i++)
            {
                if (spriteFloorTransformedProbabilities[i] >= value)
                {
                    return spritesFloor[i];
                }
            }
            return spritesFloor[^1];
        }

        internal Dictionary<Position, GameObject> SpawnAllFloors(GameObject roomObject)
        {
            Dictionary<Position, GameObject> allFloorsPlaced = new();
            for (int i = 1; i < GameConstants.ROOM_WIDTH - 1; i++)
            {
                for (int j = 1; j < GameConstants.ROOM_HEIGHT - 1; j++)
                {
                    floorSpriteRenderer.sprite = ChooseFloorSprite();
                    GameObject floorPlaced = roomObjectSpawner.InstantiateRoomContentObject(floor, roomObject, new Position() { X = i, Y = j });
                    allFloorsPlaced.Add(new Position() { X = i, Y = j }, floorPlaced);
                }
            }
            return allFloorsPlaced;
        }
    }
}