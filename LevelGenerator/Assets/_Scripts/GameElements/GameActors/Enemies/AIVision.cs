using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float range;
    [SerializeField] float memoryTime;

    [SerializeField] bool targetVisible;
    bool forgetting = false;

    void Update()
    {
        if (IsTargetVisible())
        {
            if (forgetting)
            {
                StopCoroutine(WaitToForgetEnemy());
            }

            forgetting = false;
            targetVisible = true;
        }
        else if (targetVisible && !forgetting)
        {
            forgetting = true;
            StartCoroutine(WaitToForgetEnemy());
        }
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

    IEnumerator WaitToForgetEnemy()
    {
        forgetting = true;
        yield return new WaitForSeconds(memoryTime);

        targetVisible = false;

        forgetting = false;
    }
}
