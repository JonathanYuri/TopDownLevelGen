using UnityEngine;

public class TriggerDamage : CollisionEffects
{
    [SerializeField] int damage;
    [SerializeField] string damageName;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var _) && canApplyEffect)
        {
            isColliding = true;
            objectInCollision = collision.gameObject;

            ApplyEffect();
        }
        InvokeCollisionEvent();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<IDamageable>(out var _))
        {
            ResetColision();
        }
    }

    protected override void ApplyEffect()
    {
        objectInCollision.GetComponent<IDamageable>().TakeDamage(damage, damageName);
        InitializeEffectCooldown();
    }
}
