using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BlinkEffect))]
public class DamageInvincibilityController : MonoBehaviour
{
    BlinkEffect damageInvincibilityEffect;
    [SerializeField] Timer invincibilityTimer;

    public bool Invincible { get; set; }

    void Awake()
    {
        damageInvincibilityEffect = GetComponent<BlinkEffect>();
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
