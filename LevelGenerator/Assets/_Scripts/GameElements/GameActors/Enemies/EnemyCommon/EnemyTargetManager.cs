using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetManager : MonoBehaviour
{
    Transform player;
    Transform target;
    Location playerLocation;

    public Transform Player { get => player; set => player = value; }
    public Location PlayerLocation { get => playerLocation; set => playerLocation = value; }
    public Transform Target { get => target; set => target = value; }

    public void ChasePlayer()
    {
        Target = player;
    }

    public void ChaseTarget(Transform target)
    {
        this.Target = target;
    }
}
