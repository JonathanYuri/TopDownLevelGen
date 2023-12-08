using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class TriggerDamage : MonoBehaviour
{
    Timer damageTimer;

    [SerializeField] int damage;

    bool isColliding = false;
    IDamageable damageableInCollision;

    public event Action CollisionOccured;

    void Awake()
    {
        damageTimer = GetComponent<Timer>();
        damageTimer.OnTimerExpired += OnDamageTimerExpired;
    }

    void OnDestroy()
    {
        if (damageTimer != null)
        {
            damageTimer.OnTimerExpired -= OnDamageTimerExpired;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out IDamageable damageable))
        {
            isColliding = true;
            damageableInCollision = damageable;
            
            ApplyDamage();
            damageTimer.StartTimer();
        }
        CollisionOccured?.Invoke();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var _))
        {
            ResetColision();
        }
    }

    void OnDamageTimerExpired()
    {
        if (isColliding)
        {
            if (damageableInCollision == null)
            {
                ResetColision();
                return;
            }

            ApplyDamage();
            damageTimer.StartTimer();
        }
    }

    void ApplyDamage()
    {
        damageableInCollision.TakeDamage(damage);
    }

    void ResetColision()
    {
        isColliding = false;
        damageableInCollision = null;
    }
}
