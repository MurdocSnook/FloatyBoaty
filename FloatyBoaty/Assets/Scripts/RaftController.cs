using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

[RequireComponent(typeof(Rigidbody))]
public class RaftController : MonoBehaviour {
	public GameObject rudder;
	public RiverGenerator riverGenerator;
	public float baseWaterSpeed = 1f;
	public float baseWaterForce = 1f;
	public float rotationForce = 2f;

	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate() {
		Rigidbody rb = GetComponent<Rigidbody>();

		Vector3 waterSpeed = riverGenerator.GetWaterDirectionAt(transform.position) * baseWaterSpeed;

		// apply water force
		Vector3 relativeSpeed = (waterSpeed - rb.velocity) * baseWaterForce;
		rb.AddForce(relativeSpeed);

		// simulate rudder as if we were on a sail boat
		// calculate water pushing against rudder
		Vector3 perceivedWaterVelocity = -rb.velocity;
		// calculate thrust caused by the pushing water
		Vector3 thrust = Vector3.Dot(rudder.transform.forward, perceivedWaterVelocity) * rudder.transform.forward;
		// apply thrust at rudder
		rb.AddForceAtPosition(thrust * rotationForce, rudder.transform.position);
	}
	
	void Update () {
		VRTK_DeviceFinder.PlayAreaTransform().position = transform.position;
		VRTK_DeviceFinder.PlayAreaTransform().rotation = transform.rotation;
	}

	private void OnValidate() {
		if(riverGenerator == null) {
			Debug.LogError("Please assign the river generator.");
		}
	}
}
