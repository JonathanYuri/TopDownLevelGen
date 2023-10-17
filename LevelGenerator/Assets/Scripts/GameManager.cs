using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    PlayerController player;
    PlayerLocation playerLocation;

    Camera sceneCamera;

    UIMapGenerator uiMapGenerator;
    LevelGenerator levelGenerator;

    HashSet<Position> map;

    void Start()
    {
        sceneCamera = FindFirstObjectByType<Camera>();
        uiMapGenerator = FindFirstObjectByType<UIMapGenerator>();
        levelGenerator = FindFirstObjectByType<LevelGenerator>();

        GenerateGame();
    }

    void GenerateGame()
    {
        map = levelGenerator.Generate();
        if (player == null)
        {
            SpawnPlayer();
        }
        playerLocation ??= new(player, playerPrefab);
        playerLocation.SetPlayerToInitialRoom(map.ElementAt(0), sceneCamera);
        uiMapGenerator.CreateUIMap(map, playerLocation);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.PassedThroughTheDoorEvent -= Player_PassedThroughTheDoor;
            player.OnLevelComplete -= Player_OnLevelComplete;
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObject = Instantiate(
            playerPrefab
        );
        player = playerObject.GetComponent<PlayerController>();
        player.PassedThroughTheDoorEvent += Player_PassedThroughTheDoor;
        player.OnLevelComplete += Player_OnLevelComplete;
    }

    void Player_OnLevelComplete()
    {
        GenerateGame();
    }

    void Player_PassedThroughTheDoor(object player, DoorEventArgs doorEventArgs)
    {
        Position playerOldPosition = playerLocation.atRoom;
        playerLocation.TranslatePlayerToDirectionOfRoom(doorEventArgs.doorDirection, sceneCamera);
        Position playerNewPosition = playerLocation.atRoom;

        uiMapGenerator.UpdateUIMap(playerOldPosition, playerNewPosition);
    }
}
