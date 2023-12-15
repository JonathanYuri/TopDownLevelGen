using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour, IDamageable, IMortal
{
    [SerializeField] int life;
    [SerializeField] EnemyTargetManager targetManager;

    public event Action OnDamageTaken;

    public void SetTargetPlayer(Transform target, Location targetLocation)
    {
        targetManager.Player = target;
        targetManager.PlayerLocation = targetLocation;
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
            OnDamageTaken?.Invoke();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
