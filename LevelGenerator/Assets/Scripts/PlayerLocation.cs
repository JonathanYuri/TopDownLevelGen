using System.Collections.Generic;
using UnityEngine;

public class PlayerLocation
{
    PlayerController player;
    GameObject playerPrefab;
    public Position atRoom;

    Dictionary<Vector3, Vector2> directionToPositionInRoomMatrix;

    public PlayerLocation(PlayerController player, GameObject playerPrefab)
    {
        this.player = player;
        this.playerPrefab = playerPrefab;
        directionToPositionInRoomMatrix = new()
        {
            // se estou indo pra baixo spawno o jogador na parte de cima da sala
            { Vector3.down, new Vector2((int)(GameConstants.Cols / 2), -1) }, // cima meio, to vindo de cima
            { Vector3.left, new Vector2((GameConstants.Cols - 2), -(int)(GameConstants.Rows / 2)) }, // direita meio, to vindo da direita
            { Vector3.right, new Vector2(1, -(int)(GameConstants.Rows / 2)) }, // esq meio, to vindo da esq
            { Vector3.up, new Vector2((int)(GameConstants.Cols / 2), -(GameConstants.Rows - 2)) }, // baixo meio, to vindo de baixo
        };
    }

    public void SetPlayerToRoom(Position roomPosition, Vector2 positionInRoomMatrix)
    {
        //Debug.LogWarning("Position: " + room.Row + " x " + room.Column);
        player.transform.position =
            (Vector2)playerPrefab.transform.position +
            positionInRoomMatrix +
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
        SetPlayerToRoom(roomPosition, directionToPositionInRoomMatrix[direction]);
    }
}
