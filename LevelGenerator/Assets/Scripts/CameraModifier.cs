using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraModifier : MonoBehaviour
{
    new Camera camera;

    private void Awake()
    {
        camera = GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (camera.orthographicSize == 5)
            {
                camera.orthographicSize = 30;
            }
            else
            {
                camera.orthographicSize = 5;
            }
        }
    }
}
