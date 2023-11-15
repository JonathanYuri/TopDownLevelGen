using System.Collections.Generic;

public class Map : Singleton<Map>
{
    public HashSet<Position> RoomPositions { get; set; } = new();
}
