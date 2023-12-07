using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementDirectionHandler))]
public class AIMovementController : MonoBehaviour
{
    MovementDirectionHandler movementDirectionHandler;

    [SerializeField] Rigidbody2D rb;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Vector3 moveTo;
    [SerializeField] bool move;
    
    bool firstMove = true;

    [SerializeField] float velocity;
    [SerializeField] float threshold = 0.1f;

    public float Velocity { get => velocity; set => velocity = value; }

    void Start()
    {
        if (spriteRenderer == null)
        {
            Debug.LogError("Sprite renderer not assign");
        }

        if (rb == null)
        {
            Debug.LogError("Rigidbody not assign");
        }

        movementDirectionHandler = GetComponent<MovementDirectionHandler>();
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

    public void SetMovement(Vector2 moveTo)
    {
        this.moveTo = moveTo;
        move = true;
    }
}
