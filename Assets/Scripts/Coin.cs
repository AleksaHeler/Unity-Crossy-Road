using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private float amplitude;
    private float frequency;
    private float rotSpeed;

    private Vector3 originalPos;

    public void SetValues(float _amplitude, float _frequency, float _rotSpeed)
	{
        amplitude = _amplitude;
        frequency = _frequency;
        rotSpeed = _rotSpeed;
    }

	private void Start()
	{
        originalPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Move up and down and rotate
        transform.position = originalPos + Vector3.up * Mathf.Sin(Time.time * frequency) * amplitude;
        transform.Rotate(Vector3.up, rotSpeed * Time.deltaTime);
    }
}
