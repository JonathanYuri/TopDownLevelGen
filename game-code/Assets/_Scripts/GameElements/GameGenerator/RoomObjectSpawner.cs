using SpawnRoomObjects.SpawnFloor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SpawnRoomObjects.SpawnAll
{
    [RequireComponent(typeof(FloorSpawner))]
    /// <summary>
    /// Responsible for creating and positioning objects in the room environment, such as enemies, obstacles, and doors.
    /// </summary>
    public class RoomObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject[] enemies;
        [SerializeField] GameObject[] obstacles;

        [SerializeField] GameObject levelEnd;

        [SerializeField] GameObject[] doors;
        [SerializeField] GameObject[] walls;

        // Enums to indicate the position of each element in the array of walls
        enum CornerIndex
        {
            TopLeftCorner = 0,
            TopRightCorner = 1,
            BottomLeftCorner = 2,
            BottomRightCorner = 3,
        }
        enum WallIndex
        {
            Top = 4,
            Bottom = 5,
            Left = 6,
            Right = 7,
        }

        // Enum to indicate the position of each element in the array of doors
        enum DoorIndex
        {
            Up = 0,
            Down = 1,
            Left = 2,
            Right = 3,
        }

        Dictionary<RoomContents, GameObject> objects;
        Dictionary<Direction, GameObject> directionOfDoorsToGameObject;
        Dictionary<Position, GameObject> cornerPositionToGameObject;

        FloorSpawner floorSpawner;

        void Awake()
        {
            floorSpawner = GetComponent<FloorSpawner>();

            objects = new()
            {
                { RoomContents.Obstacle1, obstacles[0] },
                { RoomContents.Obstacle2, obstacles[1] },

                { RoomContents.Enemy1, enemies[0] },
                { RoomContents.Enemy2, enemies[1] },
                { RoomContents.Enemy3, enemies[2] },

                { RoomContents.LevelEnd, levelEnd },
            };

            directionOfDoorsToGameObject = new()
            {
                { Direction.Up, doors[(int)DoorIndex.Up] },
                { Direction.Down, doors[(int)DoorIndex.Down] },
                { Direction.Left, doors[(int)DoorIndex.Left] },
                { Direction.Right, doors[(int)DoorIndex.Right] },
            };

            cornerPositionToGameObject = new()
            {
                { new Position { X = 0, Y = GameConstants.ROOM_HEIGHT - 1 }, walls[(int)CornerIndex.TopLeftCorner] },
                { new Position { X = GameConstants.ROOM_WIDTH - 1, Y = GameConstants.ROOM_HEIGHT - 1 }, walls[(int)CornerIndex.TopRightCorner] },
                { new Position { X = 0, Y = 0 }, walls[(int)CornerIndex.BottomLeftCorner] },
                { new Position { X = GameConstants.ROOM_WIDTH - 1, Y = 0 }, walls[(int)CornerIndex.BottomRightCorner] },
            };
        }

        /// <summary>
        /// Spawns room objects within the given room.
        /// </summary>
        /// <param name="room">The room containing information about the objects to spawn.</param>
        /// <param name="roomObject">The GameObject representing the room where objects should be spawned.</param>
        public IEnumerator SpawnRoomObjects(RoomContents[,] room, Position roomPosition, GameObject roomObject)
        {
            StartCoroutine(floorSpawner.SpawnAllFloors(room, roomObject, roomPosition));

            for (int i = 0; i < GameConstants.ROOM_WIDTH; i++)
            {
                for (int j = 0; j < GameConstants.ROOM_HEIGHT; j++)
                {
                    Position positionRoomContent = new() { X = i, Y = j };
                    GameObject tilePrefab = SelectTheRightObjectsToSpawnInPosition(room[i, j], positionRoomContent);

                    if (tilePrefab == null)
                    {
                        continue;
                    }

                    InstantiateRoomContentObject(tilePrefab, roomObject, positionRoomContent, roomPosition);
                    // se chegou a instanciar o objeto nao eh um chao
                    floorSpawner.allFloorsPlaced.Remove(positionRoomContent);
                }
                yield return null;
            }

            GameMapSingleton.Instance.EachRoomFloors.Add(roomPosition, floorSpawner.allFloorsPlaced.Values.ToList());
        }

        internal GameObject InstantiateRoomContentObject(GameObject tilePrefab, GameObject roomObject, Position positionRoomContent, Position roomPosition)
        {
            GameObject tileResult = Instantiate(tilePrefab, roomObject.transform);
            tileResult.transform.rotation = tilePrefab.transform.rotation;
            tileResult.transform.localPosition =
                new Vector2(-GameConstants.ROOM_MIDDLE.X, -GameConstants.ROOM_MIDDLE.Y) +
                new Vector2(positionRoomContent.X, positionRoomContent.Y);

            // new Vector2(-GameConstants.ROOM_MIDDLE.X, -GameConstants.ROOM_MIDDLE.Y) pra colocar o (0, 0) que eh o canto inferior esquerdo para a coordenada -7, -4
            // new Vector2(positionRoomContent.X, positionRoomContent.Y); para colocar todos os objetos nas posicoes certas da matriz

            if (tileResult.TryGetComponent(out Enemy enemy))
            {
                if (!GameMapSingleton.Instance.EachRoomEnemies.ContainsKey(roomPosition))
                {
                    GameMapSingleton.Instance.EachRoomEnemies[roomPosition] = new();
                }
                GameMapSingleton.Instance.EachRoomEnemies[roomPosition].Add(enemy);
            }

            else if (tileResult.TryGetComponent(out Door door))
            {
                if (!GameMapSingleton.Instance.EachRoomDoors.ContainsKey(roomPosition))
                {
                    GameMapSingleton.Instance.EachRoomDoors[roomPosition] = new();
                }
                GameMapSingleton.Instance.EachRoomDoors[roomPosition].Add(door);
            }

            return tileResult;
        }

        /// <summary>
        /// Selects the appropriate GameObject to spawn at the given position within a room.
        /// </summary>
        /// <param name="room">The room containing information about objects.</param>
        /// <param name="position">The position within the room to select the object for.</param>
        /// <returns>The GameObject to be spawned at the specified position.</returns>
        GameObject SelectTheRightObjectsToSpawnInPosition(RoomContents content, Position position)
        {
            GameObject tile = null;
            if (objects.TryGetValue(content, out GameObject gameObject))
            {
                tile = gameObject;
            }
            else if (content == RoomContents.Door)
            {
                tile = SelectTheRightPositionDoor(position);
            }
            else if (content == RoomContents.Wall)
            {
                GameObject corner = SelectTheRightPositionCorner(position);
                tile = corner switch
                {
                    null => SelectTheRightPositionWall(position),
                    _ => corner,
                };
            }
            return tile;
        }

        /// <summary>
        /// Selects the appropriate door GameObject to spawn at the given position within a room.
        /// </summary>
        /// <param name="position">The position within the room to select the door for.</param>
        /// <returns>The door GameObject to be spawned at the specified position, or null if there's no door at that position.</returns>
        GameObject SelectTheRightPositionDoor(Position position)
        {
            if (GameConstants.DOOR_POSITION_TO_NEIGHBOR_DIRECTION.TryGetValue(position, out Direction doorDirection))
            {
                return directionOfDoorsToGameObject[doorDirection];
            }
            return null;
        }

        /// <summary>
        /// Selects the appropriate corner GameObject to spawn at the given position within a room.
        /// </summary>
        /// <param name="position">The position within the room to select the corner for.</param>
        /// <returns>The corner GameObject to be spawned at the specified position, or null if there's no corner at that position.</returns>
        GameObject SelectTheRightPositionCorner(Position position) => cornerPositionToGameObject.TryGetValue(position, out GameObject corner) ? corner : null;

        /// <summary>
        /// Selects the appropriate wall GameObject to spawn at the given position within a room.
        /// </summary>
        /// <param name="position">The position within the room to select the wall for.</param>
        /// <returns>The wall GameObject to be spawned at the specified position, or null if there's no wall at that position.</returns>
        GameObject SelectTheRightPositionWall(Position position)
        {
            if (position.X == 0)
            {
                return walls[(int)WallIndex.Left];
            }
            else if (position.X == GameConstants.ROOM_WIDTH - 1)
            {
                return walls[(int)WallIndex.Right];
            }
            else if (position.Y == 0)
            {
                return walls[(int)WallIndex.Bottom];
            }
            else if (position.Y == GameConstants.ROOM_HEIGHT - 1)
            {
                return walls[(int)WallIndex.Top];
            }
            return null;
        }
    }

}