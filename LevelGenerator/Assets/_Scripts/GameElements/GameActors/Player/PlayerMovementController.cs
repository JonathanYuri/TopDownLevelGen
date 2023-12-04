using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float velocity;

    public float Velocity { get => velocity; set => velocity = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float movimentoHorizontal = Input.GetAxis("Horizontal");
        float movimentoVertical = Input.GetAxis("Vertical");

        Vector3 movimento = Velocity * Time.fixedDeltaTime * new Vector3(movimentoHorizontal, movimentoVertical);
        rb.MovePosition(this.transform.position + movimento);
    }
}
