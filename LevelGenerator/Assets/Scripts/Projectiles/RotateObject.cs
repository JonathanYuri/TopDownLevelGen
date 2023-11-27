using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    [SerializeField] float rotationVelocity = 180f;
    bool rotateClockwise;

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

    public void SetInitialRotationBasedOnMovementDirection(Vector2 movementDirection)
    {
        rotateClockwise = movementDirection.x >= 0;
        float angleRotation = Vector2.Angle(Vector2.right, movementDirection);
        Quaternion rotation = Quaternion.Euler(0, 0, angleRotation);
        this.transform.rotation = rotation;
    }
}
