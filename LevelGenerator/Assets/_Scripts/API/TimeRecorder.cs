using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecorder : MonoBehaviour
{
    float time = 0f;

    bool enemiesInRoom = false;

    PlayerController playerController;
    RoomConclusionManager roomConclusionManager;
    PlayerLocationManager playerLocationManager;
    InputManager inputManager;
    APISender apiSender;

    void Start()
    {
        apiSender = FindObjectOfType<APISender>();

        roomConclusionManager = FindObjectOfType<RoomConclusionManager>();
        roomConclusionManager.OnRoomDoorsOpened += OnRoomDoorsOpened;

        playerLocationManager = FindObjectOfType<PlayerLocationManager>();
        inputManager = FindObjectOfType<InputManager>();

        playerController = FindObjectOfType<PlayerController>();
        playerController.PassedThroughTheDoorEvent += PlayerPassedThroughTheDoor;
    }

    // Update is called once per frame
    void Update()
    {
        if (!inputManager.IsInputEnabled())
        {
            return;
        }

        if (!enemiesInRoom)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Debug.Log("time: " + time);

        if (horizontal != 0f || vertical != 0f)
        {
            time += Time.deltaTime;
        }
        else if (AttackButtonsConstants.IsAttackPressed())
        {
            time += Time.deltaTime;
        }
    }

    void PlayerPassedThroughTheDoor(object player, DoorEventArgs doorEventArgs)
    {
        // se for a sala final nao tem inimigos
        if (playerLocationManager.PlayerLocation.RoomPosition.Equals(GameMapSingleton.Instance.FinalRoomPosition))
        {
            enemiesInRoom = false;
        }
        else
        {
            enemiesInRoom = true;
        }
    }

    void OnRoomDoorsOpened()
    {
        enemiesInRoom = false;
        apiSender.SendRoomConclusionData(time);
        time = 0;
    }
}
