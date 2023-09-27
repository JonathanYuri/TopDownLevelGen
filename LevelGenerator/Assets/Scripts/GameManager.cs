using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GenerateGame generateGame; // TODO: nao precisa da variavel? so rodar e pegar o mapa eh o suficiente?
    [SerializeField] GameObject playerPrefab;
    PlayerController player;
    PlayerLocation playerLocation;

    Camera sceneCamera;

    UIMapGenerator uiMapGenerator;

    void Start()
    {
        sceneCamera = FindFirstObjectByType<Camera>();
        uiMapGenerator = FindFirstObjectByType<UIMapGenerator>();
        generateGame = FindFirstObjectByType<GenerateGame>();
        generateGame.Generate();
        SpawnPlayer();

        playerLocation = new()
        {
            player = player,
            playerPrefab = playerPrefab
        };
        playerLocation.SetPlayerToRoom(generateGame.mapa.ElementAt(0));
        uiMapGenerator.CreateUIMap(generateGame.mapa, playerLocation);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.PassedThroughTheDoorEvent -= Player_PassedThroughTheDoor;
        }
    }

    void SpawnPlayer()
    {
        GameObject playerObject = Instantiate(
            playerPrefab
        );
        player = playerObject.GetComponent<PlayerController>();
        player.PassedThroughTheDoorEvent += Player_PassedThroughTheDoor;
    }

    void Player_PassedThroughTheDoor(object player, DoorEventArgs doorEventArgs)
    {
        Position playerOldPosition = playerLocation.atRoom;
        playerLocation.TranslatePlayerToDirectionOfRoom(doorEventArgs.doorDirection, sceneCamera);
        Position playerNewPosition = playerLocation.atRoom;

        uiMapGenerator.UpdateUIMap(playerOldPosition, playerNewPosition);
    }
}
