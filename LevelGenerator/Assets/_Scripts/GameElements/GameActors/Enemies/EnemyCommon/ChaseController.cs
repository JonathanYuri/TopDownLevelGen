using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIPathController))]
[RequireComponent(typeof(AIVision))]
public class ChaseController : MonoBehaviour
{
    EnemyTargetManager targetManager;

    public EnemyTargetManager TargetManager { get => targetManager; set => targetManager = value; }

    public void TryChaseTarget()
    {
        TargetManager.ChasePlayer();
    }
}
