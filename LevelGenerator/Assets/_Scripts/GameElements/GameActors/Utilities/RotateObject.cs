using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementDirectionHandler))]
public class RotateObject : MonoBehaviour
{
    MovementDirectionHandler movementDirectionHandler;

    [SerializeField] float rotationVelocity = 180f;
    bool rotateClockwise;

    void Awake()
    {
        movementDirectionHandler = GetComponent<MovementDirectionHandler>();
    }

    void Update()
    {
        Rotate();
    }

    void Rotate()
    {
        if (rotateClockwise)
        {
            this.transform.Rotate(-Vector3.forward, rotationVelocity * Time.deltaTime);
        }
        else
        {
            this.transform.Rotate(Vector3.forward, rotationVelocity * Time.deltaTime);
        }
    }

    public void StartRotation(Vector2 movementDirection)
    {
        rotateClockwise = movementDirection.x >= 0;
        movementDirectionHandler.SetInitialRotationBasedOnMovementDirection(movementDirection);
    }
}
