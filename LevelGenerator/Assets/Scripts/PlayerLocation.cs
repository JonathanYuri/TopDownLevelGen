using System.Collections.Generic;
using UnityEngine;

public class PlayerLocation
{
    PlayerController player;
    public Position atRoom;

    Dictionary<Vector3, Vector2> directionToPositionInRoomMatrix;

    public PlayerLocation(PlayerController player, GameObject playerPrefab)
    {
        this.player = player;
        directionToPositionInRoomMatrix = new()
        {
            // se estou indo pra baixo spawno o jogador na parte de cima da room
            { Vector3.down, new Vector2(0, GameConstants.RoomMiddle.Y - 1) }, // cima meio, to vindo de cima
            { Vector3.left, new Vector2(GameConstants.RoomMiddle.X - 1, 0) }, // direita meio, to vindo da direita
            { Vector3.right, new Vector2(-GameConstants.RoomMiddle.X + 1, 0)}, // esq meio, to vindo da esq
            { Vector3.up, new Vector2(0, -GameConstants.RoomMiddle.Y + 1) }, // baixo meio, to vindo de baixo
        };
    }

    public void SetPlayerToRoom(Position roomPosition, Vector2 positionInRoomMatrix)
    {
        //Debug.LogWarning("Position: " + room.Row + " x " + room.Column);
        player.transform.position = positionInRoomMatrix;
        atRoom = roomPosition;
    }

    public void TranslatePlayerToDirectionOfRoom(Vector3 direction, Camera camera)
    {
        Position roomPosition = new()
        {
            X = atRoom.X + (int)direction.x,
            Y = atRoom.Y + (int)direction.y
        };

        Vector2 roomInUnity = Utils.TransformAMapPositionIntoAUnityPosition(roomPosition);
        camera.transform.position = new Vector3(roomInUnity.x, roomInUnity.y, camera.transform.position.z);
        SetPlayerToRoom(roomPosition, roomInUnity + directionToPositionInRoomMatrix[direction]);
    }
}
