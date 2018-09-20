using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour {
	public enum SpawnPointType {
		WATER,
		LAND,
		COVER
	}

	public SpawnPointType type;
	public bool occupied;
}
