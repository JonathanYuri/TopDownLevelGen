using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages the generation and display of the UI map, including player location and room status.
/// </summary>
public class UIMapGenerator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject mapHolder;
    [SerializeField] GameObject roomPanelPrefab;
    Image roomPanelImage;
    [SerializeField] GameObject playerInRoomPanel;
    Image playerInRoomImage;
    [SerializeField] GameObject blankSpacePrefab;

    PlayerLocation playerLocation;
    Dictionary<Position, Image> uiMap;

    void Awake()
    {
        uiMap = new();
        roomPanelImage = roomPanelPrefab.GetComponent<Image>();
        playerInRoomImage = playerInRoomPanel.GetComponent<Image>();
    }

    /// <summary>
    /// Destroys the previously generated UI map elements, clearing the map for regeneration.
    /// </summary>
    void DestroyPastUIMap()
    {
        uiMap.Clear();
        for (int i = 0; i < mapHolder.transform.childCount; i++)
        {
            Destroy(mapHolder.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// Creates the UI map to represent the game map with room panels and player's current position.
    /// </summary>
    /// <param name="map">A HashSet of Positions representing the rooms in the game map.</param>
    /// <param name="playerLocation">The current location of the player in the game map.</param>
    public void CreateUIMap(HashSet<Position> map, PlayerLocation playerLocation)
    {
        this.playerLocation = playerLocation;
        DestroyPastUIMap();

        RectTransform mapHolderRect = mapHolder.GetComponent<RectTransform>();

        int mapSize = CalculateMapSize(map);
        float roomSize = CalculateRoomSize(mapSize, mapHolderRect);
        Vector2 roomScale = new(roomSize / mapHolderRect.rect.size.x, roomSize / mapHolderRect.rect.size.y);

        Vector2 initialRoomPosition = CalculateInitialRoomPosition(mapHolderRect, roomSize);

        CreateRoomPanels(map, mapSize, roomSize, roomScale, initialRoomPosition);
    }

    /// <summary>
    /// Calculates the size of the game map based on the positions of rooms in the map.
    /// </summary>
    /// <param name="map">A HashSet of Positions representing the rooms in the game map.</param>
    /// <returns>The size of the game map.</returns>
    int CalculateMapSize(HashSet<Position> map)
    {
        int maxX = map.Max(room => room.X);
        int maxY = map.Max(room => room.Y);

        int minX = map.Min(room => room.X);
        int minY = map.Min(room => room.Y);

        int mapWidth = maxY - minY;
        int mapHeight = maxX - minX;

        return Mathf.Max(mapWidth + 1, mapHeight + 1);
    }

    /// <summary>
    /// Calculates the size of a room based on the size of the game map and the RectTransform of the map holder.
    /// </summary>
    /// <param name="mapSize">The size of the game map.</param>
    /// <param name="mapHolderRect">The RectTransform of the map holder.</param>
    /// <returns>The size of a room.</returns>
    float CalculateRoomSize(int mapSize, RectTransform mapHolderRect)
    {
        return mapHolderRect.rect.size.x / (float)mapSize;
    }

    /// <summary>
    /// Calculates the initial position of a room within the map holder based on the room size and the RectTransform of the map holder.
    /// </summary>
    /// <param name="mapHolderRect">The RectTransform of the map holder.</param>
    /// <param name="roomSize">The size of a room.</param>
    /// <returns>The initial position of a room.</returns>
    Vector2 CalculateInitialRoomPosition(RectTransform mapHolderRect, float roomSize)
    {
        return new Vector2(-mapHolderRect.rect.size.x / 2 + roomSize / 2, -mapHolderRect.rect.size.y / 2 + roomSize / 2);
    }

    /// <summary>
    /// Creates room panels for each room in the map, positioning them within the map holder based on the specified parameters.
    /// </summary>
    /// <param name="map">The set of room positions in the map.</param>
    /// <param name="mapSize">The size of the map.</param>
    /// <param name="roomSize">The size of each room panel.</param>
    /// <param name="roomScale">The scale to apply to each room panel.</param>
    /// <param name="initialRoomPosition">The initial position of the first room panel.</param>
    void CreateRoomPanels(HashSet<Position> map, int mapSize, float roomSize, Vector2 roomScale, Vector2 initialRoomPosition)
    {
        float horizontalRoomPosition = initialRoomPosition.x;
        float verticalRoomPosition = initialRoomPosition.y;

        int minX = map.Min(room => room.X);
        int minY = map.Min(room => room.Y);

        for (int j = minY; j <= mapSize - 1 + minY; j++)
        {
            for (int i = minX; i <= mapSize - 1 + minX; i++)
            {
                Position position = new() { X = i, Y = j };
                GameObject roomPanel = ChoosePanelToPosition(map, position);
                uiMap.Add(position, roomPanel.GetComponent<Image>());

                RectTransform roomRectTransform = roomPanel.GetComponent<RectTransform>();
                roomPanel.transform.localScale = new Vector3(roomScale.x, roomScale.y, 1);
                roomPanel.transform.localPosition = new Vector3(horizontalRoomPosition, verticalRoomPosition);

                horizontalRoomPosition += roomSize;
            }
            horizontalRoomPosition = initialRoomPosition.x;
            verticalRoomPosition += roomSize;
        }
    }

    /// <summary>
    /// Chooses the appropriate room panel based on the given position and player location.
    /// </summary>
    /// <param name="map">The set of room positions in the map.</param>
    /// <param name="position">The position for which to select a room panel.</param>
    /// <returns>The selected room panel GameObject.</returns>
    GameObject ChoosePanelToPosition(HashSet<Position> map, Position position)
    {
        GameObject roomPanel;
        if (playerLocation.atRoom.Equals(position))
        {
            roomPanel = Instantiate(playerInRoomPanel, mapHolder.transform);
        }
        else if (map.Contains(position))
        {
            roomPanel = Instantiate(roomPanelPrefab, mapHolder.transform);
        }
        else
        {
            roomPanel = Instantiate(blankSpacePrefab, mapHolder.transform);
        }
        return roomPanel;
    }

    /// <summary>
    /// Updates the UI map by changing the color of room panels to indicate the player's movement.
    /// </summary>
    /// <param name="playerOldPosition">The previous position of the player.</param>
    public void UpdateUIMap(Position playerOldPosition)
    {
        uiMap[playerOldPosition].color = roomPanelImage.color;
        uiMap[playerLocation.atRoom].color = playerInRoomImage.color;
    }
}
