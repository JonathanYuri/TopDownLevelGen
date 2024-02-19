using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeRecorder : MonoBehaviour
{
    float time = 0f;

    bool enemiesInRoom = false;

    PlayerController playerController;
    RoomConclusionManager roomConclusionManager;
    APISender apiSender;

    void Start()
    {
        apiSender = FindObjectOfType<APISender>();

        roomConclusionManager = FindObjectOfType<RoomConclusionManager>();
        roomConclusionManager.OnRoomDoorsOpened += OnRoomDoorsOpened;

        playerController = FindObjectOfType<PlayerController>();
        playerController.PassedThroughTheDoorEvent += PlayerPassedThroughTheDoor;
    }

    // Update is called once per frame
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // foi concluida a sala
        if (!enemiesInRoom)
        {
            return;
        }

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
        enemiesInRoom = true;
    }

    void OnRoomDoorsOpened()
    {
        enemiesInRoom = false;

        apiSender.SendRoomConclusionData(time);
    }
}
