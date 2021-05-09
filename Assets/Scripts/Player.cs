using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO: write player docs
/// </summary>
public class Player : MonoBehaviour
{
    // Singleton pattern
    private static Player _instance;
    public static Player Instance { get { return _instance; } }

    private int _coins;
    private int _steps;
    private int playerBounds;

    // Water log stuff
    private bool logBound;      // If player is standing on a log
    private Transform log;      // Position of the log
    private float logOffset;  // Offset of current position on the log

    [Header("Particles")]
    public GameObject deathParticles;
    public ParticleSystem moveParticles;

    // Getter for coins
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

	// Start is called before the first frame update
	void Start()
    {
        playerBounds = GameManager.Instance.playerBounds;
        _coins = PlayerPrefs.GetInt("Coins", 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (UIController.Instance.gamePaused)
            return;

        // If we are on a log move along with it
        if (logBound)
		{
            Vector3 pos = transform.position;
            transform.position = new Vector3(log.position.x + logOffset, pos.y, pos.z);
            if (log.position.x + logOffset < -playerBounds || log.position.x + logOffset > playerBounds)
                Die();
		}

        // Handle moving the player
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            Move(transform.position + Vector3.forward, 0);
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            Move(transform.position + Vector3.left, -90);
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            Move(transform.position + Vector3.right, 90);
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            Move(transform.position + Vector3.back, 180);
    }

	private void Move(Vector3 pos, float rot)
    {
        // Dont go left/right out of bounds
        if (pos.x <= -playerBounds || pos.x >= playerBounds)
            return;

        // Dont go back behind start line
        if (pos.z < 0)
            return;

        // Check if there is an obstacle here
        foreach(GameObject o in GameManager.Instance.Lanes[(int)pos.z].obstacles)
            if(o.transform.position.x == Mathf.Round(pos.x)) 
                return;

        // Check if player is moving on the water
        if(GameManager.Instance.Lanes[(int)pos.z].type == LaneType.water)
		{
            // If the position is on the log then ride the log
            GameObject goalLog = PositionIsOnLog(pos, (int)pos.z);
            if (goalLog != null)
            {
                // Find tile to stand on
                Vector3 tile = FindClosestTileOnLog(goalLog, pos);
                transform.position = tile;
                logBound = true;
                log = goalLog.transform;
                logOffset = goalLog.transform.position.x - tile.x;
            }
            else // Else drown
            {
                Die();
            }
            // TODO: handle going left/right on a log/going off the log into the water
            /*int dir = Mathf.RoundToInt(pos.x) - Mathf.RoundToInt(transform.position.x);
            if (logBound && dir != 0)
            {
                GameObject goalLog = PositionIsOnLog(transform.position, (int)pos.z);
                if (goalLog != null)
                {
                    // Find tile to stand on
                    Vector3 tile = FindClosestTileOnLog(goalLog, pos);
                    transform.position = tile;
                    logBound = true;
                    log = goalLog.transform;
                    logOffset = goalLog.transform.position.x - tile.x;
                }

                else // Else drown
                {
                    Die();
                }
            }*/
            // Only if going left/right
        }

        // If we want to get off the log
        if (GameManager.Instance.Lanes[(int)pos.z].type != LaneType.water && logBound)
		{
            // Find closest tile and if it is not an obstacle go on it
            pos = new Vector3(Mathf.Round(pos.x), pos.y, pos.z);
            if(GameManager.Instance.Lanes[(int)pos.z].type == LaneType.safe)
                foreach (GameObject o in GameManager.Instance.Lanes[(int)pos.z].obstacles)
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

	private void OnTriggerEnter(Collider other)
	{
        if(other.tag == "Collectible")
		{
            _coins++;
            PlayerPrefs.SetInt("Coins", _coins);
            other.GetComponent<Coin>().PickUp();
		}
        else if(other.tag == "Vehicle")
        {
            Die();
        }
    }

    private GameObject PositionIsOnLog(Vector3 pos, int lane)
    {
        // Go trough all logs
        foreach (GameObject logIt in GameManager.Instance.Lanes[lane].vehicles)
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

	private void Die()
	{
        //Destroy(gameObject);
        AudioManager.Instance.Play("Player Death");
        GameManager.Instance.GameOver();
        Instantiate(deathParticles, transform.position, Quaternion.identity, GameManager.Instance.transform);
	}
}
