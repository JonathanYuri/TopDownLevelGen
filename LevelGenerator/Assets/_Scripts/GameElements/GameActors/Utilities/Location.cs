using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    Position roomPosition;
    bool justEnteredRoom;

    [SerializeField] Timer entryTimer;

    public Position RoomPosition
    {
        get => roomPosition;
        set
        {
            roomPosition = value;
            if (entryTimer != null)
            {
                JustEnteredRoom = true;
                entryTimer.StartTimer();
            }
        }
    }

    public bool JustEnteredRoom { get => justEnteredRoom; set => justEnteredRoom = value; }

    void Start()
    {
        if (entryTimer != null)
        {
            entryTimer.OnTimerExpired += OnEntryTimerExpired;
        }
    }

    void OnDestroy()
    {
        if (entryTimer != null)
        {
            entryTimer.OnTimerExpired -= OnEntryTimerExpired;
        }
    }

    void OnEntryTimerExpired()
    {
        JustEnteredRoom = false;
    }
}
