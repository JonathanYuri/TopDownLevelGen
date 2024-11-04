using UnityEngine;

public class TriggerSlowness : CollisionEffects
{
    [SerializeField] float percentSlow;
    [SerializeField] float timeSlow;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ISlowable>(out var _) && canApplyEffect)
        {
            isColliding = true;
            objectInCollision = collision.gameObject;

            ApplyEffect();
        }
        InvokeCollisionEvent();
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ISlowable>(out var _))
        {
            ResetColision();
        }
    }

    protected override void ApplyEffect()
    {
        objectInCollision.GetComponent<ISlowable>().TakeSlowness(percentSlow, timeSlow);
        InitializeEffectCooldown();
    }
}
