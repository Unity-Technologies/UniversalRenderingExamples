using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxCam : MonoBehaviour
{
    public Transform playerCam;

    public float proportionality;

    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = startPos + playerCam.position * proportionality;
        transform.rotation = playerCam.rotation;
    }
}
