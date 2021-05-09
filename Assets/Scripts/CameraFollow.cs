using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine;

/// <summary>
/// This is a one instance class (Singleton pattern) which is located on the camera and follows player around
/// Main features:
///  - Follows player on Z axis, but not on X (the level goes from -X to X and to infinite Z)
///  - Zooms on to players exact location once he dies
/// </summary>
public class CameraFollow : MonoBehaviour
{
    // Singleton pattern
    private static CameraFollow _instance;
    /// <summary>
    /// Singleton pattern static instance of this class
    /// </summary>
    public static CameraFollow Instance { get { return _instance; } }

    // Player transform which will be followed
    private Transform target;
    // Offset from player
    private Vector3 offset;
    // Last known target position, for when the player is removed (game over) and we need to zoom in to his position
    private Vector3 lastTargetPos;
    [Range(0, 1)]
    [Tooltip("Smoothing efect used in interpolation. 0 less smoothing 1 most smoothing")]
    public float smoothing = 0.5f;

    // Camera size normally and when zoomed, as well as interpolation factor for zooming
    private float orthographicSize;
    private float zoomedOrthographicSize;
    private float zoomInterpFactor = 0.002f;

    private void Awake()
	{
        // Singleton pattern
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;
    }

	void Start()
    {
        // If target is not defined find player object (has "Player" tag)
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player").transform;

        // If target is defined set offset from it
        if(target != null)
            offset = transform.position - target.position;

        // Keep track of original size and calculate zoomed size (for game over)
        orthographicSize = Camera.main.orthographicSize;
        zoomedOrthographicSize = orthographicSize * 0.5f;
    }

    void Update()
    {
        // Linearly interpolates between current position and goal position with given parameters
        if (target != null)
		{
            Vector3 curr = transform.position;
            Vector3 goal = target.position + offset;
            float t = 0.105f - (Mathf.Clamp01(smoothing) / 10f); // Transform 0-1 slider to 0.05-0.105 for interpolating
            Vector3 nextPos = new Vector3(curr.x, curr.y, Mathf.Lerp(curr.z, goal.z, t));
            transform.position = nextPos;
            lastTargetPos = target.position;
        }
    }

    /// <summary>
    /// Called when it is game over. Only zooms towards players last position
    /// </summary>
    public void GameOver()
	{
        // Implemented in coroutine so it zooms one step each frame
        StartCoroutine(SmoothZoomTowardsPlayer());
	}
    
    /// <summary>
    /// Slowly zoom towards players last position. Used in Game Over screen
    /// </summary>
    IEnumerator SmoothZoomTowardsPlayer()
    {
        // Calculate new vignette post processing effect (tunnel efect)
        PostProcessVolume postProcess = Camera.main.GetComponentInChildren<PostProcessVolume>();
        Vignette vignette = postProcess.profile.GetSetting<Vignette>();
        float newVignetteIntensity = vignette.intensity.value * 1.2f;

        // Condition used to stop the whie loop
        bool condition = true;
        while(condition) // TODO: set condition
        {
            // Lerp towards goal position (zooming and translating)
            Camera.main.orthographicSize = Mathf.Lerp(Camera.main.orthographicSize, zoomedOrthographicSize, zoomInterpFactor);

            // Lerp increase vignette intensity
            float oldVignetteIntensity = vignette.intensity.value;
            vignette.intensity.value = Mathf.Lerp(oldVignetteIntensity, newVignetteIntensity, zoomInterpFactor);

            // Calculate next position the camera should be in
            Vector3 curr = transform.position;
            Vector3 goal = lastTargetPos + offset + Vector3.down; // look a bit down
            Vector3 nextPos = new Vector3(
                Mathf.Lerp(curr.x, goal.x, zoomInterpFactor),
                Mathf.Lerp(curr.y, goal.y, zoomInterpFactor),
                Mathf.Lerp(curr.z, goal.z, zoomInterpFactor));

            // When to end the loop ()
            float diffMove = (curr - goal).magnitude;
            float diffZoom = Mathf.Abs(Camera.main.orthographicSize - zoomedOrthographicSize);
            float diffVignette = Mathf.Abs(vignette.intensity.value - newVignetteIntensity);
            if (diffZoom <= 0.1f && diffMove <= 0.1f && diffVignette <= 0.1f)
                condition = false;

            // Actualy move the camera
            transform.position = nextPos;

            // Wait for next frame
            yield return new WaitForEndOfFrame();
        }
    }
}
