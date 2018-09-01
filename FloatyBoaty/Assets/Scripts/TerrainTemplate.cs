using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTemplate : MonoBehaviour {
	public GameObject entranceMarker;
	public GameObject exitMarker;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnValidate() {
		if (entranceMarker == null || exitMarker == null) {
			Debug.LogError("Entrance and exit markers must be assigned.", this);
		}
	}
}
