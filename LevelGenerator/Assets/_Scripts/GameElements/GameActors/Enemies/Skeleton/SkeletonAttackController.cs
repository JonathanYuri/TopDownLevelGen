using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Timer))]
public class SkeletonAttackController : MonoBehaviour
{
    [SerializeField] AIVision aiVision;
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] GameObject bone;
    [SerializeField] SoundController soundController;

    Timer attackTimer;
    bool canAttack = true;

    void Awake()
    {
        attackTimer = GetComponent<Timer>();
        attackTimer.OnTimerExpired += OnAttackTimerExpired;

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
        if (aiVision.PlayerVisible && canAttack)
        {
            Attack();
        }
    }

    void Attack()
    {
        if (targetManager.Player == null)
        {
            return;
        }

        canAttack = false;

        Vector2 directionToTarget = targetManager.Player.position - this.transform.position;
        SpawnBone(directionToTarget.normalized);

        attackTimer.StartTimer();
    }

    void SpawnBone(Vector3 directionToThrow)
    {
        Vector2 positionToSpawnProjectile = this.transform.position + (directionToThrow / 2);
        GameObject thrownBone = Instantiate(bone, positionToSpawnProjectile, Quaternion.identity);
        Projectile thrownBoneScript = thrownBone.GetComponent<Projectile>();
        thrownBoneScript.InitializeProjectile(soundController, directionToThrow);
    }

    void OnAttackTimerExpired()
    {
        canAttack = true;
    }
}
