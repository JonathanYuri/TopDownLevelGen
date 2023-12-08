using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Timer))]
[RequireComponent(typeof(Collider2D))]
public class TriggerDamage : CollisionEffects
{
    [SerializeField] int damage;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var _))
        {
            isColliding = true;
            objectInCollision = collision.gameObject;

            ApplyEffect();
            effectTimer.StartTimer();
        }
        InvokeCollisionEvent();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var _))
        {
            ResetColision();
        }
    }

    protected override void ApplyEffect()
    {
        objectInCollision.GetComponent<IDamageable>().TakeDamage(damage);
    }
}
