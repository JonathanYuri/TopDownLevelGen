using UnityEngine;

public class AIVision : MonoBehaviour
{
    [SerializeField] Timer memoryTimer;
    [SerializeField] float range;
    [SerializeField] float angle;
    bool forgetting;

    public bool PlayerVisible { get; set; }
    public EnemyTargetManager TargetManager { get; set; }
    public EnemyLocation Location { get; set; }
    public float Range { get => range; set => range = value; }

    void Awake()
    {
        if (memoryTimer == null)
        {
            Debug.LogError("Memory timer not assign");
        }
        memoryTimer.OnTimerExpired += OnMemoryTimerExpired;
    }

    void OnDestroy()
    {
        if (memoryTimer != null)
        {
            memoryTimer.OnTimerExpired -= OnMemoryTimerExpired;
        }
    }

    void Update()
    {
        HandleTargetVisibility();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, Range);

        Vector3 rotatedVector1 = Quaternion.Euler(0, 0, angle / 2) * transform.right;
        Vector3 rotatedVector2 = Quaternion.Euler(0, 0, -angle / 2) * transform.right;
        Gizmos.DrawLine(transform.position, transform.position + rotatedVector1 * Range);
        Gizmos.DrawLine(transform.position, transform.position + rotatedVector2 * Range);
    }

    void HandleTargetVisibility()
    {
        if (IsPlayerVisible())
        {
            HandlePlayerVisible();
        }
        else
        {
            HandlePlayerNotVisible();
        }
    }

    void HandlePlayerVisible()
    {
        PlayerVisible = true;
        forgetting = false;
        memoryTimer.StopTimer();
    }

    void HandlePlayerNotVisible()
    {
        if (PlayerVisible && !forgetting)
        {
            StartForgetting();
        }
    }

    bool IsPlayerVisible()
    {
        if (TargetManager.Player == null || TargetManager.PlayerLocation == null)
        {
            return false;
        }

        if (!Location.IsInPlayerRoom(TargetManager.PlayerLocation.RoomPosition))
        {
            return false;
        }

        if (TargetManager.PlayerLocation.JustEnteredRoom)
        {
            return false;
        }

        Vector2 toTarget = TargetManager.Player.position - transform.position;
        if (toTarget.magnitude > Range)
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
