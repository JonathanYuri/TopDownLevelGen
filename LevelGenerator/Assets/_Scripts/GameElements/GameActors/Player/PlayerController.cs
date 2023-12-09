using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the player's movement and interactions in the game.
/// </summary>
[RequireComponent(typeof(PlayerMovementController))]
public class PlayerController : MonoBehaviour, IDamageable, ISlowable
{
    PlayerMovementController playerMovementController;
    Timer slownessTimer;

    float velocityWithoutSlow;

    [SerializeField] int life = 100;

    public EventHandler<DoorEventArgs> PassedThroughTheDoorEvent;
    public event Action OnLevelComplete;

    void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
        velocityWithoutSlow = playerMovementController.Velocity;

        slownessTimer = GetComponentInChildren<Timer>();
        slownessTimer.OnTimerExpired += OnSlownessTimerExpired;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("OpenDoor"))
        {
            Door door = collision.GetComponent<Door>();
            DoorEventArgs doorEventArgs = new()
            {
                doorDirection = door.Direction,
            };
            PassedThroughTheDoorEvent?.Invoke(this, doorEventArgs);
        }
        else if (collision.CompareTag("LevelPortal"))
        {
            OnLevelComplete.Invoke();
        }
    }

    public void TakeDamage(int damage)
    {
        if (life - damage <= 0)
        {
            Die();
        }
        else
        {
            life -= damage;
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    public void TakeSlowness(float percentSlow, float timeSlow)
    {
        percentSlow = Mathf.Clamp01(percentSlow);

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
