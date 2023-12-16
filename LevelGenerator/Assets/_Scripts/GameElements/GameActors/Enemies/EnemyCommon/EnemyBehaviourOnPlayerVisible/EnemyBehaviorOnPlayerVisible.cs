using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBehaviorOnPlayerVisible : MonoBehaviour
{
    public EnemyTargetManager TargetManager { get; set; }

    public abstract void Run();
}
