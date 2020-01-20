using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasMoving : MonoBehaviour
{
    public Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(transform.position + playerCamera.transform.rotation * Vector3.back,
            playerCamera.transform.rotation * Vector3.down);
    }
}
