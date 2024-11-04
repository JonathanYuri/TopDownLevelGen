using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class GameMapSingleton : Singleton<GameMapSingleton>
{
    public Dictionary<Position, Room> RoomPositions { get; set; } = new();
    public Position InitialRoomPosition { get; set; } = new();
    public Position FinalRoomPosition { get; set; } = new();
    public Dictionary<Position, List<GameObject>> EachRoomFloors { get; set; } = new();
    public Dictionary<Position, HashSet<Door>> EachRoomDoors { get; set; } = new();
    public Dictionary<Position, HashSet<Enemy>> EachRoomEnemies { get; set; } = new();

    public void ClearMap()
    {
        RoomPositions.Clear();
        FinalRoomPosition = new();
        InitialRoomPosition = new();
        EachRoomFloors.Clear();
        EachRoomDoors.Clear();
        EachRoomEnemies.Clear();
    }

    public void ConfigureAStar()
    {
        GridGraph gridGraph = AstarPath.active.data.gridGraph;

        (int maxX, int minX) = RoomPositions.Keys.MaxAndMin(position => position.X);
        (int maxY, int minY) = RoomPositions.Keys.MaxAndMin(position => position.Y);

        maxX = Mathf.Abs(maxX);
        minX = Mathf.Abs(minX);

        maxY = Mathf.Abs(maxY);
        minY = Mathf.Abs(minY);

        int maxDistanceX = maxX >= minX ? maxX : minX;
        int maxDistanceY = maxY >= minY ? maxY : minY;

        // + 1 pq quero ter um espaco dps da ultima sala, e * 2 pq as dimensoes do gridgraph eh das arestas do retangulo
        Vector2 newGrid = Utils.TransformAMapPositionIntoAUnityPosition(new Position() { X = (maxDistanceX + 1) * 2, Y = (maxDistanceY + 1) * 2 });

        if (gridGraph != null)
        {
            gridGraph.SetDimensions((int)newGrid.x, (int)newGrid.y, 1f);
            AstarPath.active.Scan();
        }
    }

    public void SetEnemiesTargetPlayer(Transform player, Location playerLocation)
    {
        Enemy[] enemies = UnityEngine.Object.FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            enemy.SetTargetPlayer(player, playerLocation);
        }
    }
}
