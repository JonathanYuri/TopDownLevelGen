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

    bool isDashing = false;

    void Awake()
    {
        movementController = GetComponent<AIMovementController>();
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
            StartCoroutine(Dash());
        }
    }

    bool CanDash()
    {
        return !isDashing && timeWithoutDash >= reloadTimeDash;
    }

    IEnumerator Dash()
    {
        float normalVelocity = movementController.Velocity;
        movementController.Velocity = dashVelocity;
        isDashing = true;

        yield return new WaitForSeconds(durationDash);

        movementController.Velocity = normalVelocity;
        timeWithoutDash = 0f;
        isDashing = false;
    }
}
