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
    [SerializeField] float timeBetweenAttacks = 0.5f;

    bool canAttack = true;

    void Update()
    {
        if (canAttack)
        {
            if (AttackButtonsConstants.IsUpAttackPressed())
            {
                SpawnKnife(Vector2.up);
            }
            else if (AttackButtonsConstants.IsDownAttackPressed())
            {
                SpawnKnife(Vector3.down);
            }
            else if (AttackButtonsConstants.IsLeftAttackPressed())
            {
                SpawnKnife(Vector2.left);
            }
            else if (AttackButtonsConstants.IsRightAttackPressed())
            {
                SpawnKnife(Vector2.right);
            }
        }
    }

    void SpawnKnife(Vector3 directionToThrowKnife)
    {
        canAttack = false;
        GameObject thrownKnife = Instantiate(knife, this.transform.position + directionToThrowKnife, Quaternion.identity);
        Knife thrownKnifeScript = thrownKnife.GetComponent<Knife>();
        thrownKnifeScript.SetInitialParams(directionToThrowKnife);
        StartCoroutine(WaitToAttackAgain());
    }

    IEnumerator WaitToAttackAgain()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        canAttack = true;
    }
}
