using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct DoorDirectionBasedOnRotation
{
    public static Vector3 ToDown = new(0, 0, 270);
    public static Vector3 ToUp = new(0, 0, 90);
    public static Vector3 ToRight = new(0, 0, 0);
    public static Vector3 ToLeft = new(0, 0, 180);
}

public class DoorEventArgs : EventArgs
{
    public Vector3 doorDirection;
}

public class Door : MonoBehaviour
{
    public Vector3 direction;

    private void Awake()
    {
        Vector3 rotation = new (this.transform.eulerAngles.x, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
        if (rotation.Equals(DoorDirectionBasedOnRotation.ToDown))
        {
            direction = Vector3.down;
        }
        else if (rotation.Equals(DoorDirectionBasedOnRotation.ToUp))
        {
            direction = Vector3.up;
        }
        else if (rotation.Equals(DoorDirectionBasedOnRotation.ToLeft))
        {
            direction = Vector3.left;
        }
        else if (rotation.Equals(DoorDirectionBasedOnRotation.ToRight))
        {
            direction = Vector3.right;
        }
        else
        {
            throw new ArgumentException("Direcao de porta desconhecida: " + rotation);
        }
    }
}
