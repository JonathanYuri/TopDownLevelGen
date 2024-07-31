using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public int Id { get; set; }
    public List<RoomContents> Enemies { get; set; }
    public List<RoomContents> Obstacles { get; set; }
    public float Difficulty { get; set; }

    void Awake()
    {
        Enemies = new();
        Obstacles = new();
    }
}
