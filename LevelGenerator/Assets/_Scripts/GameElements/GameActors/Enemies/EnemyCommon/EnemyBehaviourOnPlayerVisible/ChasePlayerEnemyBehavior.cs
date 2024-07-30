public class ChasePlayerEnemyBehavior : EnemyBehaviorOnPlayerVisible
{
    public override void Run()
    {
        TargetManager.ChasePlayer();
    }
}
