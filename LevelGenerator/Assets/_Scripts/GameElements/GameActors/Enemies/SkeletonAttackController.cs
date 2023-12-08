using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class SkeletonAttackController : MonoBehaviour
{
    [SerializeField] AIVision aiVision;

    Timer attackTimer;
    bool canAttack = true;

    void Awake()
    {
        attackTimer = GetComponent<Timer>();
        attackTimer.OnTimerExpired += OnAttackTimerExpired;
    }

    void Start()
    {
        if (aiVision == null)
        {
            Debug.LogError("AIVision not assign");
        }
    }

    void OnDestroy()
    {
        if (attackTimer != null)
        {
            attackTimer.OnTimerExpired -= OnAttackTimerExpired;
        }
    }

    void Update()
    {
        TryAttack();
    }

    void TryAttack()
    {
        if (aiVision.TargetVisible && canAttack)
        {
            Attack();
        }
    }

    void Attack()
    {
        canAttack = false;
        attackTimer.StartTimer();
    }

    void OnAttackTimerExpired()
    {
        canAttack = true;
    }
}
