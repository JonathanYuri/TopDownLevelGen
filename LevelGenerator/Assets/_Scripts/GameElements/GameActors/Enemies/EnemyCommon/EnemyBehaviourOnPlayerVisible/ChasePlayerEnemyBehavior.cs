using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIPathController))]
[RequireComponent(typeof(AIVision))]
public class ChasePlayerEnemyBehavior : EnemyBehaviorOnPlayerVisible
{
    public override void Run()
    {
        TargetManager.ChasePlayer();
    }
}
