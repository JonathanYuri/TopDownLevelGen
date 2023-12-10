using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AIVision))]
[RequireComponent(typeof(AIMovementController))]
public class PatrolController : MonoBehaviour
{
    GameMapManager gameMapManager;
    EnemyTargetManager targetManager;

    EnemyLocation location;

    [SerializeField] Timer patrolTimer;
    [SerializeField] float patrolRange;

    bool canPatrol = true;

    public EnemyTargetManager TargetManager { get => targetManager; set => targetManager = value; }
    public EnemyLocation Location { get => location; set => location = value; }

    void Awake()
    {
        if (patrolTimer == null)
        {
            Debug.LogError("Patrol Timer not assign");
        }
        patrolTimer.OnTimerExpired += OnPatrolTimerExpired;
    }

    void Start()
    {
        gameMapManager = FindObjectOfType<GameMapManager>();
    }

    public void TryPatrol()
    {
        if (canPatrol)
        {
            Patrol();
        }
    }

    void Patrol()
    {
        canPatrol = false;
        patrolTimer.StartTimer();

        Transform patrolTarget = ChooseFloorToGo();
        if (patrolTarget != null)
        {
            TargetManager.ChaseTarget(patrolTarget);
        }
    }

    Transform ChooseFloorToGo()
    {
        if (!gameMapManager.EachRoomFloors.TryGetValue(Location.RoomPosition, out List<GameObject> floors))
        {
            return null;
        }

        List<GameObject> floorsChosen = floors.FindAll(floor => Vector2.Distance(floor.transform.position, transform.position) <= patrolRange);
        if (floorsChosen.Count == 0)
        {
            return null;
        }
        return floorsChosen[Random.Range(0, floorsChosen.Count)].transform;
    }

    void OnPatrolTimerExpired()
    {
        canPatrol = true;
    }
}
