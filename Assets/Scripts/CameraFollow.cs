using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO: add smooth follow */

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        if(target != null)
            offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
            transform.position = target.position + offset;
    }
}
