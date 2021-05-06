using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ObjectType
{
    grass,
    road,
    water,
    vehicle,
    tree
}

[System.Serializable]
public class Prefab
{
	public string name;
    public ObjectType type;
    public GameObject gameObject;
}
