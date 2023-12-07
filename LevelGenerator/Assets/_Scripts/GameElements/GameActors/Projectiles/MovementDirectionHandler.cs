using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class MovementDirectionHandler : MonoBehaviour
{
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetRotationBasedOnMovementDirection(Vector2 movementDirection)
    {
        bool moveRight = movementDirection.x >= 0;

        float angleRotation = moveRight ?
            Vector2.Angle(Vector2.right, movementDirection) :
            Vector2.Angle(Vector2.left, movementDirection);

        if (!moveRight)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
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
