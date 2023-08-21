using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float movementSpeed;

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Capturando os inputs do teclado
        float movimentoHorizontal = Input.GetAxis("Horizontal");
        float movimentoVertical = Input.GetAxis("Vertical");

        // Calculando o vetor de movimento
        Vector2 movimento = new(movimentoHorizontal, movimentoVertical);

        // Normalizando o vetor de movimento para evitar movimento diagonal mais rápido
        movimento.Normalize();

        // Aplicando a força para mover o jogador
        rb.velocity = movimento * movementSpeed;
    }
}
