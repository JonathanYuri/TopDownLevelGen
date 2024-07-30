using System;
using UnityEngine;

public class PlayerDamagedEventArgs : EventArgs
{
    public int damage;
    public string damageName;
}

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
    public EventHandler<PlayerDamagedEventArgs> PlayerDamaged;

    public event Action OnLevelComplete;
    public event Action OnDied;

    public bool OnLevelLoad { get; set; }
    public int Life { get => life; set => life = value; }
    public int MaxLife { get; private set; }

    void Awake()
    {
        MaxLife = Life;
        playerMovementController = GetComponent<PlayerMovementController>();
        velocityWithoutSlow = playerMovementController.Velocity;
        slownessTimer.OnTimerExpired += OnSlownessTimerExpired;
    }

    void Start()
    {
        playerHealthBarController = FindObjectOfType<HealthBarController>();
        playerHealthBarController.MaxLife = MaxLife;
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
            OnLevelLoad = true;
            OnLevelComplete?.Invoke();
        }
    }

    public void TakeDamage(int damage, string damageName)
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
        HandleDamageReceived(damage, damageName);
    }

    void HandleDamageReceived(int damage, string damageName)
    {
        PlayerDamaged?.Invoke(this, new() { damage = damage, damageName = damageName });
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

    public void Heal(int healAmount)
    {
        Life += healAmount;
        playerHealthBarController.UpdateLife(Life);
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
