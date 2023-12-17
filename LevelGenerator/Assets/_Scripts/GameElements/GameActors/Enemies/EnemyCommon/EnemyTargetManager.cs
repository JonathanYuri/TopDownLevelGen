using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetManager : MonoBehaviour
{
    public Transform Player { get; set; }
    public Location PlayerLocation { get; set; }
    public Transform Target { get; set; }

    public void ChasePlayer()
    {
        ChaseTarget(Player);
    }

    public void ChaseTarget(Transform target)
    {
        this.Target = target;
    }
}
