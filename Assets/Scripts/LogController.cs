using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Log extends Vehicle, only difference is added size variable
// So it initiates with given size and inherits update function from Vehicle
public class LogController : Vehicle
{
	public float size;

	public void SetValues(float _speed, Vector3 _direction, float _end, float _size)
	{
		speed = _speed;
		direction = _direction;
		end = _end;
		size = _size;
		transform.localScale = new Vector3(size, 0.2f, 0.8f);
	}
}
