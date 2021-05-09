using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private float amplitude = 0.2f;
    private float frequency = 1.5f;
    private float rotSpeed = 90f;

    private Vector3 originalPos;

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

    public void PickUp()
    {
        AudioManager.Instance.Play("Click");
        Destroy(gameObject);
	}
}
