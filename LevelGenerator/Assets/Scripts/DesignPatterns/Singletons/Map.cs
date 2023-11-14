using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : Singleton<Map>
{
    HashSet<Position> roomPositions = new();

    public HashSet<Position> RoomPositions { get => roomPositions; set => roomPositions = value; }
}
