using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIVision))]
[RequireComponent(typeof(Seeker))]
public class AIPathController : MonoBehaviour
{
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] float nextWaypointDistance = 0.2f;

    AIMovementController movementController;
    AIVision aiVision;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath;

    Seeker seeker;

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        aiVision = GetComponent<AIVision>();
        movementController = GetComponentInChildren<AIMovementController>();
        InvokeRepeating(nameof(UpdatePath), .0f, .5f);
    }

    void Update()
    {
        if (aiVision.TargetVisible)
        {
            FollowThePath();
        }
    }

    void UpdatePath()
    {
        if (targetManager.Target == null)
        {
            return;
        }

        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, targetManager.Target.position, OnPathComplete);
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

        // Move
        Vector3 currentWaypointPosition = path.vectorPath[currentWaypoint];
        movementController.SetMovement(currentWaypointPosition);
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
