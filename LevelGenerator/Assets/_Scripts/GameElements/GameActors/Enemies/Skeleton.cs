using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skeleton : MonoBehaviour, IDamageable
{
    [SerializeField] int life = 20;

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
