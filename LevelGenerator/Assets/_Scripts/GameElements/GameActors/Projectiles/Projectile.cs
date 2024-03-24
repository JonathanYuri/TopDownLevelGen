using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RotateObject))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    int shooterId;

    Rigidbody2D rb;
    [SerializeField] List<CollisionEffects> triggerEffects;

    RotateObject rotateObject;

    int totalEffectsApplied = 0;

    [SerializeField] float movimentVelocity = 2.0f;
    [SerializeField] Timer autoDestroyTimer;
    Vector2 movementDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rotateObject = GetComponent<RotateObject>();
        triggerEffects.ForEach(triggerEffect => triggerEffect.CollisionOccured += CollisionOccured);
        autoDestroyTimer.OnTimerExpired += OnAutoDestroyTimerExpired;
    }

    void OnDestroy()
    {
        triggerEffects.ForEach(triggerEffect => triggerEffect.CollisionOccured -= CollisionOccured);
        autoDestroyTimer.OnTimerExpired -= OnAutoDestroyTimerExpired;
    }

    void Start()
    {
        AudioManager.Instance.PlayCharacterSound("flight", shooterId);
        autoDestroyTimer.StartTimer();
    }

    void FixedUpdate()
    {
        Vector2 moviment = movimentVelocity * Time.fixedDeltaTime * movementDirection;
        rb.MovePosition((Vector2)this.transform.position + moviment);
    }

    void CollisionOccured()
    {
        totalEffectsApplied++;
        if (totalEffectsApplied == triggerEffects.Count)
        {
            PlayCollisionSound();
        }
    }

    public void InitializeProjectile(Vector2 movementDirection, int shooterId)
    {
        this.movementDirection = movementDirection;
        this.shooterId = shooterId;
        rotateObject.StartRotation(movementDirection);
    }

    void OnAutoDestroyTimerExpired()
    {
        if (gameObject != null)
        {
            PlayCollisionSound();
        }
    }

    void PlayCollisionSound()
    {
        AudioManager.Instance.PlayCharacterSound("collision", shooterId);
        AudioManager.Instance.StopCharacterSound("flight", shooterId);
        //StartCoroutine(WaitCollisionSoundToDestroy());
        Destroy(gameObject);
    }
}