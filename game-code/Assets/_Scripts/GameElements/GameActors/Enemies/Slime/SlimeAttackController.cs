using UnityEngine;

[RequireComponent(typeof(Timer))]
public class SlimeAttackController : MonoBehaviour
{
    Enemy enemy;

    [SerializeField] AIVision aiVision;
    [SerializeField] EnemyTargetManager targetManager;
    [SerializeField] GameObject slimeBall;
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

    void Start()
    {
        enemy = GetComponentInParent<Enemy>();
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
        SpawnSlimeBall(directionToTarget.normalized);

        attackTimer.StartTimer();
    }

    void SpawnSlimeBall(Vector3 directionToThrow)
    {
        Vector2 positionToSpawnProjectile = this.transform.position + (directionToThrow / 2);
        GameObject thrownSlimeBall = Instantiate(slimeBall, positionToSpawnProjectile, Quaternion.identity);
        Projectile thrownSlimeBallScript = thrownSlimeBall.GetComponent<Projectile>();
        thrownSlimeBallScript.InitializeProjectile(soundController, directionToThrow);

        soundController.PlaySound(SoundsName.Attack);
    }

    void OnAttackTimerExpired()
    {
        canAttack = true;
    }
}
