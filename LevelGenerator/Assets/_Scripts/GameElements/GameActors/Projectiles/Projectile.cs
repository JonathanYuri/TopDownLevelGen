using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RotateObject))]
[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    Rigidbody2D rb;
    CollisionEffects triggerEffect;
    RotateObject rotateObject;

    [SerializeField] float movimentVelocity = 2.0f;
    [SerializeField] float timeToAutoDestroy = 4.0f;

    Vector2 movementDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rotateObject = GetComponent<RotateObject>();
        triggerEffect = GetComponentInChildren<CollisionEffects>();
        triggerEffect.CollisionOccured += CollisionOccured;
    }

    void OnDestroy()
    {
        if (triggerEffect != null)
        {
            triggerEffect.CollisionOccured -= CollisionOccured;
        }

        StopAllCoroutines();
    }

    void Start()
    {
        StartCoroutine(AutoDestroy());
    }

    void FixedUpdate()
    {
        Vector2 moviment = movimentVelocity * Time.fixedDeltaTime * movementDirection;
        rb.MovePosition((Vector2)this.transform.position + moviment);
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

    public void InitializeProjectile(Vector2 movementDirection)
    {
        this.movementDirection = movementDirection;
        rotateObject.StartRotation(movementDirection);
    }
}