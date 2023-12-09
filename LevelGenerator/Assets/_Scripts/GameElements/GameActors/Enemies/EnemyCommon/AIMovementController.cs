using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementController : MonoBehaviour
{
    [SerializeField] MovementDirectionHandler movementDirectionHandler;

    [SerializeField] Rigidbody2D rb;

    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Vector3 moveTo;
    [SerializeField] bool move;

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

        if (movementDirectionHandler == null)
        {
            Debug.LogError("Movement Direction Handler not assign");    
        }

        moveTo = this.transform.position;
    }

    void FixedUpdate()
    {
        if (move)
        {
            Vector3 direction = moveTo - this.transform.position;
            float distanceToTarget = direction.magnitude;

            if (distanceToTarget <= threshold)
            {
                move = false;
                ResetMovementOrientation();
            }
            else
            {
                Vector3 movement = Velocity * Time.fixedDeltaTime * direction.normalized;
                rb.MovePosition(this.transform.position + movement);
            }
        }
        else
        {
            ResetMovementOrientation();
        }
    }

    void ResetMovementOrientation()
    {
        movementDirectionHandler.SetRotationBasedOnMovementDirection(Vector2.right);
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z);
    }

    public void SetMovement(Vector2 moveTo)
    {
        this.moveTo = moveTo;
        move = true;

        Vector2 direction = moveTo - (Vector2)transform.position;
        movementDirectionHandler.SetRotationBasedOnMovementDirection(direction);
    }
}
