using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// Class used to controll the game main menu. It is located on base canvas
/// </summary>
public class MainMenu : MonoBehaviour
{
	[Header("Title animation")]
	[Range(0, 5)]
	public float amplitude = 1;
	[Range(0, 5)]
	public float frequency = 1;

	[Header("Transform references")]
	public Transform titleTransform;
	public Transform deletePopup;

	// Keeping track of starting position of the title
	private Vector3 titleOriginalPos;

	private void Start()
	{
		// Get original position of title and dont display delete popup screen
		titleOriginalPos = titleTransform.position;
		deletePopup.gameObject.SetActive(false);
	}

	private void Update() {
		// Move title up and down
		if(titleTransform != null){
			titleTransform.position = titleOriginalPos + 5 * amplitude * Vector3.up * Mathf.Sin(3 * Time.time * frequency);
		}

		// Play and quit on keys, not just buttons on the screen
		if (Input.GetKeyDown(KeyCode.Space))
			Play();
		else if (Input.GetKeyDown(KeyCode.Escape))
			QuitGame();
	}

	/// <summary>
	/// Play the game. Transitions to next scene: "Game". Objects marked dont destroy on load (AudioManger) dont get destroyed
	/// </summary>
	public void Play()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
	}

	/// <summary>
	/// Closes the applicaion
	/// </summary>
	public void QuitGame()
	{
		Application.Quit();
	}

	/// <summary>
	/// Shows the popup to confirm deletion of saved game (highscores and coins)
	/// </summary>
	public void DeletePopup(bool enable)
	{
		deletePopup.gameObject.SetActive(enable);
	}

	/// <summary>
	/// Actually delete saved data
	/// </summary>
	public void DeleteSavedGame()
	{
		PlayerPrefs.DeleteAll();
	}
}
