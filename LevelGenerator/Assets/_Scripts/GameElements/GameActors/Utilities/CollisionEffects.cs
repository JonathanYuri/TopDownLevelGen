using System;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class CollisionEffects : MonoBehaviour
{
    [SerializeField] protected Timer effectCooldown;

    protected bool isColliding = false;
    protected bool canApplyEffect = true;
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
        canApplyEffect = true;

        if (isColliding)
        {
            if (objectInCollision == null)
            {
                ResetColision();
                return;
            }

            ApplyEffect();
        }
    }

    protected void InitializeEffectCooldown()
    {
        effectCooldown.StartTimer();
        canApplyEffect = false;
    }

    protected void ResetColision()
    {
        isColliding = false;
        objectInCollision = null;
    }

    protected void InvokeCollisionEvent() => CollisionOccured?.Invoke();
}
