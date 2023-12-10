using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIVision : MonoBehaviour
{
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] Timer memoryTimer;

    Location myLocation;

    [SerializeField] float range;
    [SerializeField] float angle;

    [SerializeField] bool playerVisible;
    bool forgetting;

    public bool PlayerVisible { get => playerVisible; set => playerVisible = value; }

    void OnDestroy()
    {
        if (memoryTimer != null)
        {
            memoryTimer.OnTimerExpired -= OnMemoryTimerExpired;
        }
    }

    void Start()
    {
        if (memoryTimer == null)
        {
            Debug.LogError("Memory timer not assign");
        }
        memoryTimer.OnTimerExpired += OnMemoryTimerExpired;
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
        if (IsPlayerVisible())
        {
            PlayerVisible = true;
            forgetting = false;
            memoryTimer.StopTimer();
        }
        else
        {
            if (PlayerVisible && !forgetting)
            {
                StartForgetting();
            }
        }
    }

    bool IsPlayerVisible()
    {
        if (targetManager.Player == null)
        {
            return false;
        }

        if (targetManager.PlayerLocation == null)
        {
            return false;
        }

        if (!targetManager.PlayerLocation.RoomPosition.Equals(myLocation.RoomPosition))
        {
            return false;
        }

        Vector2 toTarget = targetManager.Player.position - transform.position;
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
        PlayerVisible = false;
    }
}
