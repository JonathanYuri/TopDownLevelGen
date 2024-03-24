using UnityEngine;

[RequireComponent(typeof(PatrolController))]
[RequireComponent(typeof(EnemyBehaviorOnPlayerVisible))]
[RequireComponent(typeof(AIVision))]
public class AIController : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] EnemyLocation location;
    [SerializeField] Timer damageForgetTimer;

    PatrolController patrolController;
    EnemyBehaviorOnPlayerVisible enemyBehaviorOnPlayerVisible;
    AIVision aiVision;
    AIPathController aiPathController;

    bool targetDamagedMe;

    void Awake()
    {
        patrolController = GetComponent<PatrolController>();
        patrolController.TargetManager = targetManager;
        patrolController.Location = location;

        enemyBehaviorOnPlayerVisible = GetComponent<EnemyBehaviorOnPlayerVisible>();
        enemyBehaviorOnPlayerVisible.TargetManager = targetManager;

        aiPathController = GetComponent<AIPathController>();
        aiPathController.TargetManager = targetManager;

        aiVision = GetComponent<AIVision>();
        aiVision.TargetManager = targetManager;
        aiVision.Location = location;

        enemy.OnDamageTaken += OnDamageTaken;
        damageForgetTimer.OnTimerExpired += OnDamageForgetTimerExpired;
    }

    void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnDamageTaken -= OnDamageTaken;
        }
        if (damageForgetTimer != null)
        {
            damageForgetTimer.OnTimerExpired -= OnDamageForgetTimerExpired;
        }
    }
    void Update()
    {
        if (targetManager.PlayerLocation == null)
        {
            return;
        }

        if (location.IsInPlayerRoom(targetManager.PlayerLocation.RoomPosition))
        {
            aiPathController.FollowPath = true;
            DecideEnemyAction();
        }
        else
        {
            aiPathController.FollowPath = false;
            targetManager.Target = null;
        }
    }

    void DecideEnemyAction()
    {
        if (aiVision.PlayerVisible || targetDamagedMe)
        {
            enemyBehaviorOnPlayerVisible.Run();
        }
        else
        {
            patrolController.TryPatrol();
        }
    }

    void OnDamageTaken()
    {
        targetDamagedMe = true;
        damageForgetTimer.StartTimer();
    }

    void OnDamageForgetTimerExpired()
    {
        targetDamagedMe = false;
    }
}
