using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasePlayerEnemyBehavior : EnemyBehaviorOnPlayerVisible
{
    public override void Run()
    {
        TargetManager.ChasePlayer();
    }
}
