using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogController : MonoBehaviour
{
	// Parameters
	public Vector3 direction;
	public float speed;
	public float end;

	public void SetValues(float _speed, Vector3 _direction, float _end)
	{
		speed = _speed;
		direction = _direction;
		end = _end;
		transform.localScale = new Vector3(3, 0.15f, 0.8f);
	}

	private void Update()
	{

		// Move
		transform.position += direction * Time.deltaTime * speed;

		// Handle going off the screen
		if (direction == Vector3.left && transform.position.x < -end)
		{
			Vector3 newPos = transform.position;
			newPos.x = end;
			transform.position = newPos;
		}
		else if (direction == Vector3.right && transform.position.x > end)
		{
			Vector3 newPos = transform.position;
			newPos.x = -end;
			transform.position = newPos;
		}

	}
}
