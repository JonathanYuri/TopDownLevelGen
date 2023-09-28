using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomObjectSpawner : MonoBehaviour
{
    [SerializeField] GameObject inimigo;
    [SerializeField] GameObject obstaculo;

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
            { new Position { X = 0, Y = GameConstants.Height - 1 }, paredes[(int)CornerIndex.TopLeftCorner] },
            { new Position { X = GameConstants.Width - 1, Y = GameConstants.Height - 1 }, paredes[(int)CornerIndex.TopRightCorner] },
            { new Position { X = 0, Y = 0 }, paredes[(int)CornerIndex.BottomLeftCorner] },
            { new Position { X = GameConstants.Width - 1, Y = 0 }, paredes[(int)CornerIndex.BottomRightCorner] },
        };
    }

    public void SpawnRoomObjects(Room sala, GameObject room)
    {
        for (int i = 0; i < sala.Width; i++)
        {
            for (int j = 0; j < sala.Height; j++)
            {
                //Debug.LogWarning($"i: {i}, j: {j}: {sala.Values[i, j]}");
                Position position = new() { X = i, Y = j };
                GameObject tile = SelectTheRightObjectsToSpawnInPosition(sala, position);

                Instantiate(tile, (Vector2)tile.transform.position + new Vector2(i, j) + (Vector2)room.transform.position, tile.transform.rotation, room.transform);
            }
        }
    }

    GameObject SelectTheRightObjectsToSpawnInPosition(Room sala, Position position)
    {
        GameObject tile = chao;
        if (objects.TryGetValue(sala.Values[position.X, position.Y], out GameObject gameObject))
        {
            tile = gameObject;
        }
        else if (sala.Values[position.X, position.Y] == RoomContents.Door)
        {
            tile = SelectTheRightPositionDoor(position);
        }
        else if (sala.Values[position.X, position.Y] == RoomContents.Wall)
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
        foreach (var kvp in GameConstants.NeighboorDirectionToDoorPosition)
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
        else if (position.X == GameConstants.Width - 1)
        {
            return paredes[(int)WallIndex.Right];
        }
        else if (position.Y == 0)
        {
            return paredes[(int)WallIndex.Bottom];
        }
        else if (position.Y == GameConstants.Height - 1)
        {
            return paredes[(int)WallIndex.Top];
        }
        return null;
    }
}
