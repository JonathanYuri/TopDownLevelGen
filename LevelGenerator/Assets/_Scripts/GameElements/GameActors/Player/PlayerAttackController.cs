using System.Collections.Generic;
using UnityEngine;

public static class AttackButtonsConstants
{
    public static KeyCode UP_ATTACK = KeyCode.UpArrow;
    public static KeyCode DOWN_ATTACK = KeyCode.DownArrow;
    public static KeyCode LEFT_ATTACK = KeyCode.LeftArrow;
    public static KeyCode RIGHT_ATTACK = KeyCode.RightArrow;

    public static bool IsUpAttackPressed() => Input.GetKey(UP_ATTACK);
    public static bool IsDownAttackPressed() => Input.GetKey(DOWN_ATTACK);
    public static bool IsLeftAttackPressed() => Input.GetKey(LEFT_ATTACK);
    public static bool IsRightAttackPressed() => Input.GetKey(RIGHT_ATTACK);
    public static bool IsAttackPressed() => IsUpAttackPressed() || IsDownAttackPressed() || IsLeftAttackPressed() || IsRightAttackPressed();
}

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] GameObject knife;
    [SerializeField] Timer attackTimer;
    InputManager inputManager;

    [SerializeField] GameObject attackSpawnUp;
    [SerializeField] GameObject attackSpawnDown;
    [SerializeField] GameObject attackSpawnRight;
    [SerializeField] GameObject attackSpawnLeft;

    [SerializeField] SoundController soundController;

    Dictionary<Vector2, GameObject> directionToAttackObject;

    bool canAttack = true;

    void Awake()
    {
        attackTimer.OnTimerExpired += OnAttackTimerExpired;

        directionToAttackObject = new()
        {
            { Vector2.down, attackSpawnDown },
            { Vector2.up, attackSpawnUp },
            { Vector2.left, attackSpawnLeft },
            { Vector2.right, attackSpawnRight },
        };
    }

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
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
        if (!inputManager.IsInputEnabled())
        {
            return;
        }

        if (canAttack)
        {
            if (AttackButtonsConstants.IsUpAttackPressed())
            {
                Attack(Vector2.up);
            }
            else if (AttackButtonsConstants.IsDownAttackPressed())
            {
                Attack(Vector3.down);
            }
            else if (AttackButtonsConstants.IsLeftAttackPressed())
            {
                Attack(Vector2.left);
            }
            else if (AttackButtonsConstants.IsRightAttackPressed())
            {
                Attack(Vector2.right);
            }
        }
    }

    void Attack(Vector3 directionToAttack)
    {
        canAttack = false;

        SpawnKnife(directionToAttack);

        attackTimer.StartTimer();
    }

    void SpawnKnife(Vector3 directionToThrowKnife)
    {
        GameObject attackSpawn = directionToAttackObject[directionToThrowKnife];
        GameObject thrownKnife = Instantiate(knife, attackSpawn.transform.position, Quaternion.identity);
        Projectile thrownKnifeScript = thrownKnife.GetComponent<Projectile>();
        thrownKnifeScript.InitializeProjectile(soundController, directionToThrowKnife);
    }

    void OnAttackTimerExpired()
    {
        canAttack = true;
    }
}
