using UnityEngine;

public class MovementDirectionHandler : MonoBehaviour
{
    public void SetRotationBasedOnMovementDirection(Vector2 movementDirection)
    {
        bool moveRight = movementDirection.x >= 0;

        float angleRotation = moveRight ?
            Vector2.Angle(Vector2.right, movementDirection) :
            Vector2.Angle(Vector2.left, movementDirection);

        if (!moveRight)
        {
            // spriteRenderer.flipX = true
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 180f, transform.rotation.eulerAngles.z);
        }
        else
        {
            // spriteRenderer.flipX = false
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, 0f, transform.rotation.eulerAngles.z);
        }

        if (transform.rotation.y == 180f)
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

        Quaternion rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, angleRotation);
        this.transform.rotation = rotation;
    }
}
