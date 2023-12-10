using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PatrolController))]
[RequireComponent(typeof(ChaseController))]
[RequireComponent(typeof(AIVision))]
public class EnemyStateController : MonoBehaviour
{
    PatrolController patrolController;
    ChaseController chaseController;
    AIVision aiVision;

    void Awake()
    {
        patrolController = GetComponent<PatrolController>();
        chaseController = GetComponent<ChaseController>();
        aiVision = GetComponent<AIVision>();
    }

    void Update()
    {
        if (aiVision.PlayerVisible)
        {
            chaseController.TryChaseTarget();
        }
        else
        {
            patrolController.TryPatrol();
        }
    }
}
