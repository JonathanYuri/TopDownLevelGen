using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIMovementController))]
[RequireComponent(typeof(Seeker))]
public class AIPathController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float nextWaypointDistance = 0.2f;

    AIMovementController movementController;

    Path path;
    int currentWaypoint = 0;
    bool reachedEndOfPath;

    Seeker seeker;

    void Awake()
    {
        seeker = GetComponent<Seeker>();
        movementController = GetComponent<AIMovementController>();
        InvokeRepeating(nameof(UpdatePath), .0f, .5f);
    }

    void Update()
    {
        FollowThePath();
    }

    void UpdatePath()
    {
        if (target == null)
        {
            return;
        }

        if (seeker.IsDone())
        {
            seeker.StartPath(transform.position, target.position, OnPathComplete);
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
