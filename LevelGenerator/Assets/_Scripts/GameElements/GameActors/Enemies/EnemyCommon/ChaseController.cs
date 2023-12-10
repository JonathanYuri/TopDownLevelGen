using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIPathController))]
[RequireComponent(typeof(AIVision))]
public class ChaseController : MonoBehaviour
{
    [SerializeField] EnemyTargetManager targetManager;

    public void TryChaseTarget()
    {
        targetManager.ChasePlayer();
    }
}
