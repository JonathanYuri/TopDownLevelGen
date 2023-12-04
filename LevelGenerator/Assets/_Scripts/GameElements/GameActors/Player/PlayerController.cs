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

    [SerializeField] int life = 100;

    public EventHandler<DoorEventArgs> PassedThroughTheDoorEvent;
    public event Action OnLevelComplete;

    void Awake()
    {
        playerMovementController = GetComponent<PlayerMovementController>();
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
        StartCoroutine(TakeSlownessCoroutine(percentSlow, timeSlow));
    }

    IEnumerator TakeSlownessCoroutine(float percentSlow, float timeSlow)
    {
        percentSlow = Mathf.Clamp01(percentSlow);
        float velocityWithoutSlow = playerMovementController.Velocity;

        StartSlowness(percentSlow);
        yield return new WaitForSeconds(timeSlow);
        EndSlowness(velocityWithoutSlow);
    }

    void StartSlowness(float percentSlow)
    {
        // se eu quero um slow de 0.3, eu quero que minha velocity seja multiplicada por 0.7
        float velocityMultiplier = 1 - percentSlow;
        playerMovementController.Velocity *= velocityMultiplier;
    }

    void EndSlowness(float velocityWithoutSlow)
    {
        playerMovementController.Velocity = velocityWithoutSlow;
    }
}
