using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AIVision))]
[RequireComponent(typeof(AIMovementController))]
public class PatrolController : MonoBehaviour
{
    GameMapManager gameMapManager;
    [SerializeField] EnemyTargetManager targetManager;
    Location location;

    [SerializeField] Timer patrolTimer;
    [SerializeField] float patrolRange;

    bool canPatrol = true;

    void Awake()
    {
        if (patrolTimer == null)
        {
            Debug.LogError("Patrol Timer not assign");
        }
        if (targetManager == null)
        {
            Debug.LogError("Enemy Target Manager not assign");
        }
        patrolTimer.OnTimerExpired += OnPatrolTimerExpired;
    }

    void Start()
    {
        gameMapManager = FindObjectOfType<GameMapManager>();
        location = GetComponentInParent<Location>();
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
        Transform patrolTarget;
        // choose floor to go

        if (gameMapManager.EachRoomFloors.TryGetValue(location.RoomPosition, out List<GameObject> floors))
        {
            List<GameObject> floorsChosen = floors.FindAll(floor => Vector2.Distance(floor.transform.position, transform.position) <= patrolRange);
            if (floorsChosen.Count == 0)
            {
                return;
            }
            patrolTarget = floorsChosen[Random.Range(0, floorsChosen.Count)].transform;
        }
        else
        {
            return;
        }

        targetManager.ChaseTarget(patrolTarget);
        canPatrol = false;
        patrolTimer.StartTimer();
    }

    void OnPatrolTimerExpired()
    {
        canPatrol = true;
    }
}
