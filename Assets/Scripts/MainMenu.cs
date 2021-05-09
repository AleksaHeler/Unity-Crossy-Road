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
	public Transform deletePopup;

	private void Start()
	{
		originalPos = titleTransform.position;
		deletePopup.gameObject.SetActive(false);
	}

	private void Update() {
		// Move title up and down
		if(titleTransform != null){
			titleTransform.position = originalPos + 5 * amplitude * Vector3.up * Mathf.Sin(3 * Time.time * frequency);
		}

		if (Input.GetKeyDown(KeyCode.Space))
			Play();
		else if (Input.GetKeyDown(KeyCode.Escape))
			QuitGame();
	}

	public void Play()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	public void DeletePopup(bool enable)
	{
		deletePopup.gameObject.SetActive(enable);
	}

	public void DeleteSavedGame()
	{
		PlayerPrefs.DeleteAll();
	}
}
