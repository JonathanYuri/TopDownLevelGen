using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AttackButtonsConstants
{
    public static KeyCode UP_ATTACK = KeyCode.I;
    public static KeyCode DOWN_ATTACK = KeyCode.K;
    public static KeyCode LEFT_ATTACK = KeyCode.J;
    public static KeyCode RIGHT_ATTACK = KeyCode.L;

    public static bool IsUpAttackPressed() => Input.GetKey(UP_ATTACK);
    public static bool IsDownAttackPressed() => Input.GetKey(DOWN_ATTACK);
    public static bool IsLeftAttackPressed() => Input.GetKey(LEFT_ATTACK);
    public static bool IsRightAttackPressed() => Input.GetKey(RIGHT_ATTACK);
}

public class PlayerAttackController : MonoBehaviour
{
    [SerializeField] GameObject knife;
    Timer attackTimer;

    bool canAttack = true;

    void Awake()
    {
        attackTimer = GetComponentInChildren<Timer>();
        attackTimer.OnTimerExpired += OnAttackTimerExpired;
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
        GameObject thrownKnife = Instantiate(knife, this.transform.position + directionToThrowKnife, Quaternion.identity);
        Projectile thrownKnifeScript = thrownKnife.GetComponent<Projectile>();
        thrownKnifeScript.InitializeProjectile(directionToThrowKnife);
    }

    void OnAttackTimerExpired()
    {
        canAttack = true;
    }
}
