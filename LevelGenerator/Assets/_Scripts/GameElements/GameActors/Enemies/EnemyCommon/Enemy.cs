using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDefeatedEventArgs
{
    public Enemy enemy;
    public Position roomPosition;
}

[RequireComponent(typeof(SpriteRenderer))]
public class Enemy : MonoBehaviour, IDamageable, IMortal
{
    [SerializeField] int life;
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] EnemyLocation location;

    [Header("Damage")]
    [SerializeField] BlinkEffect damageBlinkEffect;
    [SerializeField] float damageBlinkEffectDuration;

    public event Action OnDamageTaken;
    public EventHandler<EnemyDefeatedEventArgs> OnDefeated;

    void OnDestroy()
    {
        EnemyDefeatedEventArgs e = new()
        {
            enemy = this,
            roomPosition = location.RoomPosition,
        };
        OnDefeated?.Invoke(this, e);
    }

    public void SetTargetPlayer(Transform target, Location targetLocation)
    {
        targetManager.Player = target;
        targetManager.PlayerLocation = targetLocation;
    }

    public void TakeDamage(int damage, string damageName)
    {
        if (life - damage <= 0)
        {
            Die();
        }
        else
        {
            life -= damage;
            OnDamageTaken?.Invoke();
            damageBlinkEffect.StartEffect(damageBlinkEffectDuration);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
