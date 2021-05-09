using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used for animating vehicles (cars) and logs on water
/// </summary>
public class Vehicle : MonoBehaviour
{
	/// Left or right Vector3
	public Vector3 direction;
	/// Speed of the lane this vehicle is in
	public float speed;
	/// End of the lane (+/- in x coordinates) where the vehicle will be moved back to its starting point
	public float end;

	/// <summary>
	/// Used in place of constructor. Just sets passed values
	/// </summary>
	/// <param name="_speed">Speed of the lane in which this vehicle is in</param>
	/// <param name="_direction">Direction of the lane (Vector3.left or Vector3.right)</param>
	/// <param name="_end">End of the lane (+/- in x coordinates) where the vehicle will be moved back to its starting point</param>
	public void SetValues(float _speed, Vector3 _direction, float _end)
	{
		speed = _speed;
		direction = _direction;
		end = _end;
	}

	private void Update()
	{
		// Move with constant speed (not FPS bound)
		transform.position += direction * Time.deltaTime * speed;

		// Handle going off the screen
		// New position is same, just the x coordinate is set to begining of the lane
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
