using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;

    Rigidbody2D rb;

    public EventHandler<DoorEventArgs> PassedThroughTheDoorEvent;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float movimentoHorizontal = Input.GetAxis("Horizontal");
        float movimentoVertical = Input.GetAxis("Vertical");

        Vector3 movimento = new Vector3(movimentoHorizontal, movimentoVertical) * movementSpeed * Time.fixedDeltaTime;
        rb.MovePosition(this.transform.position + movimento);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("OpenDoor"))
        {
            Door door = collision.GetComponent<Door>();
            DoorEventArgs doorEventArgs = new()
            {
                doorDirection = door.direction,
            };
            PassedThroughTheDoorEvent?.Invoke(this, doorEventArgs);
        }
    }
}
