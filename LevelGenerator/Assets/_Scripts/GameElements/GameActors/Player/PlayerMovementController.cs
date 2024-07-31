using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovementController : MonoBehaviour
{
    InputManager inputManager;
    Rigidbody2D rb;

    SpriteRenderer spriteRenderer;

    [SerializeField] float velocity;

    public float Velocity { get => velocity; set => velocity = value; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if (!inputManager.IsInputEnabled())
        {
            return;
        }

        Vector2 movement = Vector2.zero;

        if (Input.GetKey(KeyCode.W)) movement.y = 1;
        if (Input.GetKey(KeyCode.S)) movement.y = -1;
        if (Input.GetKey(KeyCode.A)) movement.x = -1;
        if (Input.GetKey(KeyCode.D)) movement.x = 1;

        spriteRenderer.flipX = movement.x < 0;
        Vector3 finalMovement = Velocity * Time.fixedDeltaTime * movement;
        rb.MovePosition(this.transform.position + finalMovement);
    }
}
