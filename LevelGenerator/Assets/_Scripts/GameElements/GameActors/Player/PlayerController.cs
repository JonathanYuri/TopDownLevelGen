using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the player's movement and interactions in the game.
/// </summary>
[RequireComponent(typeof(PlayerMovementController))]
public class PlayerController : MonoBehaviour, IDamageable, IMortal, ISlowable
{
    PlayerMovementController playerMovementController;
    HealthBarController playerHealthBarController;
    [SerializeField] Timer slownessTimer;
    [SerializeField] DamageInvincibilityController damageInvincibilityController;

    float velocityWithoutSlow;

    [SerializeField] int life = 100;

    public EventHandler<DoorEventArgs> PassedThroughTheDoorEvent;

    public event Action OnLevelComplete;
    public event Action OnDied;

    public bool OnLevelLoad { get; set; }
    public int Life { get => life; private set => life = value; }

    void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        velocityWithoutSlow = playerMovementController.Velocity;
        slownessTimer.OnTimerExpired += OnSlownessTimerExpired;
    }

    void Start()
    {
        playerHealthBarController = FindObjectOfType<HealthBarController>();
    }

    void OnDestroy()
    {
        if (slownessTimer != null)
        {
            slownessTimer.OnTimerExpired -= OnSlownessTimerExpired;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("OpenDoor"))
        {
            Door door = collision.GetComponentInParent<Door>();
            DoorEventArgs doorEventArgs = new()
            {
                doorDirection = door.Direction,
            };
            PassedThroughTheDoorEvent?.Invoke(this, doorEventArgs);
        }
        else if (collision.CompareTag("LevelPortal"))
        {
            OnLevelComplete?.Invoke();
            OnLevelLoad = true;
        }
    }

    public void TakeDamage(int damage)
    {
        if (damageInvincibilityController.Invincible)
        {
            return;
        }

        if (OnLevelLoad)
        {
            return;
        }

        damageInvincibilityController.MakeInvincible();
        HandleDamageReceived(damage);
    }

    void HandleDamageReceived(int damage)
    {
        if (Life - damage <= 0)
        {
            playerHealthBarController.UpdateLife(0);
            Die();
        }
        else
        {
            Life -= damage;
            playerHealthBarController.UpdateLife(Life);
        }
    }

    public void Die()
    {
        OnDied?.Invoke();
        Destroy(gameObject);
    }

    public void TakeSlowness(float percentSlow, float timeSlow)
    {
        percentSlow = Mathf.Clamp01(percentSlow);

        // se ele ja estava lento
        slownessTimer.StopTimer();
        EndSlowness();

        StartSlowness(percentSlow, timeSlow);
    }

    void OnSlownessTimerExpired()
    {
        EndSlowness();
    }

    void StartSlowness(float percentSlow, float timeSlow)
    {
        // se eu quero um slow de 0.3, eu quero que minha velocity seja multiplicada por 0.7
        float velocityMultiplier = 1 - percentSlow;
        playerMovementController.Velocity *= velocityMultiplier;

        slownessTimer.TimerDuration = timeSlow;
        slownessTimer.StartTimer();
    }

    void EndSlowness()
    {
        playerMovementController.Velocity = velocityWithoutSlow;
    }
}
