using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected AIPathController aiPathController;
    protected AIVision aiVision;

    public void SetTargetPlayer(Transform target, Location targetLocation)
    {
        aiPathController.Target = target;
        aiVision.Target = target;
        aiVision.TargetLocation = targetLocation;
    }
}
