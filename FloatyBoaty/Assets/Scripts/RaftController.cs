using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class RaftController : MonoBehaviour {
	public RiverGenerator riverGenerator;
	public float waterBaseSpeed = 1f;

	// Use this for initialization
	void Start () {
		
	}
	
	void FixedUpdate () {
		Vector3 velocity = riverGenerator.GetWaterDirectionAt(transform.position) * waterBaseSpeed;

		transform.position += velocity * Time.fixedDeltaTime;
	}

	private void Update() {
		VRTK_DeviceFinder.PlayAreaTransform().position = transform.position;
	}

	private void OnValidate() {
		if(riverGenerator == null) {
			Debug.LogError("Please assign the river generator.");
		}
	}
}
