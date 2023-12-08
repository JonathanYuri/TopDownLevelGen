using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class DashController : MonoBehaviour
{
    [SerializeField] AIMovementController movementController;
    Timer dashTimer;

    bool canDashByTimerDash = true;
    bool isDashing = false;

    [SerializeField] float durationDash;
    [SerializeField] float dashVelocity;

    float velocityWithoutDash;

    void Start()
    {
        if (movementController == null)
        {
            Debug.LogError("Movement controller not assign");
        }
        velocityWithoutDash = movementController.Velocity;

        dashTimer = GetComponent<Timer>();
        dashTimer.OnTimerExpired += OnDashTimerExpired;
    }

    void OnDestroy()
    {
        if (dashTimer != null)
        {
            dashTimer.OnTimerExpired -= OnDashTimerExpired;
        }
    }

    void OnDashTimerExpired()
    {
        canDashByTimerDash = true;
    }

    public void TryDash()
    {
        if (CanDash())
        {
            StartCoroutine(DashCoroutine());
        }
    }

    bool CanDash()
    {
        return !isDashing && canDashByTimerDash;
    }

    IEnumerator DashCoroutine()
    {
        StartDash();
        yield return new WaitForSeconds(durationDash);
        EndDash();
    }

    void StartDash()
    {
        movementController.Velocity = dashVelocity;
        canDashByTimerDash = false;
        isDashing = true;
    }

    void EndDash()
    {
        movementController.Velocity = velocityWithoutDash;
        isDashing = false;
        dashTimer.StartTimer();
    }
}
