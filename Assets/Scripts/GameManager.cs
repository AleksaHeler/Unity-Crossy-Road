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

enum LaneType
{
    safe = 0,
    road = 1,
    water = 2
}

public class GameManager : MonoBehaviour
{
    public bool useSpecifiedSeed = false;
    public int seed = 0;

    public Prefab[] prefabs;


    Dictionary<int, LaneType> lanes = new Dictionary<int, LaneType>();

    // Start is called before the first frame update
    void Awake()
    {
        // Setup random seed
        if (useSpecifiedSeed)
            Random.InitState(seed);
        else
            Random.InitState((int)System.DateTime.Now.Ticks);

        for (int i = -5; i < 10; i++)
		{
            GenerateLane(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GenerateLane(int lane)
	{
		// If the lane is not yet defined, set it to random type
		if (!lanes.ContainsKey(lane))
        {
            // TODO: add biases for different lanes
            if (lane == 0) // If it is the first lane, it should always be safe
                lanes.Add(lane, LaneType.safe);
            else
                lanes.Add(lane, (LaneType)Random.Range(0, 3));
        }

        // Get the type of current lane
        LaneType laneType = lanes[lane];

        // Go trough the lane and place tiles
        for (int i = -10; i < 10; i++)
        {
            GameObject go = null;

            switch (laneType)
            {
                case LaneType.safe:
                    go = prefabs[0].gameObject;
                    break;
                case LaneType.road:
                    go = prefabs[1].gameObject;
                    break;
                case LaneType.water:
                    go = prefabs[3].gameObject;
                    break;
                default:
                    break;
            }
            if(go != null)
                GameObject.Instantiate(go, new Vector3(i, 0, lane), Quaternion.identity);

            if(laneType == LaneType.safe && Random.Range(0,10) < 2)
			{
                GameObject.Instantiate(prefabs[Random.Range(4, 8)].gameObject, new Vector3(i, 0, lane), Quaternion.identity);
            }
        }
	}
}
