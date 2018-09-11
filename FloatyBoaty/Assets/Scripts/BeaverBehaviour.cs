using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class BeaverBehaviour : MonoBehaviour {
	public Transform target;
	[Header("Avoid collisions")]
	public float pushStrength = 2.5f;
	public float colliderCheckRadius = 0.7f;

	[Header("Movement")]
	public float maxSpeed = 1f;
	public float maxAcceleration = 1f;
	[Range(0f, 1f)]
	public float waterResistance = 0.2f;
	public float forwardsWeight = 1f;
	public float strafeWeight = 1f;

	[Header("Visuals")]
	public float fullTurnAngle = 30f;
	public float rotationLerp = 1f;
	public float beaverViewSwitchOverSpeed = 1f;

	private Vector3 velocity;

	private void Start() {
		GameController gc = GameController.GetInstance();

		if(gc.raft != null) {
			target = gc.raft.transform;
		}
		else if(target == null) {
			GameObject o = GameObject.Find("Raft");
			if(o != null) {
				target = o.transform;
			}
		}
	}

	private void Update() {
		Animator anim = GetComponentInChildren<Animator>();
		if(anim != null && anim.GetCurrentAnimatorStateInfo(0).IsName("Dead")) {
			transform.position = transform.position + Vector3.down * Time.deltaTime;
		} 
		else if(anim == null || !anim.GetCurrentAnimatorStateInfo(0).IsName("BeaverDie")) {
			// TODO: Prototype code, refactor later.
			Vector3 oldRotation = transform.rotation.eulerAngles;

			GameController gc = GameController.GetInstance();
			RiverGenerator rg = gc.riverGenerator;

			velocity += Vector3.ClampMagnitude(GetAcceleration() * Time.deltaTime, maxAcceleration);
			Vector3 waterSpeed = rg.GetWaterSpeedAt(transform.position);
			velocity += (waterSpeed - velocity) * waterResistance;

			velocity = Vector3.ClampMagnitude(velocity - waterSpeed, maxSpeed) + waterSpeed;
			velocity.Scale(new Vector3(1, 0, 1));

			transform.position += velocity * Time.deltaTime;

			Quaternion newRotation = transform.rotation;
			if(velocity.magnitude > beaverViewSwitchOverSpeed) {
				newRotation = Quaternion.LookRotation(velocity - waterSpeed, Vector3.up);
			} else
			if(target != null) {
				newRotation = Quaternion.Lerp(
					Quaternion.LookRotation(target.position - transform.position, Vector3.up),
					Quaternion.LookRotation(velocity - waterSpeed, Vector3.up), 
					velocity.magnitude / beaverViewSwitchOverSpeed
				);
			}

			transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, rotationLerp * Time.deltaTime);

			float rotationChange = (transform.rotation.eulerAngles.y - oldRotation.y) / Time.deltaTime;
			anim.SetFloat("Blend", rotationChange / fullTurnAngle);
		}
	}

	private Vector3 GetAcceleration() {
		SphereCollider sc = GetComponent<SphereCollider>();
		Vector3 acc = Vector3.zero;
		
		// avoid other objects
		if(sc != null) {
			Collider[] colliders = Physics.OverlapSphere(
				transform.position, 
				sc.radius, 
				LayerMask.GetMask(new string[] {"Bouncy", "Default"})
				);
			foreach (Collider col in colliders)
			{
				if(col.gameObject == this.gameObject) {
					continue;
				}

				Vector3 closestP = col.ClosestPoint(transform.position);
				Vector3 dif = transform.position - closestP;
				// check if too small, then fix
				if(dif.magnitude < 0.01f) {
					dif = transform.position - col.gameObject.transform.position;
				}
				// for safety check if still too small
				if(dif.magnitude > 0.01f) {
					acc += dif.normalized * (pushStrength / dif.magnitude);
				}
			}
		}

		if (target != null) {
			// move to target
			acc += (target.position - transform.position) * forwardsWeight;
			// straft
			acc += Vector3.Cross(target.position - transform.position, Vector3.up).normalized * strafeWeight;
		}

		return acc;
	}
}
