using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaintainMaxRangeDistanceEnemyBehavior : EnemyBehaviorOnPlayerVisible
{
    [SerializeField] EnemyLocation enemyLocation;
    [SerializeField] AIVision aiVision;

    public override void Run()
    {
        Transform chosenFloor = ChooseFloorToGo();
        if (chosenFloor != null)
        {
            TargetManager.ChaseTarget(chosenFloor);
        }
    }

    Transform ChooseFloorToGo()
    {
        if (!GameMapSingleton.Instance.EachRoomFloors.TryGetValue(enemyLocation.RoomPosition, out List<GameObject> floors))
        {
            return null;
        }

        if (TargetManager.Player == null)
        {
            return null;
        }

        Vector2 toTarget = TargetManager.Player.position - transform.position;
        if (toTarget.magnitude <= aiVision.Range)
        {
            return TryMoveAwayFromPlayer(floors, toTarget);
        }
        else
        {
            return TryMoveTowardPlayer(floors);
        }
    }

    Transform TryMoveAwayFromPlayer(List<GameObject> floors, Vector2 toTarget)
    {
        // pega a posicao no meu range que me deixa ver o player ainda (maior distancia)
        var orderedFloors = floors
            .Where(floor => Vector2.Distance(floor.transform.position, transform.position) <= aiVision.Range - toTarget.magnitude)
            .OrderByDescending(floor => Vector2.Distance(floor.transform.position, transform.position))
            .ToList();

        return orderedFloors.Count > 0 ? orderedFloors[0].transform : null;
    }

    Transform TryMoveTowardPlayer(List<GameObject> floors)
    {
        // pega a posicao no meu range que me deixa mais perto do player
        var orderedFloors = floors
            .Where(floor => Vector2.Distance(floor.transform.position, transform.position) <= aiVision.Range)
            .OrderBy(floor => Vector2.Distance(floor.transform.position, TargetManager.Player.position))
            .ToList();

        return orderedFloors.Count > 0 ? orderedFloors[0].transform : null;
    }
}
