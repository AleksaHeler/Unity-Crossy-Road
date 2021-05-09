using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    grass,
    road,
    water,
    vehicle,
    tree,
    collectible
}

/// <summary>
/// Class used to describe tiles, cars, trees... that will be spawned in game
/// </summary>
[System.Serializable]
public class Prefab
{
	public string name;
    public ObjectType type;
    public GameObject gameObject;
}
