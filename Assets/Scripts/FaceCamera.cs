using System.Collections.Generic;
using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform);
        var rotation = transform.rotation.eulerAngles;

        rotation.y = 0;
        transform.rotation = Quaternion.Euler(rotation);
    }
}
