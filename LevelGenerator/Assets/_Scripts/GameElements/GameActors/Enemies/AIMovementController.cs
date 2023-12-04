using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(MovementDirectionHandler))]
[RequireComponent(typeof(SpriteRenderer))]
public class AIMovementController : MonoBehaviour
{
    MovementDirectionHandler movementDirectionHandler;

    Rigidbody2D rb;
    SpriteRenderer spriteRenderer;

    [SerializeField] Vector3 moveTo;
    [SerializeField] bool move;
    
    bool firstMove = true;

    [SerializeField] float velocity;
    [SerializeField] float threshold = 0.1f;

    public float Velocity { get => velocity; set => velocity = value; }

    void Awake()
    {
        movementDirectionHandler = GetComponent<MovementDirectionHandler>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        moveTo = this.transform.position;
    }

    void FixedUpdate()
    {
        if (move)
        {
            Vector3 direction = moveTo - this.transform.position;
            float distanceToTarget = direction.magnitude;

            if (firstMove)
            {
                movementDirectionHandler.SetInitialRotationBasedOnMovementDirection(direction);
                firstMove = false;
            }

            if (distanceToTarget <= threshold)
            {
                move = false;
                firstMove = true;
                movementDirectionHandler.SetInitialRotationBasedOnMovementDirection(Vector2.right);
                spriteRenderer.flipX = false;
                spriteRenderer.flipY = false;
            }
            else
            {
                Vector3 movement = Velocity * Time.fixedDeltaTime * direction.normalized;
                rb.MovePosition(this.transform.position + movement);
            }
        }
    }
}
