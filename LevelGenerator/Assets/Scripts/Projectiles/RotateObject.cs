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
        float angleRotation = Mathf.Min(Vector2.Angle(Vector2.right, movementDirection), Vector2.Angle(Vector2.left, movementDirection));

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();

        if (movementDirection.x < 0)
        {
            angleRotation = -angleRotation;
            spriteRenderer.flipX = true;
        }

        if (movementDirection.y < 0)
        {
            spriteRenderer.flipY = true;
        }

        Quaternion rotation = Quaternion.Euler(0, 0, angleRotation);
        this.transform.rotation = rotation;
    }
}
