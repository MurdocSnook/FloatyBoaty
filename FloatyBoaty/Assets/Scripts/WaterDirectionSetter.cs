using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterDirectionSetter : MonoBehaviour {
	public float updateThreshold = 0.0001f;
	public float speedMultiplier = 30f;

	private void Update() {
		MeshRenderer mr = GetComponent<MeshRenderer>();
		Vector4 waterMovement = mr.material.GetVector("_BumpDirection");
		Vector3 newWaterMovement = GameController.instance.riverGenerator.GetWaterSpeedAt(transform.position) * speedMultiplier;

		Vector4 waterMovementConverted = new Vector4(0, 0, -newWaterMovement.x, -newWaterMovement.z);

		if(Vector4.Distance(waterMovement, waterMovementConverted) > updateThreshold) {
			mr.material.SetVector("_BumpDirection", waterMovementConverted);
		}
	}
}
