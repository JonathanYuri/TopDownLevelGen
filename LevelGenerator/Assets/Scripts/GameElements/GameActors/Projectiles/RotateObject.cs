using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RotateObject : MonoBehaviour
{
    [SerializeField] float rotationVelocity = 180f;
    bool rotateClockwise;

    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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

    public void SetInitialRotationBasedOnMovementDirection(Vector2 movementDirection)
    {
        bool moveRight = movementDirection.x >= 0;
        rotateClockwise = moveRight;
       
        float angleRotation = moveRight ?
            Vector2.Angle(Vector2.right, movementDirection) :
            Vector2.Angle(Vector2.left, movementDirection);

        if (!moveRight)
        {
            spriteRenderer.flipX = true;
        }

        if (spriteRenderer.flipX)
        {
            if (movementDirection.y >= 0)
            {
                angleRotation = -angleRotation;
            }
        }
        else
        {
            if (movementDirection.y < 0)
            {
                angleRotation = -angleRotation;
            }
        }

        Quaternion rotation = Quaternion.Euler(0, 0, angleRotation);
        this.transform.rotation = rotation;
    }
}
