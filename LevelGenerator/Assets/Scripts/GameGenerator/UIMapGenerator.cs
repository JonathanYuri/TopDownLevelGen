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
    Image roomPanelImage;
    [SerializeField] GameObject playerInRoomPanel;
    Image playerInRoomImage;
    [SerializeField] GameObject blankSpacePrefab;

    Dictionary<Position, Image> uiMap;

    void Awake()
    {
        roomPanelImage = roomPanelPrefab.GetComponent<Image>();
        playerInRoomImage = playerInRoomPanel.GetComponent<Image>();
    }

    void DestroyPastUIMap()
    {
        for (int i = 0; i < mapHolder.transform.childCount; i++)
        {
            Destroy(mapHolder.transform.GetChild(i).gameObject);
        }
    }

    public void CreateUIMap(HashSet<Position> map, PlayerLocation playerLocation)
    {
        DestroyPastUIMap();

        uiMap = new();
        RectTransform mapHolderRect = mapHolder.GetComponent<RectTransform>();

        int maxX = map.Max(room => room.X);
        int maxY = map.Max(room => room.Y);

        int minX = map.Min(room => room.X);
        int minY = map.Min(room => room.Y);

        int mapWidth = maxY - minY;
        int mapHeight = maxX - minX;

        int maxSize = Mathf.Max(mapWidth + 1, mapHeight + 1);
        float roomSize = mapHolderRect.rect.size.x / (float)maxSize;

        //Debug.LogError("SIZE MAPHOLDER: " + mapHolderRect.rect.size);
        //Debug.LogError("ROOM SIZE: " + roomSize);
        //Debug.LogError("Quantas rooms?: " + maxSize);

        float initialPosition = -mapHolderRect.rect.size.x / 2 + roomSize / 2;

        float horizontalRoomPosition = initialPosition;
        float verticalRoomPosition = initialPosition; // comeca de baixo e vai subindo

        for (int j = minY; j <= maxSize - 1 + minY; j++)
        {
            for (int i = minX; i <= maxSize - 1 + minX; i++)
            {
                Position position = new() { X = i, Y = j };

                GameObject roomPanel = ChoosePanelToPosition(map, playerLocation, position);

                uiMap.Add(position, roomPanel.GetComponent<Image>());

                RectTransform roomRectTransform = roomPanel.GetComponent<RectTransform>();
                roomPanel.transform.localScale = new Vector3(roomSize / mapHolderRect.rect.size.x, roomSize / mapHolderRect.rect.size.x, 1);
                roomPanel.transform.localPosition = new Vector3(horizontalRoomPosition, verticalRoomPosition);

                //Debug.LogError($"i {i} e j {j}, horizontal: {horizontalRoomPosition} vertical {verticalRoomPosition}");
                horizontalRoomPosition += roomSize;
            }
            horizontalRoomPosition = initialPosition;
            verticalRoomPosition += roomSize;
        }
    }

    GameObject ChoosePanelToPosition(HashSet<Position> map, PlayerLocation playerLocation, Position position)
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

    public void UpdateUIMap(Position playerOldPosition, Position playerNewPosition)
    {
        uiMap[playerOldPosition].color = roomPanelImage.color;
        uiMap[playerNewPosition].color = playerInRoomImage.color;
    }
}
