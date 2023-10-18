using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controls the player's movement and interactions in the game.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;

    Rigidbody2D rb;

    public EventHandler<DoorEventArgs> PassedThroughTheDoorEvent;
    public event Action OnLevelComplete;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float movimentoHorizontal = Input.GetAxis("Horizontal");
        float movimentoVertical = Input.GetAxis("Vertical");

        Vector3 movimento = movementSpeed * Time.fixedDeltaTime * new Vector3(movimentoHorizontal, movimentoVertical);
        rb.MovePosition(this.transform.position + movimento);
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
}
