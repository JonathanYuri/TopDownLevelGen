using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMapSingleton : Singleton<GameMapSingleton>
{
    public HashSet<Position> RoomPositions { get; set; } = new();
    public Dictionary<Position, List<GameObject>> EachRoomFloors { get; set; } = new();

    public void ConfigureAStar()
    {
        GridGraph gridGraph = AstarPath.active.data.gridGraph;

        (int maxX, int minX) = RoomPositions.MaxAndMin(position => position.X);
        (int maxY, int minY) = RoomPositions.MaxAndMin(position => position.Y);

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
        Enemy[] enemies = Object.FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            enemy.SetTargetPlayer(player, playerLocation);
        }
    }
}
