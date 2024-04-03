using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecorder : MonoBehaviour
{
    InputManager inputManager;

    public bool EnemiesInRoom { get; set; }
    public float Time { get; set; }

    void Start()
    {
        inputManager = FindObjectOfType<InputManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inputManager.IsInputEnabled())
        {
            return;
        }

        if (!EnemiesInRoom)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Debug.Log("time: " + Time);

        if (horizontal != 0f || vertical != 0f)
        {
            Time += UnityEngine.Time.deltaTime;
        }
        else if (AttackButtonsConstants.IsAttackPressed())
        {
            Time += UnityEngine.Time.deltaTime;
        }
    }
}
