using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Game manager things:
 *   - creates procedural level
 *     - First sets up a random seed, then one lane at a time generates the level
 *     - Generates X amount of lanes in front of the player and deletes X amount behind
 *     - Handles player going backwards (to start)
 *     - realised in lanes (safe, road, water)
 *     - initializes obstacles (vehicles and water)
 *     - initializes collectibles
 *   - initializes player
 *   - has getters for UI to use
 *   - saves highscore to local storage
 *   
 *   
 *   TODO: when placing coins first check if empty (no trees)
 */


public class GameManager : MonoBehaviour
{
	// Singleton pattern
	private static GameManager _instance;
	public static GameManager Instance { get { return _instance; } }

	// Parameters
	[Header("Temporary")]
	public GameObject audioManagerPrefab;
	[Header("Player")]
	public GameObject playerPrefab;
	[Header("Level parameters")]
	public bool useSpecifiedSeed = false;
	public int seed = 0;
	public float obstacleFrequency = 0.2f;
	public float vehicleFrequency = 0.2f;
	public float logFrequency = 0.2f;
	public float collectibleFrequency = 0.1f;
	public int laneWidth = 10;
	public int lanesInFrontOfPlayer = 20;		// How many lanes to prepare in front and behind player
	public int lanesBehindOfPlayer = 10;
	public float minSpeed = 0.7f;
	public float maxSpeed = 2f;
	public int playerBounds = 5;				// how many tiles can player move left/right not to go off the screen

	// Variables
	public Prefab[] prefabs;											// Prefabs to spawn for tiles, cars, trees...
	private GameObject environment;										// Put everything in one gameobject
	private Dictionary<int, Lane> lanes = new Dictionary<int, Lane>();  // Keeping track of lanes (Lane class)
	public Dictionary<int, Lane> Lanes { get { return lanes; } }
	private Player player;
	private Vector3 prevPlayerPos;

	// Start is called before the first frame update
	void Awake()
	{
		// Singleton pattern
		if (_instance != null && _instance != this)
			Destroy(this.gameObject);
		else
			_instance = this;

		// Create the environment gameObject
		environment = new GameObject("Environment");

		// Setup random seed
		if (useSpecifiedSeed)
			Random.InitState(seed);
		else
			Random.InitState((int)System.DateTime.Now.Ticks);

		// Generate first lanes
		for (int i = -lanesBehindOfPlayer + 1; i < lanesInFrontOfPlayer; i++) 
		{
			GenerateLane(i);
		}

		Instantiate(playerPrefab, new Vector3(0, 0.1f, 0), Quaternion.identity);
	}

	private void Start()
	{
		// Start playing car sounds
		if (AudioManager.Instance == null)
			Instantiate(audioManagerPrefab, Vector3.zero, Quaternion.identity);
		AudioManager.Instance.Play("Car Noise");

		// Local reference to player
		player = Player.Instance;
		prevPlayerPos = player.transform.position;
	}

	private void Update()
	{
		if (player == null)
			return;

		int newZ = (int)player.transform.position.z;
		int oldZ = (int)prevPlayerPos.z;

		// If the player changed lanes, create new ones and deactivate old ones not needed
		if (newZ > oldZ)    // Player moved forwards
		{
			// deactivate backmost lane
			Lane backLane = lanes[newZ - lanesBehindOfPlayer];
			if (backLane != null)
				backLane.parent.gameObject.SetActive(false);
			// Generate/activate new lane
			GenerateLane(newZ + lanesInFrontOfPlayer - 1);
		}
		else if (newZ < oldZ) // Player moved backwards
		{
			// deactivate frontmost lane
			Lane frontLane = lanes[newZ + lanesInFrontOfPlayer];
			if (frontLane != null)
				frontLane.parent.gameObject.SetActive(false);
			// Generate/activate new lane
			GenerateLane(newZ - lanesBehindOfPlayer + 1);
		}

		if (newZ != oldZ)
			prevPlayerPos = player.transform.position;
	}

	private void GenerateLane(int lane)
	{
		// If this lane was already defined just set it to active
		if (lanes.ContainsKey(lane))
		{
			lanes[lane].parent.gameObject.SetActive(true);
			return;
		}

		// TODO: add biases for different lanes
		// If the lane is not yet defined, set it to random type
		if (lane == 0) // If it is the first lane, it should always be safe
			lanes.Add(lane, new Lane());
		else
			lanes.Add(lane, new Lane((LaneType)Random.Range(0, 3)));

		// Set lane speed and direction
		lanes[lane].direction = Random.Range(0, 100) > 50 ? -1 : 1;
		lanes[lane].speed = Random.Range(minSpeed, maxSpeed);

		// Get the type of current lane
		LaneType laneType = lanes[lane].type;

		// Create an empty game object to hold the lane pieces and set it to be child of environment
		GameObject laneGameObject = new GameObject("Lane " + lane);
		laneGameObject.transform.parent = environment.transform;
		lanes[lane].parent = laneGameObject.transform;

		// Was there a vehicle spawned on prev X coord in iteration
		// So cars wont be so close to each other
		bool vehiclePrevious = false;
		int logPrevious = 0;
		// Go trough the lane and place tiles
		for (int i = -laneWidth; i < laneWidth; i++)
		{
			// What tile to place
			GameObject tile = null;

			switch (laneType)
			{
				case LaneType.safe:
					tile = prefabs[0].gameObject;
					break;
				case LaneType.road:
					tile = prefabs[1].gameObject;
					break;
				case LaneType.water:
					tile = prefabs[3].gameObject;
					break;
				default:
					break;
			}

			// If we successfuly selected a prefab, then spawn it
			if (tile != null)
			{
				Vector3 pos = new Vector3(i, 0, lane);
				lanes[lane].tiles.Add(Instantiate(tile, pos, Quaternion.identity, laneGameObject.transform));
			}

			// On safe tiles, randomly place trees (but not on tile 0,0 - start tile)
			if (laneType == LaneType.safe && Random.Range(0, 100) < 100 * obstacleFrequency && lane != 0 && i != 0)
			{
				GameObject prefab = prefabs[Random.Range(4, 8)].gameObject;
				Vector3 pos = new Vector3(i, 0, lane);
				lanes[lane].obstacles.Add(Instantiate(prefab, pos, Quaternion.identity, laneGameObject.transform));
			}
			// If we are on a safe lane inside player move zone and havent placed an obstacle, then place coins randomly
			else if (lanes[lane].type == LaneType.safe && i > -playerBounds && i < playerBounds && lane > 0 && Random.Range(0.0f, 1f) < collectibleFrequency)
			{
				GenerateCoin(new Vector3(i, 0.5f, lane), laneGameObject.transform);
			}

			// On roads put cars at given frequency
			if (laneType == LaneType.road && Random.Range(0, 100) < 100 * vehicleFrequency && !vehiclePrevious)
			{
				// Mark for next iteration not to have car (too close)
				vehiclePrevious = true;
				// Get the prefab and position
				GameObject prefab = prefabs[Random.Range(9, 15)].gameObject; // 8 is delivery which i dont want to use
				Vector3 pos = new Vector3(i, 0.1f, lane);
				GameObject vehicle;

				// Spawn based on direction and speed
				if (lanes[lane].direction < 0)
				{
					vehicle = Instantiate(prefab, pos, Quaternion.Euler(0, -90, 0), laneGameObject.transform);
					vehicle.AddComponent<Vehicle>().SetValues(lanes[lane].speed, Vector3.left, laneWidth);
				}
				else
				{
					vehicle = Instantiate(prefab, pos, Quaternion.Euler(0, 90, 0), laneGameObject.transform);
					vehicle.AddComponent<Vehicle>().SetValues(lanes[lane].speed, Vector3.right, laneWidth);
				}
				lanes[lane].vehicles.Add(vehicle);
			}
			else if (vehiclePrevious) { vehiclePrevious = false; }

			// On water put logs at given frequency
			if (laneType == LaneType.water && Random.Range(0.0f, 1.0f) < logFrequency && logPrevious <= 0)
			{
				int size = Random.Range(2, 4);
				logPrevious = (int)size + 10;
				GenerateLog(new Vector3(i, 0, lane), size, lanes[lane].direction, laneGameObject.transform);
			}
			else if (logPrevious > 0) { logPrevious--; }

		}

		// If the lane is water and no logs were added, add one here
		if (lanes[lane].type == LaneType.water && lanes[lane].vehicles.Count == 0)
			GenerateLog(new Vector3(0, 0, lane), Random.Range(2, 5), lanes[lane].direction, laneGameObject.transform);
	} // End of GenerateLane(int lane)

	public void GameOver()
	{
		AudioManager.Instance.Stop("Music");
		AudioManager.Instance.Stop("Car Noise");
	}

	#region Helper functions
	private void GenerateLog(Vector3 pos, float size, int dir, Transform parent)
	{
		// Get the prefab and position
		GameObject logPrefab = prefabs[15].gameObject;
		GameObject log = Instantiate(logPrefab, pos, Quaternion.identity, parent);
		Vector3 direction;
		// Spawn based on direction and speed
		direction = dir < 0 ? Vector3.left : Vector3.right;
		log.AddComponent<LogController>().SetValues(lanes[(int)pos.z].speed, direction, laneWidth, size);
		lanes[(int)pos.z].vehicles.Add(log);
	}

	private void GenerateCoin(Vector3 pos, Transform parent)
	{
		// Get the prefab and instantiate it
		GameObject prefab = prefabs[16].gameObject;
		GameObject coin = Instantiate(prefab, pos, Quaternion.identity, parent);
		// Add coin animation script to it and add to lane collectibles list
		lanes[(int)pos.z].collectibles.Add(coin);
	}

	#endregion
}
