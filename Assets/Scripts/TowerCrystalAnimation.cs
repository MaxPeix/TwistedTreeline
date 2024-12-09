using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerCrystalAnimation : MonoBehaviour
{
    public float rotationSpeed;
    public float bobAmplitude;
    public float bobFrequency;

    private Transform startPos;

    void Start()
    {
        startPos = transform;
    }

    void Update()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);
        BobCrystal();
    }

    void BobCrystal()
    {
        var bobbingPos = Vector3.up * Mathf.Cos(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(transform.position.x,  bobbingPos.y, transform.position.z);
    }
}
