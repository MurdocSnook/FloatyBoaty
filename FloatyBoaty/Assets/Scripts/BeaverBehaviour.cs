using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Creature))]
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
	public string[] evadeLayers = new string[] {"Bouncy", "Default"};

	[Header("Visuals")]
	public float fullTurnAngle = 30f;
	public float rotationLerp = 1f;
	[Tooltip("Animation curve determining when the beaver looks at the boat vs. just looking forward, depending on distance to target.")]
	public AnimationCurve lookAtBoatVSDirectionBlending;
	public float beaverSinkSpeed = 1f;

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
		// TODO: Prototype code, refactor later.

		GameController gc = GameController.GetInstance();
		RiverGenerator rg = gc.riverGenerator;
		Creature cr = GetComponent<Creature>();

		velocity += Vector3.ClampMagnitude(GetAcceleration() * Time.deltaTime, maxAcceleration);
		Vector3 waterSpeed = rg.GetWaterSpeedAt(transform.position);
		velocity += (waterSpeed - velocity) * waterResistance;

		velocity = Vector3.ClampMagnitude(velocity - waterSpeed, maxSpeed) + waterSpeed;
		velocity.Scale(new Vector3(1, 0, 1));

		if(cr.IsDead() && (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead"))) {
			velocity += Vector3.down * beaverSinkSpeed * Time.deltaTime;
		}

		transform.position += velocity * Time.deltaTime;

		if(!cr.IsDead()){
			Vector3 oldRotation = transform.rotation.eulerAngles;
			Quaternion desiredRotation = transform.rotation;
			if(target != null && lookAtBoatVSDirectionBlending != null) {
				float distanceToTarget = Vector3.Distance(target.position, transform.position);
				float lookAtTarget = lookAtBoatVSDirectionBlending.Evaluate(distanceToTarget);

				desiredRotation = Quaternion.Lerp(
					Quaternion.LookRotation(velocity - waterSpeed, Vector3.up), 
					Quaternion.LookRotation(target.position - transform.position, Vector3.up),
					lookAtTarget
				);
			}

			transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationLerp * Time.deltaTime);

			float rotationChange = Mathf.DeltaAngle(oldRotation.y, transform.rotation.eulerAngles.y) / Time.deltaTime;
			anim.SetFloat("Blend", rotationChange / fullTurnAngle);
		}
	}

	private Vector3 GetAcceleration() {
		Vector3 acc = Vector3.zero;
		Creature cr = GetComponent<Creature>();
		
		// avoid other objects
		Collider[] colliders = Physics.OverlapSphere(
			transform.position, 
			colliderCheckRadius, 
			LayerMask.GetMask(evadeLayers)
			);
		foreach (Collider col in colliders)
		{
			if(col.gameObject == this.gameObject) {
				continue;
			}
			Vector3 closestP;
			if(col.GetType() != typeof(MeshCollider) || ((MeshCollider) col).convex) {
				closestP = col.ClosestPoint(transform.position);
			} else {
				RaycastHit hit = new RaycastHit();
				for(int i = 0; i < 8; i++) {
					RaycastHit potentialHit;
					if (Physics.Raycast(
						transform.position,
						Quaternion.Euler(0, i * (360f/8f), 0) * Vector3.forward,
						out potentialHit,
						colliderCheckRadius,
						LayerMask.GetMask(evadeLayers)
					)) {
						if(hit.collider == null || potentialHit.distance < hit.distance) {
							hit = potentialHit;
						}
					}
				}
				if(hit.collider != null) {
					closestP = hit.point;
				} else {
					closestP = Random.onUnitSphere;
				}
			}
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

		if(!cr.IsDead()){
			if (target != null) {
				// move to target
				acc += (target.position - transform.position) * forwardsWeight;
				// straft
				acc += Vector3.Cross(target.position - transform.position, Vector3.up).normalized * strafeWeight;
			}
		}

		return acc;
	}
}
