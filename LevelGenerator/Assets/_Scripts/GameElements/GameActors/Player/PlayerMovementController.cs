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

        float horizontalMovement = Input.GetAxis("Horizontal");
        float verticalMovement = Input.GetAxis("Vertical");

        spriteRenderer.flipX = horizontalMovement < 0;

        Vector3 movement = Velocity * Time.fixedDeltaTime * new Vector3(horizontalMovement, verticalMovement);
        rb.MovePosition(this.transform.position + movement);
    }
}
