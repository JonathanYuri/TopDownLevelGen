using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class SkeletonAttackController : MonoBehaviour
{
    [SerializeField] AIVision aiVision;
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] GameObject bone;

    Timer attackTimer;
    bool canAttack = true;

    void Awake()
    {
        attackTimer = GetComponent<Timer>();
        attackTimer.OnTimerExpired += OnAttackTimerExpired;
    }

    void OnDestroy()
    {
        if (attackTimer != null)
        {
            attackTimer.OnTimerExpired -= OnAttackTimerExpired;
        }
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
        SpawnBone(directionToTarget.normalized);

        attackTimer.StartTimer();
    }

    void SpawnBone(Vector3 directionToThrow)
    {
        Vector2 positionToSpawnProjectile = this.transform.position + (directionToThrow / 2);
        GameObject thrownBone = Instantiate(bone, positionToSpawnProjectile, Quaternion.identity);
        Projectile thrownBoneScript = thrownBone.GetComponent<Projectile>();
        thrownBoneScript.InitializeProjectile(directionToThrow);
    }

    void OnAttackTimerExpired()
    {
        canAttack = true;
    }
}
