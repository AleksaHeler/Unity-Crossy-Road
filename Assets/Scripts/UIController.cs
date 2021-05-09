using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
	// Singleton pattern
	private static UIController _instance;
	public static UIController Instance { get { return _instance; } }

	public TextMeshProUGUI coinsText;
	public TextMeshProUGUI stepsText;
	public TextMeshProUGUI statsText;
	public TextMeshProUGUI highscoreText;
	public TextMeshProUGUI pausedTitleText;
	public TextMeshProUGUI restartText;

	public Transform gameStatsScreen;
	public Transform gameOverScreen;
	public Transform pauseScreen;

	public bool isHighscore = false;
	public bool gamePaused = false;
	public bool gameOver = false;

	private float gameOverWait = 0f;
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
		coinsText.text = "0";
		stepsText.text = "0";

		player = Player.Instance;

		gameOverScreen.gameObject.SetActive(false);
		gameStatsScreen.gameObject.SetActive(true);
		pauseScreen.gameObject.SetActive(false);

		isHighscore = false;
	}

	void Update()
	{
		// Pause and resume
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

		if (gameOver)
		{
			if (gameOverWait < 3f)
			{
				gameOverWait += Time.unscaledDeltaTime;
				int val = (3 - Mathf.FloorToInt(gameOverWait));
				restartText.text = val.ToString();
			}
			else if (Input.anyKeyDown)
			{
				RestartGame();
			}
			else
			{
				restartText.text = "Press any key to restart";
			}
		}
	}

	public void GameOver()
	{
		gameOver = true;
		gameStatsScreen.gameObject.SetActive(false);
		gameOverScreen.gameObject.SetActive(true);
		int steps = GameManager.Instance.steps;
		int highscore = GameManager.Instance.highscore;
		int coins = GameManager.Instance.coins;
		statsText.text = "Steps: " + steps + "\nHighscore: " + highscore + "\nCoins: " + coins;
		if (isHighscore)
		{
			highscoreText.gameObject.SetActive(true);
			StartCoroutine(AnimateHighscoreText());
		}
	}

	public void PauseGame()
	{
		if (gameOver)
			return;
		pauseScreen.gameObject.SetActive(true);
		gameStatsScreen.gameObject.SetActive(false);
		Time.timeScale = 0f;
		gamePaused = true;
		AudioManager.Instance.SetVolume("Music", 0.1f);
		AudioManager.Instance.Pause("Car Noise");
		StartCoroutine(AnimatePauseTitleText());
	}


	public void ResumeGame()
	{
		pauseScreen.gameObject.SetActive(false);
		gameStatsScreen.gameObject.SetActive(true);
		Time.timeScale = 1f;
		gamePaused = false;
		AudioManager.Instance.SetVolume("Music", 0.6f);
		AudioManager.Instance.Play("Car Noise");
		StopCoroutine(AnimatePauseTitleText());
	}

	public void RestartGame()
	{
		// Destroy audiomanager (it persists trough scenes but is also in mainmenu scene)
		Destroy(AudioManager.Instance.gameObject);
		// Load main menu scene
		SceneManager.LoadScene(0);
	}

	public void QuitGame()
	{
		Application.Quit();
	}

	IEnumerator AnimateHighscoreText()
	{
		while (true)
		{
			highscoreText.fontSize = 82 + 12 * Mathf.Sin(Time.time * 10f);
			yield return new WaitForEndOfFrame();
		}
	}
	IEnumerator AnimatePauseTitleText()
	{
		float t = 0;
		while (true)
		{
			pausedTitleText.fontSize = 150 + 10 * Mathf.Sin(t * 5f);
			t += 1f / 60f;
			yield return new WaitForSecondsRealtime(1f / 60f);
		}
	}
}
