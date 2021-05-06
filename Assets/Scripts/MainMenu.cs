using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
	[Range(0, 5)]
	public float amplitude = 1;
	[Range(0, 5)]
	public float frequency = 1;
	public Transform titleTransform;
	private Vector3 originalPos;

	private void Start()
	{
		originalPos = titleTransform.position;
	}

	private void Update() {
		// Move title up and down
		if(titleTransform != null){
			titleTransform.position = originalPos + 5 * amplitude * Vector3.up * Mathf.Sin(3 * Time.time * frequency);
		}
	}

	public void Play()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void QuitGame()
	{
		Debug.Log("Closing application!");
		Application.Quit();
	}
}
