using System;
using System.Collections;
using UnityEngine;

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
            effectCooldown.StartTimer();
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
