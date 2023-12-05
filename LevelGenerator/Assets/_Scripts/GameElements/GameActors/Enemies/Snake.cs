using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIMovementController))]
public class Snake : MonoBehaviour, IDamageable
{
    AIMovementController movementController;
    Timer dashTimer;

    [SerializeField] int life = 20;

    [Header("Dash")]

    bool canDashByTimerDash = true;
    bool isDashing = false;

    [SerializeField] float durationDash;
    [SerializeField] float dashVelocity;

    float velocityWithoutDash;

    void Awake()
    {
        movementController = GetComponent<AIMovementController>();
        velocityWithoutDash = movementController.Velocity;

        dashTimer = GetComponentInChildren<Timer>();
        dashTimer.OnTimerExpired += OnDashTimerExpired;
    }

    void Update()
    {
        Debug.Log(movementController.Velocity);
        TryDash();
    }

    void OnDestroy()
    {
        if (dashTimer != null)
        {
            dashTimer.OnTimerExpired -= OnDashTimerExpired;
        }
    }

    public void TakeDamage(int damage)
    {
        if (life - damage <= 0)
        {
            Die();
        }
        else
        {
            life -= damage;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void TryDash()
    {
        if (CanDash())
        {
            StartCoroutine(DashCoroutine());
        }
    }

    void OnDashTimerExpired()
    {
        canDashByTimerDash = true;
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
