using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(PlayerLocationManager))]
public class PlayerManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;

    PlayerLocationManager playerLocationManager;
    GameManager gameManager;
    UIMapGenerator uiMapGenerator;

    public PlayerController Player { get; set; }

    void Awake()
    {
        playerLocationManager = GetComponent<PlayerLocationManager>();
    }

    void Start()
    {
        SpawnPlayer();
        gameManager = FindObjectOfType<GameManager>();
        uiMapGenerator = FindObjectOfType<UIMapGenerator>();
    }

    void OnDestroy()
    {
        if (Player != null)
        {
            Player.PassedThroughTheDoorEvent -= PlayerPassedThroughTheDoor;
            Player.OnLevelComplete -= OnLevelComplete;
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObject = Instantiate(playerPrefab);
        PlayerController playerController = playerObject.GetComponent<PlayerController>();
        Player = playerController;

        playerLocationManager.Player = Player;
        Player.PassedThroughTheDoorEvent += PlayerPassedThroughTheDoor;
        Player.OnLevelComplete += OnLevelComplete;
    }

    void PlayerPassedThroughTheDoor(object player, DoorEventArgs doorEventArgs)
    {
        Position playerOldPosition = playerLocationManager.Location.RoomPosition;
        playerLocationManager.TranslatePlayerToDirectionOfRoom(doorEventArgs.doorDirection);
        uiMapGenerator.UpdateUIMap(playerOldPosition, playerLocationManager.Location.RoomPosition);
    }

    void OnLevelComplete()
    {
        gameManager.LoadNextLevel();
    }
}
