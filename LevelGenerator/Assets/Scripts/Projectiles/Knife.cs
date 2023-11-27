using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Knife : MonoBehaviour
{
    Rigidbody2D rb;
    TriggerDamage triggerDamage;

    [SerializeField] float rotationVelocity = 180f;
    [SerializeField] float movimentVelocity = 2.0f;
    [SerializeField] float timeToAutoDestroy = 4.0f;

    public Vector2 directionToMove;
    public bool rotateClockwise;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        triggerDamage = GetComponentInChildren<TriggerDamage>();
        triggerDamage.CollisionOccured += CollisionOccured;
    }

    void Start()
    {
        StartCoroutine(AutoDestroy());
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

    void FixedUpdate()
    {
        Vector2 moviment = movimentVelocity * Time.fixedDeltaTime * directionToMove;
        rb.MovePosition((Vector2)this.transform.position + moviment);
    }

    void OnDestroy()
    {
        if (triggerDamage != null)
        {
            triggerDamage.CollisionOccured -= CollisionOccured;
        }
    }

    IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(timeToAutoDestroy);
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
    }

    void CollisionOccured()
    {
        Destroy(gameObject);
    }

    public void SetRotationBasedOnDirection(Vector2 direction)
    {
        directionToMove = direction;

        if (direction == Vector2.left || direction == Vector2.down)
        {
            rotateClockwise = false;
            GetComponent<SpriteRenderer>().flipX = true;
        }
        else if (direction == Vector2.up)
        {
            rotateClockwise = false;
        }
        else if (direction == Vector2.right)
        {
            rotateClockwise = true;
        }
    }
}
