using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : Enemy, IDamageable
{
    [SerializeField] int life = 20;

    void Awake()
    {
        aiPathController = GetComponentInChildren<AIPathController>();
        aiVision = GetComponentInChildren<AIVision>();
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
