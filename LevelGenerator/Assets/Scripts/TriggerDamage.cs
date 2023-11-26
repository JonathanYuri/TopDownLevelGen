using System;
using System.Collections;
using UnityEngine;

public class TriggerDamage : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] float reloadTimeAttack;

    [SerializeField] string targetTag;
    bool isColliding = false;

    public event Action CollisionOccured;

    void OnTriggerEnter2D(Collider2D collision)
    {
        // so vai colidir com quem quer colidir, por exemplo nao quero que o ataque do player afete o player
        if (!collision.gameObject.CompareTag(targetTag))
        {
            return;
        }

        if (collision.TryGetComponent(out IDamageable damageable))
        {
            isColliding = true;
            StartCoroutine(ApplyDamageOverTime(damageable));
        }
        CollisionOccured?.Invoke();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var _))
        {
            isColliding = false;
        }
    }

    IEnumerator ApplyDamageOverTime(IDamageable damageable)
    {
        while (isColliding)
        {
            if (damageable == null)
            {
                isColliding = false;
                break;
            }

            damageable.TakeDamage(damage);
            yield return new WaitForSeconds(reloadTimeAttack);
        }
    }
}
