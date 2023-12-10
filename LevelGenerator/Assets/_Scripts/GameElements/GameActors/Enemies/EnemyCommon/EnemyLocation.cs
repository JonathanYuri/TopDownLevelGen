using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLocation : MonoBehaviour
{
    Location location;

    public Position RoomPosition
    {
        get
        {
            if (location == null)
            {
                location = GetComponentInParent<Location>();
            }
            return location.RoomPosition;
        }
    }

    public bool IsInPlayerRoom(Position playerRoom) => RoomPosition.Equals(playerRoom);
}
