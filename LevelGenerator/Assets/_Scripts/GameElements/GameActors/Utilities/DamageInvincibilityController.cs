using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageInvincibilityEffect))]
public class DamageInvincibilityController : MonoBehaviour
{
    DamageInvincibilityEffect damageInvincibilityEffect;
    [SerializeField] Timer invincibilityTimer;

    public bool Invincible { get; set; }

    void Awake()
    {
        damageInvincibilityEffect = GetComponent<DamageInvincibilityEffect>();
    }

    void Start()
    {
        invincibilityTimer.OnTimerExpired += OnInvincibilityTimerExpired;
    }

    void OnDestroy()
    {
        if (invincibilityTimer != null)
        {
            invincibilityTimer.OnTimerExpired -= OnInvincibilityTimerExpired;
        }
    }

    public void MakeInvincible()
    {
        if (!Invincible)
        {
            Invincible = true;
            damageInvincibilityEffect.StartEffect(invincibilityTimer.TimerDuration);
            invincibilityTimer.StartTimer();
        }
    }

    void OnInvincibilityTimerExpired()
    {
        Invincible = false;
    }
}
