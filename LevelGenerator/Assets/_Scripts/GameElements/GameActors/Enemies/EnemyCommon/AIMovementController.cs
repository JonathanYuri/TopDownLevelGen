using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMovementController : MonoBehaviour
{
    [SerializeField] MovementDirectionHandler movementDirectionHandler;
    [SerializeField] Rigidbody2D rb;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] SoundController soundController;

    [SerializeField] float velocity;
    [SerializeField] float threshold = 0.1f;
    
    Vector3 destination;

    public float Velocity { get => velocity; set => velocity = value; }
    public bool ArrivedAtDestination { get; set; } = true;

    void Awake()
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
    }

    void FixedUpdate()
    {
        if (!ArrivedAtDestination)
        {
            MoveTowardsDestination();
        }
    }

    void MoveTowardsDestination()
    {
        Vector3 direction = destination - this.transform.position;
        float distanceToTarget = direction.magnitude;

        if (distanceToTarget <= threshold)
        {
            ArrivedAtDestination = true;
            soundController.StopSound(SoundsName.Footsteps);
        }
        else
        {
            Vector2 movement = Velocity * Time.fixedDeltaTime * direction.normalized;
            rb.velocity = Vector2.zero;
            rb.MovePosition((Vector2)this.transform.position + movement);
        }
    }

    public void SetDestination(Vector2 destination)
    {
        if (Vector2.Distance(transform.position, destination) < threshold)
        {
            return;
        }

        this.destination = destination;

        ArrivedAtDestination = false;
        soundController.PlaySound(SoundsName.Footsteps);

        Vector2 direction = destination - (Vector2)transform.position;
        movementDirectionHandler.SetRotationBasedOnMovementDirection(direction);
    }
}
