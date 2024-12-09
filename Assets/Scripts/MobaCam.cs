using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobaCam : MonoBehaviour
{
    public float scrollSpeed = 15;
    public float scrollEdge = 0.01f;
    public GameObject player;
    private bool locked = true;

    void Start()
    {
        // Set the camera to the player's position
        transform.position = player.transform.position +  new Vector3(0, 20, 0);
    }

    // Update is called once per frame
    void Update()
    {
        // Move camera with mouse position
        Vector3 pos = transform.position;

        if (Input.mousePosition.y >= Screen.height * (1 - scrollEdge))
        {
            pos.z -= scrollSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.y <= Screen.height * scrollEdge)
        {
            pos.z += scrollSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.x >= Screen.width * (1 - scrollEdge))
        {
            pos.x -= scrollSpeed * Time.deltaTime;
        }

        if (Input.mousePosition.x <= Screen.width * scrollEdge)
        {
            pos.x += scrollSpeed * Time.deltaTime;
        }

        // Apply the updated position back to the transform
        transform.position = pos;

        // Center the camera on the player when Space is pressed
        if (Input.GetKey(KeyCode.Space))
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 20, player.transform.position.z);
        }

        // Lock the camera to the player when Y is pressed
        if (Input.GetKeyDown(KeyCode.Y))
        {
            locked = !locked;
        }

        if (locked)
        {
            transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 20, player.transform.position.z);
        }
    }
}
