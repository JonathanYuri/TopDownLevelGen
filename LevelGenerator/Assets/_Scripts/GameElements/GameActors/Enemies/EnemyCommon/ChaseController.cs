using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIPathController))]
[RequireComponent(typeof(AIVision))]
public class ChaseController : MonoBehaviour
{
    public EnemyTargetManager TargetManager { get; set; }

    public void TryChaseTarget()
    {
        TargetManager.ChasePlayer();
    }
}
