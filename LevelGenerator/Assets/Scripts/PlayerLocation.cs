using UnityEngine;

public class PlayerLocation
{
    public PlayerController player;
    public GameObject playerPrefab;
    public Position atRoom;
    
    public void SetPlayerToRoom(Position roomPosition)
    {
        //Debug.LogWarning("Position: " + room.Row + " x " + room.Column);
        player.transform.position =
            (Vector2)playerPrefab.transform.position +
            new Vector2((int)(GameConstants.Cols / 2), -(GameConstants.Rows - 2)) +
            Utils.TransformAMapPositionIntoAUnityPosition(roomPosition);
        atRoom = roomPosition;
    }

    public void TranslatePlayerToDirectionOfRoom(Vector3 direction, Camera camera)
    {
        Position roomPosition = new()
        {
            Row = atRoom.Row + (int)direction.y,
            Column = atRoom.Column + (int)direction.x
        };

        Vector3 unityPosition = Utils.TransformAMapPositionIntoAUnityPosition(roomPosition);
        camera.transform.position = new Vector3(unityPosition.x, unityPosition.y, camera.transform.position.z);
        SetPlayerToRoom(roomPosition);
    }
}
