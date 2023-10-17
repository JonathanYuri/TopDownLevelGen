using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject inimigo;
    [SerializeField] GameObject obstaculo;
    [SerializeField] GameObject portal;

    [SerializeField] GameObject[] portas;
    [SerializeField] GameObject[] paredes;

    [SerializeField] GameObject chao;

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

    private void Awake()
    {
        objects = new()
        {
            { RoomContents.Ground, chao },
            { RoomContents.Nothing, chao },

            { RoomContents.Obstacle1, obstaculo },
            { RoomContents.Obstacle2, obstaculo },
            { RoomContents.Obstacle3, obstaculo },

            { RoomContents.Enemy1, inimigo },
            { RoomContents.Enemy2, inimigo },
            { RoomContents.Enemy3, inimigo },

            { RoomContents.Portal, portal },
        };

        directionOfDoorsToGameObject = new()
        {
            { Direction.Up, portas[(int)DoorIndex.Up] },
            { Direction.Down, portas[(int)DoorIndex.Down] },
            { Direction.Left, portas[(int)DoorIndex.Left] },
            { Direction.Right, portas[(int)DoorIndex.Right] },
        };

        cornerPositionToGameObject = new()
        {
            { new Position { X = 0, Y = GameConstants.ROOM_HEIGHT - 1 }, paredes[(int)CornerIndex.TopLeftCorner] },
            { new Position { X = GameConstants.ROOM_WIDTH - 1, Y = GameConstants.ROOM_HEIGHT - 1 }, paredes[(int)CornerIndex.TopRightCorner] },
            { new Position { X = 0, Y = 0 }, paredes[(int)CornerIndex.BottomLeftCorner] },
            { new Position { X = GameConstants.ROOM_WIDTH - 1, Y = 0 }, paredes[(int)CornerIndex.BottomRightCorner] },
        };
    }

    public void SpawnRoomObjects(Room room, GameObject roomObject)
    {
        for (int i = 0; i < GameConstants.ROOM_WIDTH; i++)
        {
            for (int j = 0; j < GameConstants.ROOM_HEIGHT; j++)
            {
                //Debug.LogWarning($"i: {i}, j: {j}: {room.Values[i, j]}");
                Position position = new() { X = i, Y = j };
                GameObject tile = SelectTheRightObjectsToSpawnInPosition(room, position);

                // new Vector2(-GameConstants.ROOM_MIDDLE.X, -GameConstants.ROOM_MIDDLE.Y) pra colocar o (0, 0) que eh o canto inferior esquerdo para a coordenada -7, -4
                // new Vector2(i, j) para colocar todos os objetos nas posicoes certas da matriz
                // (Vector2)roomObject.transform.position para colocar nas salas certas
                Instantiate(tile, new Vector2(-GameConstants.ROOM_MIDDLE.X, -GameConstants.ROOM_MIDDLE.Y) + new Vector2(i, j) + (Vector2)roomObject.transform.position, tile.transform.rotation, roomObject.transform);
            }
        }
    }

    GameObject SelectTheRightObjectsToSpawnInPosition(Room room, Position position)
    {
        GameObject tile = chao;
        if (objects.TryGetValue(room.Values[position.X, position.Y], out GameObject gameObject))
        {
            tile = gameObject;
        }
        else if (room.Values[position.X, position.Y] == RoomContents.Door)
        {
            tile = SelectTheRightPositionDoor(position);
        }
        else if (room.Values[position.X, position.Y] == RoomContents.Wall)
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

    GameObject SelectTheRightPositionDoor(Position position)
    {
        // pegar todas as posicoes das portas e ver se eh igual a que estou agora
        foreach (var kvp in GameConstants.NEIGHBOR_DIRECTION_TO_DOOR_POSITION)
        {
            if (kvp.Value.Equals(position))
            {
                return directionOfDoorsToGameObject[kvp.Key];
            }
        }
        return null;
    }

    GameObject SelectTheRightPositionCorner(Position position)
    {
        if (cornerPositionToGameObject.TryGetValue(position, out GameObject corner))
        {
            return corner;
        }
        return null;
    }

    GameObject SelectTheRightPositionWall(Position position)
    {
        if (position.X == 0)
        {
            return paredes[(int)WallIndex.Left];
        }
        else if (position.X == GameConstants.ROOM_WIDTH - 1)
        {
            return paredes[(int)WallIndex.Right];
        }
        else if (position.Y == 0)
        {
            return paredes[(int)WallIndex.Bottom];
        }
        else if (position.Y == GameConstants.ROOM_HEIGHT - 1)
        {
            return paredes[(int)WallIndex.Top];
        }
        return null;
    }
}
