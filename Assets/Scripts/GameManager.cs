using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is responsible for level generation and spawning player inside the game. 
/// It also handles game over after player calls function GameManager.Instance.GameOver()
/// </summary>
public class GameManager : MonoBehaviour
{
	// Singleton pattern
	private static GameManager _instance;
	public static GameManager Instance { get { return _instance; } }

	#region Parameters
	[Header("Player")]
	public GameObject playerPrefab;
	[Space]
	[Header("Level seed")]
	public bool useSpecifiedSeed = false;
	public int seed = 42;
	[Header("Entity frequencies")]
	public float obstacleFrequency = 0.15f;
	public float vehicleFrequency = 0.3f;
	public float logFrequency = 0.15f;
	public float collectibleFrequency = 0.08f;
	[Header("Lane settings")]
	public int laneWidth = 30;
	public int vehicleEnd = 22;
	public int lanesInFrontOfPlayer = 20;       // How many lanes to prepare in front and behind player
	public int lanesBehindOfPlayer = 10;
	[Header("Speed settings")]
	public float minSpeed = 1f;
	public float maxSpeed = 3f;
	public float speedIncreaseByLane = 0.05f;   // Further the player goes faster the traffic will be
	[Header("Player settings")]
	public int playerBounds = 6;                // how many tiles can player move left/right not to go off the screen
	[Header("Sound settings")]
	public GameObject audioManagerPrefab;       // Used only for debugging so we can start from the game scene

	// Variables
	[Header("Game objects")]
	[Tooltip("Prefabs to spawn for tiles, cars, trees...")]
	public List<Prefab> prefabs;                                        // Prefabs to spawn for tiles, cars, trees...
	private GameObject environment;                                     // Put everything in one gameobject
	public Dictionary<int, Lane> lanes = new Dictionary<int, Lane>();   // Keeping track of lanes (Lane class)
	private Player player;
	private Vector3 prevPlayerPos;

	[HideInInspector]
	public int steps;													// Keep track of player stats (game over screen)
	[HideInInspector]
	public int highscore;
	[HideInInspector]
	public int coins;
	#endregion

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
		{
			int _seed = (int)System.DateTime.Now.Ticks;
			Debug.Log("Level seed: " + _seed);
			Random.InitState(_seed);
		}

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

		// If the lane is not yet defined, set it to random type
		if (lane == 0) // If it is the first lane, it should always be safe
			lanes.Add(lane, new Lane());
		else
			lanes.Add(lane, new Lane((LaneType)Random.Range(0, 3)));

		// Set lane speed and direction
		lanes[lane].direction = Random.Range(0f, 1f) > .5f ? -1 : 1;
		lanes[lane].speed = Random.Range(minSpeed, maxSpeed) + speedIncreaseByLane * lane;

		// Get the type of current lane
		LaneType laneType = lanes[lane].type;

		// Create an empty game object to hold the lane pieces and set it to be child of environment
		GameObject laneGameObject = new GameObject("Lane " + lane);
		laneGameObject.transform.parent = environment.transform;
		lanes[lane].parent = laneGameObject.transform;

		// Find the gameobject of the right tile and place it
		foreach (Prefab p in prefabs)
		{
			if (p.name.Equals(laneType.ToString()))
			{
				lanes[lane].ground = Instantiate(p.gameObject, new Vector3(0, 0, lane), Quaternion.identity, laneGameObject.transform);
				break;
			}
		}

		// Go trough the lane and generate items/vehicles
		for (int i = -vehicleEnd / 2 + 1; i < vehicleEnd / 2 ; i++)
		{
			// On grass put trees and coins at given frequency
			if (laneType == LaneType.Grass)
				// On random chance (and if coordinates arent 0,0 - start coordinates)
				if (Random.Range(0f, 1f) < obstacleFrequency && lane != 0)
					GenerateTree(new Vector3(i, 0, lane), laneGameObject.transform);
				// If we are on a safe lane inside player move zone and havent placed an obstacle, then place coins randomly
				else if (i > -playerBounds && i < playerBounds && lane > 0 && Random.Range(0f, 1f) < collectibleFrequency)
					GenerateCoin(new Vector3(i, 0.5f, lane), laneGameObject.transform);

			// On roads put cars at given frequency
			if (laneType == LaneType.Road && Random.Range(0f, 1f) < vehicleFrequency)
				GenerateVehicle(new Vector3(i, 0.1f, lane), laneGameObject.transform);

			// On water put logs at given frequency
			if (laneType == LaneType.Water && Random.Range(0.0f, 1.0f) < logFrequency)
				GenerateLog(new Vector3(i, 0, lane), laneGameObject.transform);
		}

		// Remove cars/logs that are too close to each other (inefficient but not many items and not every frame)
		if (lanes[lane].type == LaneType.Road || lanes[lane].type == LaneType.Water)
		{
			// Find all objects that need to be removed and remove them
			List<GameObject> objToRemove = new List<GameObject>();
			for (int i = 0; i < lanes[lane].vehicles.Count; i++)
			{
				for (int j = i + 1; j < lanes[lane].vehicles.Count; j++)
				{
					GameObject o1 = lanes[lane].vehicles[i];
					GameObject o2 = lanes[lane].vehicles[j];
					float dist = Vector3.Distance(o1.transform.position, o2.transform.position);
					if (lanes[lane].type == LaneType.Water && dist <= 3.5f)
					{
						objToRemove.Add(o2);
						Destroy(o2);
					}
					else if (lanes[lane].type == LaneType.Road && dist <= 1.5f)
					{
						objToRemove.Add(o2);
						Destroy(o2);
					}
				}
			}
			foreach(GameObject o in objToRemove)
				lanes[lane].vehicles.Remove(o);
		}

		// If the lane is road/water and no vehicles/logs were added, add one here
		if (lanes[lane].type == LaneType.Road && lanes[lane].vehicles.Count == 0)
			GenerateVehicle(new Vector3(0, 0, lane), laneGameObject.transform);
		if (lanes[lane].type == LaneType.Water && lanes[lane].vehicles.Count == 0)
			GenerateLog(new Vector3(0, 0, lane), laneGameObject.transform);
	} // End of GenerateLane(int lane)

	public void GameOver()
	{
		// Mark final scores
		steps = player.Steps;
		highscore = PlayerPrefs.GetInt("Highscore", 0);
		coins = player.Coins;
		if (steps > highscore)
		{
			PlayerPrefs.SetInt("Highscore", steps);
			UIController.Instance.isHighscore = true;
		}
		// Stop music and sounds
		AudioManager.Instance.Stop("Music");
		AudioManager.Instance.Stop("Car Noise");
		// Notify other game components that it is game over
		CameraFollow.Instance.GameOver();
		UIController.Instance.GameOver();
		Time.timeScale = 0.4f;
		Destroy(player.gameObject);
	}

	#region Helper functions
	private void GenerateVehicle(Vector3 pos, Transform parent)
	{
		// Get the prefab. First get all vehicle prefabs then select randomly
		List<Prefab> vehiclePrefabs = new List<Prefab>();
		foreach(Prefab p in prefabs)
		{
			if (p.type == ObjectType.vehicle)
				vehiclePrefabs.Add(p);
		}
		if (vehiclePrefabs.Count == 0)
			return;
		GameObject prefab = vehiclePrefabs[Random.Range(0, vehiclePrefabs.Count)].gameObject;
		float angle = lanes[(int)pos.z].direction < 0 ? -90 : 90;	// Calculate movement direction and rotation for the car
		// Instantiate and create the vehicle and add it to list to keep track of it
		GameObject vehicle = Instantiate(prefab, pos, Quaternion.Euler(0, angle, 0), parent);
		Vector3 dir = lanes[(int)pos.z].direction > 0 ? Vector3.right : Vector3.left;
		vehicle.AddComponent<Vehicle>().SetValues(lanes[(int)pos.z].speed, dir, vehicleEnd/2);
		lanes[(int)pos.z].vehicles.Add(vehicle);
	}

	private void GenerateLog(Vector3 pos, Transform parent)
	{
		// Get the prefab and instantiate it
		GameObject logPrefab = null;
		foreach (Prefab p in prefabs)
			if (p.type == ObjectType.log)
				logPrefab = p.gameObject;
		if (logPrefab == null)
			return;
		GameObject log = Instantiate(logPrefab, pos, Quaternion.identity, parent);
		// Spawn based on direction and speed
		Vector3 direction = lanes[(int)pos.z].direction > 0 ? Vector3.right : Vector3.left;
		log.AddComponent<Vehicle>().SetValues(lanes[(int)pos.z].speed, direction, vehicleEnd/2);
		lanes[(int)pos.z].vehicles.Add(log);
	}

	private void GenerateCoin(Vector3 pos, Transform parent)
	{
		// Get the prefab and instantiate it
		GameObject prefab = null;
		foreach (Prefab p in prefabs)
			if (p.type == ObjectType.collectible)
				prefab = p.gameObject;
		if (prefab == null)
			return;
		GameObject coin = Instantiate(prefab, pos, Quaternion.identity, parent);
		// Add coin animation script to it and add to lane collectibles list
		lanes[(int)pos.z].collectibles.Add(coin);
	}

	private void GenerateTree(Vector3 pos, Transform parent)
	{
		// Get the prefab. First get all vehicle prefabs then select randomly
		List<Prefab> treePrefabs = new List<Prefab>();
		foreach (Prefab p in prefabs)
		{
			if (p.type == ObjectType.tree)
				treePrefabs.Add(p);
		}
		if (treePrefabs.Count == 0)
			return;
		GameObject prefab = treePrefabs[Random.Range(0, treePrefabs.Count)].gameObject;
		lanes[(int)pos.z].obstacles.Add(Instantiate(prefab, pos, Quaternion.identity, parent));
	}
	#endregion
}
