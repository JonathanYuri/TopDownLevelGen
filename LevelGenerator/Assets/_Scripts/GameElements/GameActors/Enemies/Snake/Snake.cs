using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snake : Enemy, IDamageable
{
    DashController dashController;
    [SerializeField] AIVision aiVision;

    [SerializeField] int life = 20;

    void Awake()
    {
        dashController = GetComponentInChildren<DashController>();
    }

    void Start()
    {
        if (aiVision == null)
        {
            Debug.LogError("AIVision not assign");
        }
    }

    void Update()
    {
        if (aiVision.PlayerVisible)
        {
            dashController.TryDash();
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
}
