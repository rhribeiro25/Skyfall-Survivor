using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    PlayerController player;
    Vector3 offset;

    // Use this for initialization
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
        offset = player.transform.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = player.transform.position - offset;
        }
    }
}