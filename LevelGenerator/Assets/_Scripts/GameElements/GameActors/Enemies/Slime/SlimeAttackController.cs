using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class SlimeAttackController : MonoBehaviour
{
    [SerializeField] AIVision aiVision;
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] GameObject slimeBall;

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

        if (targetManager == null)
        {
            Debug.LogError("TargetManager not assign");
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
        if (targetManager.Target == null)
        {
            return;
        }

        canAttack = false;

        Vector2 directionToTarget = targetManager.Target.position - this.transform.position;
        SpawnSlimeBall(directionToTarget.normalized);

        attackTimer.StartTimer();
    }

    void SpawnSlimeBall(Vector3 directionToThrow)
    {
        Vector2 positionToSpawnProjectile = this.transform.position + (directionToThrow / 2);
        GameObject thrownSlimeBall = Instantiate(slimeBall, positionToSpawnProjectile, Quaternion.identity);
        Projectile thrownSlimeBallScript = thrownSlimeBall.GetComponent<Projectile>();
        thrownSlimeBallScript.InitializeProjectile(directionToThrow);
    }

    void OnAttackTimerExpired()
    {
        canAttack = true;
    }
}
