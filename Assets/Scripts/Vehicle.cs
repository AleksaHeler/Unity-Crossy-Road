using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
	// Parameters
	public Vector3 direction;
	public float speed;
	public float end;

	// Instead of constructor
	public void SetValues(float _speed, Vector3 _direction, float _end)
	{
		speed = _speed;
		direction = _direction;
		end = _end;
	}

	private void Update()
	{

		// Move
		transform.position += direction * Time.deltaTime * speed;

		// Handle going off the screen
		if(direction == Vector3.left && transform.position.x < -end)
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
