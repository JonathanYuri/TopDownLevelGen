using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    [SerializeField] float rotationVelocity = 3f;

    void Update()
    {
        this.transform.Rotate(Vector3.up, rotationVelocity * Time.deltaTime);
    }
}
