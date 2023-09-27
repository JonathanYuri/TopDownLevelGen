using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIMapGenerator : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] GameObject mapHolder;
    [SerializeField] GameObject roomPanelPrefab;
    [SerializeField] GameObject blankSpacePrefab;
    [SerializeField] GameObject playerInRoomPanel;

    PlayerLocation playerLocation;

    Dictionary<Position, GameObject> uiMap;

    public void CreateUIMap(HashSet<Position> map, PlayerLocation playerLocation)
    {
        uiMap = new();
        this.playerLocation = playerLocation;
        RectTransform mapHolderRect = mapHolder.GetComponent<RectTransform>();

        int maxRow = map.Max(room => room.Row);
        int maxColumn = map.Max(room => room.Column);

        int minRow = map.Min(room => room.Row);
        int minColumn = map.Min(room => room.Column);

        int mapWidth = maxColumn - minColumn;
        int mapHeight = maxRow - minRow;

        int maxSize = Mathf.Max(mapWidth + 1, mapHeight + 1);
        float roomSize = mapHolderRect.rect.size.x / (float)maxSize;

        Debug.LogError("SIZE MAPHOLDER: " + mapHolderRect.rect.size);
        Debug.LogError("ROOM SIZE: " + roomSize);
        Debug.LogError("Quantas salas?: " + maxSize);

        Debug.LogError("mapa: ");
        foreach (Position pos in map)
        {
            Debug.LogError($"x: {pos.Row}, y: {pos.Column}");
        }

        float initialPosition = -mapHolderRect.rect.size.x / 2 + roomSize / 2;

        float horizontalRoomPosition = initialPosition;
        float verticalRoomPosition = -initialPosition; // comeca de cima e vai descendo

        // pra preencher o mapPanel todo
        int maxMapColumn = mapWidth >= mapHeight ? maxColumn : minColumn + mapHeight;
        int minMapRow = mapHeight >= mapWidth ? minRow : maxRow - mapWidth;

        for (int i = maxRow; i >= minMapRow; i--)
        {
            for (int j = minColumn; j <= maxMapColumn; j++)
            {
                Position position = new() { Row = i, Column = j };
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

                uiMap.Add(position, roomPanel);

                RectTransform roomRectTransform = roomPanel.GetComponent<RectTransform>();
                roomPanel.transform.localScale = new Vector3(roomSize / mapHolderRect.rect.size.x, roomSize / mapHolderRect.rect.size.x, 1);
                roomPanel.transform.localPosition = new Vector3(horizontalRoomPosition, verticalRoomPosition);

                Debug.LogError($"i {i} e j {j}, horizontal: {horizontalRoomPosition} vertical {verticalRoomPosition}");
                horizontalRoomPosition += roomSize;
            }
            horizontalRoomPosition = initialPosition;
            verticalRoomPosition -= roomSize;
        }
    }

    public void UpdateUIMap(Position playerOldPosition, Position playerNewPosition)
    {
        uiMap[playerOldPosition].GetComponent<Image>().color = roomPanelPrefab.GetComponent<Image>().color;
        uiMap[playerNewPosition].GetComponent<Image>().color = playerInRoomPanel.GetComponent<Image>().color;
    }
}
