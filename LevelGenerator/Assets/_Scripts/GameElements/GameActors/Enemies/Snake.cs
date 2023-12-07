using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Enemy, IDamageable
{
    DashController dashController;

    [SerializeField] int life = 20;

    void Awake()
    {
        dashController = GetComponentInChildren<DashController>();
        aiPathController = GetComponentInChildren<AIPathController>();
        aiVision = GetComponentInChildren<AIVision>();
    }

    void Update()
    {
        // dashController.TryDash();
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
}
