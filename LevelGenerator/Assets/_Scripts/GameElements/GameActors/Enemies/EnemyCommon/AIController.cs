using UnityEngine;

[RequireComponent(typeof(PatrolController))]
[RequireComponent(typeof(ChaseController))]
[RequireComponent(typeof(AIVision))]
public class AIController : MonoBehaviour
{
    [SerializeField] Enemy enemy;
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] EnemyLocation location;
    [SerializeField] Timer damageForgetTimer;

    PatrolController patrolController;
    ChaseController chaseController;
    AIVision aiVision;
    AIPathController aiPathController;

    bool targetDamagedMe;

    void Awake()
    {
        patrolController = GetComponent<PatrolController>();
        patrolController.TargetManager = targetManager;
        patrolController.Location = location;

        chaseController = GetComponent<ChaseController>();
        chaseController.TargetManager = targetManager;

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
        if (location.IsInPlayerRoom(targetManager.PlayerLocation.RoomPosition))
        {
            DecideEnemyAction();
        }
    }

    void DecideEnemyAction()
    {
        if (aiVision.PlayerVisible || targetDamagedMe)
        {
            chaseController.TryChaseTarget();
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
