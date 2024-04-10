using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    void Update()
    {
        if (player != null)
        {
            Vector3 newPosition = player.position + offset;
            transform.position = newPosition;
        }
    }
}