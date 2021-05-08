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
 */


public class GameManager : MonoBehaviour
{
    // Parameters
    public int generateNLanes = 10;
    public bool useSpecifiedSeed = false;
    public int seed = 0;
    public float obstacleFrequency = 0.2f;
    public float vehicleFrequency = 0.2f;
    public float logFrequency = 0.2f;
    public float collectibleFrequency = 0.1f;
    public int laneWidth = 10;
    public float minSpeed = 0.7f;
    public float maxSpeed = 2f;
    public int playerMaxMove = 5; // how many tiles can player move left/right not to go off the screen

    // Variables
    public Prefab[] prefabs;                                    // Prefabs to spawn for tiles, cars, trees...
    private GameObject environment;                             // Put everything in one gameobject
    Dictionary<int, Lane> lanes = new Dictionary<int, Lane>();  // Keeping track of lanes (Lane class)

    // Start is called before the first frame update
    void Awake()
    {
        // Create the environment gameObject
        environment = new GameObject("Environment");

        // Setup random seed
        if (useSpecifiedSeed)
            Random.InitState(seed);
        else
            Random.InitState((int)System.DateTime.Now.Ticks);

        // Hard coded generate some number of lanes in advance
        for (int i = -5; i < 50; i++)
		{
            GenerateLane(i);
        }
    }

	private void Start()
	{
        // Start playing car sounds
        AudioManager.Instance.Play("Car Noise");
	}

	private void GenerateLane(int lane)
	{
		// If the lane is not yet defined, set it to random type
		if (!lanes.ContainsKey(lane))
        {
            // TODO: add biases for different lanes
            if (lane == 0) // If it is the first lane, it should always be safe
                lanes.Add(lane, new Lane());
            else
                lanes.Add(lane, new Lane((LaneType)Random.Range(0, 3)));

            // Set lane speed and direction
            lanes[lane].direction = Random.Range(0, 100) > 50 ? -1 : 1;
            lanes[lane].speed = Random.Range(minSpeed, maxSpeed);
        }

        // Get the type of current lane
        LaneType laneType = lanes[lane].type;

        // Create an empty game object to hold the lane pieces and set it to be child of environment
        GameObject laneGameObject = new GameObject("Lane " + lane);
        laneGameObject.transform.parent = environment.transform;

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

            // On roads put cars at given frequency
            if(laneType == LaneType.road && Random.Range(0, 100) < 100 * vehicleFrequency && !vehiclePrevious)
			{
                // Mark for next iteration not to have car (too close)
                vehiclePrevious = true;
                // Get the prefab and position
                GameObject prefab = prefabs[Random.Range(8, 15)].gameObject;
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
            if (laneType == LaneType.water && Random.Range(0, 100) < 100 * logFrequency && logPrevious <= 0)
            {
                // Get the prefab and position
                GameObject prefab = prefabs[15].gameObject;
                Vector3 pos = new Vector3(i, 0.1f, lane);
                GameObject vehicle = Instantiate(prefab, pos, Quaternion.identity, laneGameObject.transform);
                Vector3 dir;
                float size = Random.Range(2, 5);
                logPrevious = (int)size;
                // Spawn based on direction and speed
                dir = lanes[lane].direction < 0 ? dir = Vector3.left : Vector3.right;
                vehicle.AddComponent<LogController>().SetValues(lanes[lane].speed, dir, laneWidth, size);
                lanes[lane].vehicles.Add(vehicle);
            }
            else if(logPrevious > 0) { logPrevious--; }

            // If we are on a safe lane inside player move zone place coins randomly
            if(lanes[lane].type == LaneType.safe && i > -playerMaxMove && i < playerMaxMove && lane > 0 && Random.Range(0.0f, 1f) < collectibleFrequency)
			{
                // Get the prefab and instantiate it
                GameObject prefab = prefabs[16].gameObject;
                Vector3 pos = new Vector3(i, 0.5f, lane);
                GameObject coin = Instantiate(prefab, pos, Quaternion.identity, laneGameObject.transform);
                // Add coin animation script to it and add to lane collectibles list
                coin.AddComponent<Coin>().SetValues(0.2f, 1.5f, 90);
                lanes[lane].collectibles.Add(coin);
            }
        }

        // If the lane is water and no logs were added, add one here
        if (lanes[lane].type == LaneType.water && lanes[lane].vehicles.Count == 0)
		{
            // Get the prefab and position
            GameObject logPrefab = prefabs[15].gameObject;
            Vector3 pos = new Vector3(0, 0.1f, lane);
            GameObject log = Instantiate(logPrefab, pos, Quaternion.identity, laneGameObject.transform);
            Vector3 dir;
            float size = Random.Range(2, 5);
            // Spawn based on direction and speed
            dir = lanes[lane].direction < 0 ? Vector3.left : Vector3.right;
            log.AddComponent<LogController>().SetValues(lanes[lane].speed, dir, laneWidth, size);
            lanes[lane].vehicles.Add(log);
        }
    }
}
