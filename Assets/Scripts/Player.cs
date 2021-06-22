using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class controlling player behaviour. Gets input data and moves, detects game over, collects coins
/// </summary>
public class Player : MonoBehaviour
{
    // Singleton pattern
    private static Player _instance;
    public static Player Instance { get { return _instance; } }

    // Internal values for stats
    private int _coins;
    private int _steps;
    private int playerBounds;

    // Stuff for 'riding' logs
    private bool logBound;              // If player is standing on a log
    private Transform mountedLog;       // Position of the log
    private float posOffsetOnLog;       // Offset of current position on the log

    [Header("Particles")]
    public GameObject deathParticles;
    public ParticleSystem moveParticles;

    // Exposing stats as read-only
    public int Coins { get { return _coins; } }
    public int Steps { get { return _steps; } }

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
        // Sets max player movement on X axis and gets saved coins from player prefs
        playerBounds = GameManager.Instance.playerBounds;
        _coins = PlayerPrefs.GetInt("Coins", 0);
    }

    void Update()
    {
        // If game is paused dont move
        if (UIController.Instance.gamePaused)
            return;

        // If we are on a log move along with it (only on X axis)
        if (logBound)
		{
            Vector3 pos = transform.position;
            transform.position = new Vector3(mountedLog.position.x + posOffsetOnLog, pos.y, pos.z);
            if (mountedLog.position.x + posOffsetOnLog < -playerBounds || mountedLog.position.x + posOffsetOnLog > playerBounds)
                Die();
		}

        // Handle moving the player
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            Move(transform.position + Vector3.forward, 0);  // Params: new pos, rotation
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            Move(transform.position + Vector3.left, -90);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            Move(transform.position + Vector3.right, 90);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            Move(transform.position + Vector3.back, 180);
    }

    /// <summary>
    /// Move the player to given position and face him in given rotation
    /// </summary>
    /// <param name="pos">Position in world space</param>
    /// <param name="rot">Rotation in global space (not relative)</param>
	private void Move(Vector3 pos, float rot)
    {
        // Dont go left/right out of bounds or back behind start line
        if (pos.x <= -playerBounds || pos.x >= playerBounds || pos.z < 0)
            return;

        // Check if there is an obstacle here
        foreach(GameObject o in GameManager.Instance.lanes[(int)pos.z].obstacles)
            if(o.transform.position.x == Mathf.Round(pos.x)) 
                return;

        // Check if player is moving on the water
        if(GameManager.Instance.lanes[(int)pos.z].type == LaneType.Water)
		{
            // If the position is on the log then ride the log (mount it)
            // This also handles moving on the log from one pos to another
            GameObject goalLog = PositionIsOnLog(pos, (int)pos.z);
            if (goalLog != null)
            {
                // Find tile to stand on
                Vector3 tile = FindClosestTileOnLog(goalLog, pos);
                transform.position = tile;
                logBound = true;
                mountedLog = goalLog.transform;
                posOffsetOnLog = goalLog.transform.position.x - tile.x;
            }
            else // Else drown
            {
                Die();
            }
        }

        // If we want to get off the log
        if (GameManager.Instance.lanes[(int)pos.z].type != LaneType.Water && logBound)
		{
            // Find closest tile and if it is not an obstacle go on it
            pos = new Vector3(Mathf.Round(pos.x), pos.y, pos.z);
            if(GameManager.Instance.lanes[(int)pos.z].type == LaneType.Grass)
                foreach (GameObject o in GameManager.Instance.lanes[(int)pos.z].obstacles)
                    if (o.transform.position.x == Mathf.Round(pos.x))
                        return;
            logBound = false;
		}

        // Actually move
        AudioManager.Instance.Play("Player Jump");
        moveParticles.Play();
        transform.position = pos;
        transform.rotation = Quaternion.Euler(0, rot, 0);

        // Check for increased steps
        if (pos.z > _steps)
            _steps = (int)pos.z;
    }

    // Handle trigger colliders (coins and vehicles)
	private void OnTriggerEnter(Collider other)
	{
        if(other.tag == "Collectible")
		{
            // Increase coins count and save it to player prefs
            _coins++;
            PlayerPrefs.SetInt("Coins", _coins);
            other.GetComponent<Coin>().PickUp();
		}
        else if(other.tag == "Vehicle")
        {
            Die();
        }
    }

    /// <summary>
    /// If the given position is on a log, it will return that log
    /// </summary>
    private GameObject PositionIsOnLog(Vector3 pos, int lane)
    {
        // Go trough all logs in the lane, and if the player pos and log pos line up, return it
        foreach (GameObject logIt in GameManager.Instance.lanes[lane].vehicles)
        {
            Vector3 logPos = logIt.transform.position;
            float logSize = 3.0f;

            float logDistance = Mathf.Abs(logPos.x - pos.x);

            if (logDistance <= logSize / 2.0f)
            {
                return logIt;
            }
        }
        return null;
    }

    /// <summary>
    /// For given log finds which of three tiles on it are closest to the given position
    /// </summary>
    private Vector3 FindClosestTileOnLog(GameObject _log, Vector3 pos)
	{
        //float pos = transform.position.x;
        float logPos = _log.transform.position.x;
        if(Mathf.Abs(logPos - pos.x) <= 0.5f)
            return _log.transform.position;
        else if(pos.x - logPos > 0)
            return _log.transform.position + Vector3.left;
		else
            return _log.transform.position + Vector3.right;
	}

    /// <summary>
    /// Play death sound, call GameManagers GameOver() and instantiate death particles
    /// </summary>
	private void Die()
	{
        //Destroy(gameObject);
        AudioManager.Instance.Play("Player Death");
        GameManager.Instance.GameOver();
        Instantiate(deathParticles, transform.position, Quaternion.identity, GameManager.Instance.transform);
	}
}
