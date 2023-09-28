using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorEventArgs : EventArgs
{
    public Vector3 doorDirection;
}

public class Door : MonoBehaviour
{
    public Vector3 direction;

    static readonly Dictionary<Vector3, Vector3> RotationToDirectionMap = new()
    {
        { new Vector3(0, 0, 270), Vector3.down },
        { new Vector3(0, 0, 90), Vector3.up },
        { new Vector3(0, 0, 0), Vector3.right },
        { new Vector3(0, 0, 180), Vector3.left }
    };

    private void Awake()
    {
        Vector3 rotation = transform.eulerAngles;
        if (RotationToDirectionMap.TryGetValue(rotation, out Vector3 mappedDirection))
        {
            direction = mappedDirection;
        }
        else
        {
            throw new ArgumentException("Direcao de porta desconhecida: " + rotation);
        }
    }
}
