using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LaneType
{
    Grass = 0,
    Road = 1,
    Water = 2
}

/// <summary>
/// Class describing a lane and containing all objects in it in Lists of GameObjects
/// </summary>
public class Lane
{
    public LaneType type;                       // safe, road, water
    public int direction;                       // Vehicles: left/right
    public float speed;                         // Speed of all vehicles on the lane
    public GameObject ground;                   // All tiles placed in this lane
    public List<GameObject> vehicles;           // All vehicles in this lane (including logs)
    public List<GameObject> obstacles;          // All obstacles in this lane
    public List<GameObject> collectibles;       // All collectibles in this lane
    public Transform parent;                    // All entities in lane are child of this objecy

    // Empty constructor is safe lane
    public Lane() : this(LaneType.Grass) { }

    // Constructor
    public Lane(LaneType t)
    {
        type = t;
        vehicles = new List<GameObject>();
        obstacles = new List<GameObject>();
        collectibles = new List<GameObject>();
    }
}
