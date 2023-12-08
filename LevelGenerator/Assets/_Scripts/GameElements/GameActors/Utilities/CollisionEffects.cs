using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CollisionEffects : MonoBehaviour
{
    protected Timer effectTimer;

    protected bool isColliding = false;
    protected GameObject objectInCollision;

    public event Action CollisionOccured;

    protected abstract void ApplyEffect();

    void Awake()
    {
        effectTimer = GetComponent<Timer>();
        effectTimer.OnTimerExpired += OnEffectTimerExpired;
    }

    void OnDestroy()
    {
        if (effectTimer != null)
        {
            effectTimer.OnTimerExpired -= OnEffectTimerExpired;
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
            effectTimer.StartTimer();
        }
    }

    protected void InvokeCollisionEvent() => CollisionOccured?.Invoke();

    protected void ResetColision()
    {
        isColliding = false;
        objectInCollision = null;
    }
}
