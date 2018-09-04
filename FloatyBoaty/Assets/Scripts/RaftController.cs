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
	public float centreboardForce = 1f;

	// Use this for initialization
	void Start () {
		
	}

	void FixedUpdate() {
		Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 waterDir = riverGenerator.GetWaterDirectionAt(transform.position);
        Vector3 waterSpeed = waterDir * baseWaterSpeed;

		// apply water force
		Vector3 relativeSpeed = (waterSpeed - rb.velocity) * baseWaterForce;
		rb.AddForce(relativeSpeed);

		// calculate water pushing against boat (kind of fake, ignores that water is moving with boat)
		Vector3 perceivedWaterVelocity = -rb.velocity;

		// calculate thrust caused by the pushing water on the raft
		Vector3 thrustRaft = Vector3.Dot(transform.forward, perceivedWaterVelocity) * transform.forward;
		// apply thrust at centreboard
		rb.AddForceAtPosition(thrustRaft * centreboardForce, transform.position);

		// calculate thrust caused by the pushing water on the rudder
		Vector3 thrustRudder = Vector3.Dot(rudder.transform.forward, perceivedWaterVelocity) * rudder.transform.forward;
		// apply thrust at rudder
		rb.AddForceAtPosition(thrustRudder * rotationForce, rudder.transform.position);
	}
	
	void Update () {
        Transform playAreaTransform = VRTK_DeviceFinder.PlayAreaTransform();
		if(playAreaTransform != null) {
			playAreaTransform.position = transform.position;
	        playAreaTransform.rotation = transform.rotation;
		}
	}

	private void OnValidate() {
		if(riverGenerator == null) {
			Debug.LogError("Please assign the river generator.");
		}
	}
}
