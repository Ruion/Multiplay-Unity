using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target { get; set; }
    public Vector3 cameraOffset;
    public float smoothFactor = 0.5f;

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, target.position + cameraOffset, smoothFactor);
    }
}