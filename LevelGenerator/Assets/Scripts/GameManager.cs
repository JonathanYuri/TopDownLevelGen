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

    HashSet<Position> map;

    void Start()
    {
        GameConstants.InitializeDictionary();
        sceneCamera = FindFirstObjectByType<Camera>();
        uiMapGenerator = FindFirstObjectByType<UIMapGenerator>();
        map = FindFirstObjectByType<GameGenerator>().Generate();
        SpawnPlayer();

        playerLocation = new(player, playerPrefab);
        // + Utils.TransformAMapPositionIntoAUnityPosition(generateGame.mapa.ElementAt(1))
        playerLocation.SetPlayerToRoom(map.ElementAt(0), new Vector2(0, 0)); // spawnar no meio
        uiMapGenerator.CreateUIMap(map, playerLocation);
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
