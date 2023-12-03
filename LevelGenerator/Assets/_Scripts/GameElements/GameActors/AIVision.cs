using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float range;

    [SerializeField] bool targetVisible;

    void Update()
    {
        targetVisible = IsTargetVisible();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    bool IsTargetVisible()
    {
        if (target == null)
        {
            return false;
        }

        Vector2 toTarget = target.position - transform.position;
        if (toTarget.magnitude > range)
        {
            return false;
        }

        return true;
    }
}
