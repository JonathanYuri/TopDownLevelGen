using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] float velocity;

    public float Velocity { get => velocity; set => velocity = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        Vector3 movement = Velocity * Time.fixedDeltaTime * new Vector3(horizontalMovement, verticalMovement);
        rb.MovePosition(this.transform.position + movement);
    }
}
