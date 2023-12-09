using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    [SerializeField] EnemyTargetManager targetManager;
    Timer memoryTimer;

    Location myLocation;

    [SerializeField] float range;
    [SerializeField] float angle;

    [SerializeField] bool targetVisible;
    bool forgetting;

    public bool TargetVisible { get => targetVisible; set => targetVisible = value; }

    void Awake()
    {
        memoryTimer = GetComponentInChildren<Timer>();
        memoryTimer.OnTimerExpired += OnMemoryTimerExpired;
    }

    void OnDestroy()
    {
        if (memoryTimer != null)
        {
            memoryTimer.OnTimerExpired -= OnMemoryTimerExpired;
        }
    }

    void Start()
    {
        myLocation = GetComponentInParent<Location>();
    }

    void Update()
    {
        HandleTargetVisibility();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);

        Vector3 rotatedVector1 = Quaternion.Euler(0, 0, angle / 2) * transform.right;
        Vector3 rotatedVector2 = Quaternion.Euler(0, 0, -angle / 2) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + rotatedVector1 * range);
        Gizmos.DrawLine(transform.position, transform.position + rotatedVector2 * range);
    }

    void HandleTargetVisibility()
    {
        if (IsTargetVisible())
        {
            TargetVisible = true;
            forgetting = false;
            memoryTimer.StopTimer();
        }
        else
        {
            if (TargetVisible && !forgetting)
            {
                StartForgetting();
            }
        }
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

        if (Vector2.Angle(transform.right, toTarget) > angle / 2)
        {
            return false;
        }

        return true;
    }

    void StartForgetting()
    {
        forgetting = true;
        memoryTimer.StartTimer();
    }

    void OnMemoryTimerExpired()
    {
        TargetVisible = false;
    }
}
