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

    Dictionary<Position, Image> uiMap;
    HashSet<Position> map;

    LevelGenerator levelGenerator;
    PlayerLocationManager playerLocationManager;

    void Awake()
    {
        uiMap = new();
        roomPanelImage = roomPanelPrefab.GetComponent<Image>();
        playerInRoomImage = playerInRoomPanel.GetComponent<Image>();
    }

    void Start()
    {
        playerLocationManager = FindObjectOfType<PlayerLocationManager>();
        levelGenerator = FindObjectOfType<LevelGenerator>();
        levelGenerator.OnLevelGenerated += OnLevelGenerated;
    }

    void OnDestroy()
    {
        if (levelGenerator != null)
        {
            levelGenerator.OnLevelGenerated -= OnLevelGenerated;
        }
    }

    void OnLevelGenerated()
    {
        CreateUIMap();
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
    public void CreateUIMap()
    {
        this.map = GameMapSingleton.Instance.RoomPositions;
        DestroyPastUIMap();

        RectTransform mapHolderRect = mapHolder.GetComponent<RectTransform>();

        int mapSize = CalculateMapSize();
        float roomSize = CalculateRoomSize(mapSize, mapHolderRect);
        Vector2 roomScale = new(roomSize / mapHolderRect.rect.size.x, roomSize / mapHolderRect.rect.size.y);

        Vector2 initialRoomPosition = CalculateInitialRoomPosition(mapHolderRect, roomSize);

        CreateRoomPanels(mapSize, roomSize, roomScale, initialRoomPosition);
    }

    /// <summary>
    /// Calculates the size of the game map based on the positions of rooms in the map.
    /// </summary>
    /// <returns>The size of the game map.</returns>
    int CalculateMapSize()
    {
        (int maxX, int minX) = map.MaxAndMin(room => room.X);
        (int maxY, int minY) = map.MaxAndMin(room => room.Y);

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
    /// <param name="mapSize">The size of the map.</param>
    /// <param name="roomSize">The size of each room panel.</param>
    /// <param name="roomScale">The scale to apply to each room panel.</param>
    /// <param name="initialRoomPosition">The initial position of the first room panel.</param>
    void CreateRoomPanels(int mapSize, float roomSize, Vector2 roomScale, Vector2 initialRoomPosition)
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

                InstantiatePanelForPosition(position, roomScale, horizontalRoomPosition, verticalRoomPosition);
                horizontalRoomPosition += roomSize;
            }
            horizontalRoomPosition = initialRoomPosition.x;
            verticalRoomPosition += roomSize;
        }
    }

    void InstantiatePanelForPosition(Position position, Vector2 roomScale, float horizontalRoomPosition, float verticalRoomPosition)
    {
        GameObject roomPanel = ChoosePanelToPosition(position);
        GameObject roomPanelInGame = Instantiate(roomPanel, mapHolder.transform);

        roomPanelInGame.transform.localScale = new Vector3(roomScale.x, roomScale.y, 1);
        roomPanelInGame.transform.localPosition = new Vector3(horizontalRoomPosition, verticalRoomPosition);

        uiMap.Add(position, roomPanelInGame.GetComponent<Image>());
    }

    /// <summary>
    /// Chooses the appropriate room panel based on the given position and player location.
    /// </summary>
    /// <param name="position">The position for which to select a room panel.</param>
    /// <returns>The selected room panel GameObject.</returns>
    GameObject ChoosePanelToPosition(Position position)
    {
        if (playerLocationManager.PlayerLocation.RoomPosition.Equals(position))
        {
            return playerInRoomPanel;
        }

        if (map.Contains(position))
        {
            return roomPanelPrefab;
        }

        return blankSpacePrefab;
    }

    /// <summary>
    /// Updates the UI map by changing the color of room panels to indicate the player's movement.
    /// </summary>
    /// <param name="playerOldPosition">The previous position of the player.</param>
    public void UpdateUIMap(Position playerOldPosition, Position playerNewPosition)
    {
        uiMap[playerOldPosition].color = roomPanelImage.color;
        uiMap[playerNewPosition].color = playerInRoomImage.color;
    }
}
