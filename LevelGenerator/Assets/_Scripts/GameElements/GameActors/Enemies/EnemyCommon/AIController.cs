using UnityEngine;

[RequireComponent(typeof(PatrolController))]
[RequireComponent(typeof(ChaseController))]
[RequireComponent(typeof(AIVision))]
public class AIController : MonoBehaviour
{
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] EnemyLocation location;

    PatrolController patrolController;
    ChaseController chaseController;
    AIVision aiVision;
    AIPathController aiPathController;

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
        if (aiVision.PlayerVisible)
        {
            chaseController.TryChaseTarget();
        }
        else
        {
            patrolController.TryPatrol();
        }
    }
}
