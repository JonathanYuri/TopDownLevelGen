using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIMovementController))]
[RequireComponent(typeof(Seeker))]
public class AIPathController : MonoBehaviour
{
    [SerializeField] Timer updatePathTimer;
    [SerializeField] float nextWaypointDistance = 0.2f;

    AIMovementController movementController;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath;

    Seeker seeker;

    public EnemyTargetManager TargetManager { get; set; }

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        movementController = GetComponent<AIMovementController>();

        if (updatePathTimer == null)
        {
            Debug.LogError("Update Path Timer not assign");
        }

        updatePathTimer.OnTimerExpired += OnUpdatePathTimerExpired;
        updatePathTimer.StartTimer();

        UpdatePath();
    }

    void Update()
    {
        FollowThePath();
    }

    void OnDestroy()
    {
        if (updatePathTimer != null)
        {
            updatePathTimer.OnTimerExpired -= OnUpdatePathTimerExpired;
        }
    }

    void OnUpdatePathTimerExpired()
    {
        UpdatePath();
    }

    void UpdatePath()
    {
        updatePathTimer.StartTimer();

        if (TargetManager == null || TargetManager.Target == null)
        {
            return;
        }

        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, TargetManager.Target.position, OnPathComplete);
        }
    }

    void FollowThePath()
    {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Move();
    }

    void Move()
    {
        Vector2 currentWaypointPosition = path.vectorPath[currentWaypoint];
        movementController.SetDestination(currentWaypointPosition);
        Debug.DrawLine(transform.position, currentWaypointPosition, Color.red);

        float distanceToWaypoint = Vector2.Distance(transform.position, currentWaypointPosition);
        if (distanceToWaypoint < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
