using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* TODO: add smooth follow */

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    private Vector3 offset;
    [Range(0, 1)]
    public float smoothing = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;
        if(target != null)
            offset = transform.position - target.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Linearly interpolates between current position and goal position with given parameters
        if (target != null)
		{
            Vector3 curr = transform.position;
            Vector3 goal = target.position + offset; //0-1 -> 0-0.2 inverse
            float t = 0.105f - (Mathf.Clamp01(smoothing) / 10f); // Transform 0-1 slider to 0.05-0.105 for interpolating
            Vector3 nextPos = new Vector3(
                curr.x,
                curr.y,
                Mathf.Lerp(curr.z, goal.z, t));
            transform.position = nextPos;
        }
    }
}
