using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class is animating coin by moving it up and down with 
/// sine wave and rotating it around with constant speed
/// </summary>
public class Coin : MonoBehaviour
{
    // Parameters
    private float amplitude = 0.2f;
    private float frequency = 1.5f;
    private float rotSpeed = 90f;

    // Keep track of original position
    private Vector3 originalPos;

	private void Start()
	{
        originalPos = transform.position;
    }


    private void Update()
    {
        // Move up and down and rotate
        transform.position = originalPos + Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;
        transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
    }

    /// <summary>
    /// Get picked up by player: plays audio and destroys itself
    /// </summary>
    public void PickUp()
    {
        AudioManager.Instance.Play("Click");
        Destroy(gameObject);
	}
}
