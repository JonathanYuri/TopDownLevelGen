using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Location))]
public class PlayerLocationManager : MonoBehaviour
{
    static readonly Dictionary<Vector3, Vector2> directionToPositionInRoomMatrix = new()
    {
        // se estou indo pra baixo spawno o jogador na parte de cima da room
        { Vector3.down, new Vector2(0, GameConstants.ROOM_MIDDLE.Y - 1) }, // cima meio, to vindo de cima
        { Vector3.left, new Vector2(GameConstants.ROOM_MIDDLE.X - 1, 0) }, // direita meio, to vindo da direita
        { Vector3.right, new Vector2(-GameConstants.ROOM_MIDDLE.X + 1, 0)}, // esq meio, to vindo da esq
        { Vector3.up, new Vector2(0, -GameConstants.ROOM_MIDDLE.Y + 1) }, // baixo meio, to vindo de baixo
    };

    public PlayerController Player { get; set; }
    public Location Location { get; private set; }

    void Awake()
    {
        Location = GetComponent<Location>();
    }

    public void SetPlayerToInitialRoom(Camera camera, Position initialPosition)
    {
        camera.transform.position = new Vector3(0, 0, camera.transform.position.z);
        SetPlayerToRoom(initialPosition, new Vector3(0, 0));
    }

    public void SetPlayerToRoom(Position roomPosition, Vector2 positionInRoomMatrix)
    {
        Player.transform.position = positionInRoomMatrix;
        Location.RoomPosition = roomPosition;
    }

    /// <summary>
    /// Translates the player's position to a neighboring room based on the specified direction.
    /// </summary>
    /// <param name="direction">The direction of movement.</param>
    /// <param name="camera">The game camera.</param>
    public void TranslatePlayerToDirectionOfRoom(Vector3 direction, Camera camera)
    {
        Position roomPosition = new()
        {
            X = Location.RoomPosition.X + (int)direction.x,
            Y = Location.RoomPosition.Y + (int)direction.y
        };

        Vector2 roomInUnity = Utils.TransformAMapPositionIntoAUnityPosition(roomPosition);
        camera.transform.position = new Vector3(roomInUnity.x, roomInUnity.y, camera.transform.position.z);
        SetPlayerToRoom(roomPosition, roomInUnity + directionToPositionInRoomMatrix[direction]);
    }
}
