using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    [SerializeField] EnemyTargetManager targetManager;

    Location myLocation;

    [SerializeField] float range;
    [SerializeField] float memoryTime;

    [SerializeField] bool targetVisible;
    Coroutine forgetEnemyCoroutine;

    public bool TargetVisible { get => targetVisible; set => targetVisible = value; }

    void Start()
    {
        myLocation = GetComponentInParent<Location>();
    }

    void Update()
    {
        if (IsTargetVisible())
        {
            if (forgetEnemyCoroutine != null)
            {
                StopCoroutine(forgetEnemyCoroutine);
            }

            TargetVisible = true;
        }
        else if (TargetVisible && (forgetEnemyCoroutine == null))
        {
            forgetEnemyCoroutine = StartCoroutine(WaitToForgetEnemy());
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    bool IsTargetVisible()
    {
        if (targetManager.Target == null)
        {
            return false;
        }

        if (targetManager.TargetLocation == null)
        {
            return false;
        }

        if (!targetManager.TargetLocation.RoomPosition.Equals(myLocation.RoomPosition))
        {
            return false;
        }

        Vector2 toTarget = targetManager.Target.position - transform.position;
        if (toTarget.magnitude > range)
        {
            return false;
        }

        return true;
    }

    IEnumerator WaitToForgetEnemy()
    {
        yield return new WaitForSeconds(memoryTime);
        TargetVisible = false;
    }
}
