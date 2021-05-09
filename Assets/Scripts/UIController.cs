using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

/// <summary>
/// Class used to animate and controll user interface during the game, pause menu and game over screen
/// </summary>
public class UIController : MonoBehaviour
{
	// Singleton pattern
	private static UIController _instance;
	public static UIController Instance { get { return _instance; } }

	[Header("Text references")]
	public TextMeshProUGUI coinsText;
	public TextMeshProUGUI stepsText;
	public TextMeshProUGUI statsText;
	public TextMeshProUGUI highscoreText;
	public TextMeshProUGUI pausedTitleText;
	public TextMeshProUGUI restartText;

	[Header("Transform references")]
	public Transform gameStatsScreen;
	public Transform gameOverScreen;
	public Transform pauseScreen;

	// Variables used to keep track of some in game informations
	[HideInInspector]
	public bool isHighscore = false;
	[HideInInspector]
	public bool gamePaused = false;
	[HideInInspector]
	public bool gameOver = false;

	// Timer for waiting three seconds before player can continue once he died
	private float gameOverWait = 0f;
	// Local reference to player script for data
	Player player;

	private void Awake()
	{
		// Singleton pattern
		if (_instance != null && _instance != this)
			Destroy(this.gameObject);
		else
			_instance = this;
	}

	void Start()
	{
		// Reset texts
		coinsText.text = "0";
		stepsText.text = "0";

		// Get player reference
		player = Player.Instance;

		// Enable only the stats screen for now
		gameOverScreen.gameObject.SetActive(false);
		gameStatsScreen.gameObject.SetActive(true);
		pauseScreen.gameObject.SetActive(false);

		isHighscore = false;
	}

	void Update()
	{
		// Pause and resume on key press
		if (Input.GetKeyDown(KeyCode.Escape) && gamePaused)
			ResumeGame();
		else if (Input.GetKeyDown(KeyCode.Escape) && !gamePaused)
			PauseGame();

		// Display stats
		if (player != null)
		{
			coinsText.text = player.Coins.ToString();
			stepsText.text = player.Steps.ToString();
		}

		// If game is over wait three seconds before enableing continue
		if (gameOver)
		{
			if (gameOverWait < 3f)
			{
				// Wait for three seconds and display remaining time on screen
				gameOverWait += Time.unscaledDeltaTime;
				int val = 3 - Mathf.FloorToInt(gameOverWait);
				restartText.text = val.ToString();
			}
			else if (Input.anyKeyDown)
			{
				RestartGame();
			}
			else
			{
				// Once the timer is up, set text to:
				restartText.text = "Press any key to restart";
			}
		}
	}

	/// <summary>
	/// Called when player dies. Shows game over screen with stats
	/// </summary>
	public void GameOver()
	{
		gameOver = true;
		gameStatsScreen.gameObject.SetActive(false);
		gameOverScreen.gameObject.SetActive(true);
		// Get game stats from GameManager (player is dead)
		int steps = GameManager.Instance.steps;
		int highscore = GameManager.Instance.highscore;
		int coins = GameManager.Instance.coins;
		// Show the stats on screen
		statsText.text = "Steps: " + steps + "\nHighscore: " + highscore + "\nCoins: " + coins;
		// If its highscore, show 'highscore' tag and animate it
		if (isHighscore)
		{
			highscoreText.gameObject.SetActive(true);
			StartCoroutine(AnimateHighscoreText());
		}
	}

	/// <summary>
	/// Pauses time and shows pause screen
	/// </summary>
	public void PauseGame()
	{
		// If its game over, we dont have this functionality
		if (gameOver)
			return;
		// Enable right game objects
		pauseScreen.gameObject.SetActive(true);
		gameStatsScreen.gameObject.SetActive(false);
		// Stop time and set sounds accordingly
		Time.timeScale = 0f;
		gamePaused = true;
		AudioManager.Instance.SetVolume("Music", 0.1f);
		AudioManager.Instance.Pause("Car Noise");
		// Animate pause title
		StartCoroutine(AnimatePauseTitleText());
	}

	/// <summary>
	/// Resumes the game and removes pause screen
	/// </summary>
	public void ResumeGame()
	{
		// Set right objects active
		pauseScreen.gameObject.SetActive(false);
		gameStatsScreen.gameObject.SetActive(true);
		// Resume time and sounds
		Time.timeScale = 1f;
		gamePaused = false;
		AudioManager.Instance.SetVolume("Music", 0.6f);
		AudioManager.Instance.Play("Car Noise");
		// Stop animating the pause text
		StopCoroutine(AnimatePauseTitleText());
	}

	/// <summary>
	/// Loads main menu scene to restart the game
	/// </summary>
	public void RestartGame()
	{
		// Destroy audiomanager (it persists trough scenes but is also in main menu scene)
		Destroy(AudioManager.Instance.gameObject);
		// Load main menu scene
		SceneManager.LoadScene(0);
	}

	/// <summary>
	/// Closes the application
	/// </summary>
	public void QuitGame()
	{
		Application.Quit();
	}

	/// <summary>
	/// Animates highscore text font size by sin() value
	/// </summary>
	IEnumerator AnimateHighscoreText()
	{
		while (true)
		{
			highscoreText.fontSize = 82 + 12 * Mathf.Sin(Time.time * 10f);
			yield return new WaitForEndOfFrame();
		}
	}

	/// <summary>
	/// Animates pause title text font size by sin() value
	/// </summary>
	IEnumerator AnimatePauseTitleText()
	{
		while (true)
		{
			pausedTitleText.fontSize = 150 + 10 * Mathf.Sin(Time.unscaledTime * 5f);
			yield return new WaitForSecondsRealtime(1f / 60f);
		}
	}
}
