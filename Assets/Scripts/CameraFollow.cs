using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

/* TODO: add smooth follow */

public class CameraFollow : MonoBehaviour
{
    // Singleton pattern
    private static CameraFollow _instance;
    public static CameraFollow Instance { get { return _instance; } }

    public Transform target;
    private Vector3 offset;
    private Vector3 lastTargetPos;
    [Range(0, 1)]
    public float smoothing = 0.5f;

    private float orthographicSize;
    private float zoomedOrthographicSize ;
    private float zoomT = 0.002f;

    private void Awake()
	{
        // Singleton pattern
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

	// Start is called before the first frame update
	void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;
        if(target != null)
            offset = transform.position - target.position;

        orthographicSize = Camera.main.orthographicSize;
        zoomedOrthographicSize = orthographicSize * 0.5f;
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
            lastTargetPos = target.position;
        }
    }

    public void GameOver()
	{
        StartCoroutine(SmoothZoomTowardsPlayer());
	}
    
    IEnumerator SmoothZoomTowardsPlayer()
    {
        PostProcessVolume postProcess = Camera.main.GetComponentInChildren<PostProcessVolume>();
        float newVignetteIntensity = postProcess.profile.GetSetting<Vignette>().intensity.value * 1.2f;

        bool condition = true;
        while(condition) // TODO: set condition
        {
            // Lerp towards goal position (zooming and translating)
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoomedOrthographicSize, zoomT);

            // Increase vignette post processing
            float oldVignetteIntensity = postProcess.profile.GetSetting<Vignette>().intensity.value;
            postProcess.profile.GetSetting<Vignette>().intensity.value = Mathf.Lerp(oldVignetteIntensity, newVignetteIntensity, zoomT);

            Vector3 curr = transform.position;
            Vector3 goal = lastTargetPos + offset + Vector3.down; // look a bit down
            Vector3 nextPos = new Vector3(
                Mathf.Lerp(curr.x, goal.x, zoomT),
                Mathf.Lerp(curr.y, goal.y, zoomT),
                Mathf.Lerp(curr.z, goal.z, zoomT));

            // When to end the loop
            float diffMove = (curr - goal).magnitude;
            float diffZoom = Mathf.Abs(Camera.main.orthographicSize - zoomedOrthographicSize);
            float diffVignette = Mathf.Abs(Camera.main.orthographicSize - zoomedOrthographicSize);
            if (diffZoom <= 0.1 && diffMove <= 0.1)
                condition = false;

            transform.position = nextPos;

            yield return new WaitForEndOfFrame();
        }
    }
}
