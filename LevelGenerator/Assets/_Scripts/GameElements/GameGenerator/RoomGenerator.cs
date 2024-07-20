using System.Collections;
using UnityEngine;
using RoomGeneticAlgorithm.Run;
using SpawnRoomObjects.SpawnAll;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(LevelGenerator))]
[RequireComponent(typeof(RoomObjectSpawner))]
public class RoomGenerator : MonoBehaviour
{
    LevelGenerator levelGenerator;
    RoomObjectSpawner roomObjectSpawner;
    RoomInfoProvider roomInfoProvider;

    [SerializeField] GameObject roomPrefab;

    void Awake()
    {
        levelGenerator = GetComponent<LevelGenerator>();
        roomObjectSpawner = GetComponent<RoomObjectSpawner>();
        roomInfoProvider = GetComponent<RoomInfoProvider>();
    }

    /// <summary>
    /// Generates a room object at the specified position, with optional content generation.
    /// </summary>
    /// <param name="roomPosition">The position where the room object will be generated.</param>
    /// <returns>GameObject of the room</returns>
    GameObject GenerateRoomGameObject(Position roomPosition)
    {
        Vector2 roomObjectPosition = Utils.TransformAMapPositionIntoAUnityPosition(roomPosition);
        GameObject roomObject = Instantiate(roomPrefab, roomObjectPosition, Quaternion.identity, levelGenerator.Rooms);
        roomObject.GetComponent<Location>().RoomPosition = roomPosition;
        return roomObject;
    }

    /// <summary>
    /// Generates a room within the specified room object and position.
    /// </summary>
    /// <param name="roomPosition">The position of the room to be generated.</param>
    /// <param name="generateObjectsInRoom">Determines whether to generate objects in the room (default: true).</param>
    /// <returns>An IEnumerator for asynchronous room generation.</returns>
    public IEnumerator GenerateRoom(Position roomPosition, HashSet<Position> map, bool generateObjectsInRoom = true)
    {
        GameObject roomObject = GenerateRoomGameObject(roomPosition);
        Room room = roomObject.GetComponent<Room>();

        RoomData roomData = roomInfoProvider.GetRoomData(roomPosition, map);
        RoomSkeleton roomSkeleton = new(roomData);

        room.Difficulty = roomData.difficulty;

        if (generateObjectsInRoom)
        {
            room.Enemies = roomData.enemies.ToList();
            room.Obstacles = roomData.obstacles.ToList();

            float startTime = Time.realtimeSinceStartup;
            GeneticRoomGenerator geneticRoomGenerator = new();
            yield return geneticRoomGenerator.GeneticLooping(roomSkeleton);

            roomSkeleton.Values = geneticRoomGenerator.Best.RoomMatrix.Values;

            float endTime = Time.realtimeSinceStartup;

            Debug.LogError("Tempo de execução da corrotina: " + (endTime - startTime) + " segundos");
            Debug.LogError("Tempo total de execução ate agora: " + endTime + " segundos");
        }

        if (roomInfoProvider.IsFinalRoom(roomPosition))
        {
            roomSkeleton.Values[GameConstants.ROOM_MIDDLE.X, GameConstants.ROOM_MIDDLE.Y] = RoomContents.LevelEnd;
        }

        GameMapSingleton.Instance.RoomPositions.Add(roomPosition, room);

        yield return roomObjectSpawner.SpawnRoomObjects(roomSkeleton, roomPosition, roomObject);
    }
}
