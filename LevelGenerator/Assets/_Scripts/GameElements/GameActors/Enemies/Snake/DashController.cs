using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashController : MonoBehaviour
{
    [SerializeField] AIMovementController movementController;
    [SerializeField] Timer dashDuration;
    [SerializeField] Timer dashCooldown;

    bool dashReady = true;
    bool isDashing = false;

    [SerializeField] float dashVelocity;

    float velocityWithoutDash;

    void Awake()
    {
        if (movementController == null)
        {
            Debug.LogError("Movement controller not assign");
        }
        if (dashDuration == null)
        {
            Debug.LogError("Dash duration not assign");
        }
        if (dashCooldown == null)
        {
            Debug.LogError("Dash cooldown not assign");
        }
    }

    void Start()
    {
        velocityWithoutDash = movementController.Velocity;

        dashDuration.OnTimerExpired += OnDashDurationTimerExpired;
        dashCooldown.OnTimerExpired += OnDashCooldownTimerExpired;
    }

    void OnDestroy()
    {
        if (dashDuration != null)
        {
            dashDuration.OnTimerExpired -= OnDashDurationTimerExpired;
        }
        if (dashCooldown != null)
        {
            dashCooldown.OnTimerExpired -= OnDashCooldownTimerExpired;
        }
    }

    void OnDashDurationTimerExpired()
    {
        EndDash();
    }

    void OnDashCooldownTimerExpired()
    {
        dashReady = true;
    }

    public void TryDash()
    {
        if (CanDash())
        {
            StartDash();
            dashDuration.StartTimer();
        }
    }

    bool CanDash()
    {
        return !isDashing && dashReady;
    }

    void StartDash()
    {
        movementController.Velocity = dashVelocity;
        dashReady = false;
        isDashing = true;
    }

    void EndDash()
    {
        movementController.Velocity = velocityWithoutDash;
        isDashing = false;
        dashCooldown.StartTimer();
    }
}
