using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIMovementController))]
public class Snake : MonoBehaviour, IDamageable
{
    AIMovementController movementController;

    [SerializeField] int life = 20;

    [Header("Dash")]

    [SerializeField] float durationDash;
    [SerializeField] float reloadTimeDash;
    [SerializeField] float dashVelocity;

    float timeWithoutDash = 0f;
    float velocityWithoutDash;
    bool isDashing = false;

    void Awake()
    {
        movementController = GetComponent<AIMovementController>();
        velocityWithoutDash = movementController.Velocity;
    }

    void Update()
    {
        timeWithoutDash += Time.deltaTime;

        TryDash();
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

    bool CanDash()
    {
        return !isDashing && timeWithoutDash >= reloadTimeDash;
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
        isDashing = true;
    }

    void EndDash()
    {
        movementController.Velocity = velocityWithoutDash;
        timeWithoutDash = 0f;
        isDashing = false;
    }
}
