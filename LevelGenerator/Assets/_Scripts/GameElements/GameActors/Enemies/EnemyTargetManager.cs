using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTargetManager : MonoBehaviour
{
    Transform target;
    Location targetLocation;

    public Transform Target { get => target; set => target = value; }
    public Location TargetLocation { get => targetLocation; set => targetLocation = value; }
}
