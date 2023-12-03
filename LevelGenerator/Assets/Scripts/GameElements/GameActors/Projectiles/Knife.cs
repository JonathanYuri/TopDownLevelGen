using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RotateObject))]
[RequireComponent(typeof(Rigidbody2D))]
public class Knife : MonoBehaviour
{
    Rigidbody2D rb;
    TriggerDamage triggerDamage;
    RotateObject rotateObject;

    [SerializeField] float movimentVelocity = 2.0f;
    [SerializeField] float timeToAutoDestroy = 4.0f;

    Vector2 movementDirection;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rotateObject = GetComponent<RotateObject>();
        triggerDamage = GetComponentInChildren<TriggerDamage>();
        triggerDamage.CollisionOccured += CollisionOccured;
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

    public void SetInitialParams(Vector2 movementDirection)
    {
        this.movementDirection = movementDirection;
        rotateObject.StartRotation(movementDirection);
    }
}
