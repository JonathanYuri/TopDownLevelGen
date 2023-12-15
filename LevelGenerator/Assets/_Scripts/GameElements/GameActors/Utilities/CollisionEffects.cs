using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class CollisionEffects : MonoBehaviour
{
    [SerializeField] protected Timer effectCooldown;

    protected bool isColliding = false;
    protected GameObject objectInCollision;

    public event Action CollisionOccured;

    protected abstract void ApplyEffect();

    void Start()
    {
        effectCooldown.OnTimerExpired += OnEffectTimerExpired;
    }

    void OnDestroy()
    {
        if (effectCooldown != null)
        {
            effectCooldown.OnTimerExpired -= OnEffectTimerExpired;
        }
    }

    void OnEffectTimerExpired()
    {
        if (isColliding)
        {
            if (objectInCollision == null)
            {
                ResetColision();
                return;
            }

            ApplyEffect();
            effectCooldown.StartTimer();
        }
    }

    protected void InvokeCollisionEvent() => CollisionOccured?.Invoke();

    protected void ResetColision()
    {
        isColliding = false;
        objectInCollision = null;
    }
}
