using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] EnemyTargetManager targetManager;

    public void SetTargetPlayer(Transform target, Location targetLocation)
    {
        targetManager.Target = target;
        targetManager.TargetLocation = targetLocation;
    }
}
