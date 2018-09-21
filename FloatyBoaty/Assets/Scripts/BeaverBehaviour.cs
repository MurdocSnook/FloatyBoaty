using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Creature))]
public class BeaverBehaviour : MonoBehaviour {
	public Transform target;
	[Tooltip("Used to judge the distance to target more precisely.")]
	public Collider targetCollider;
	[Header("Avoid collisions")]
	public float pushStrength = 2.5f;

	[Header("Movement")]
	public float maxSpeed = 1f;
	public float maxAcceleration = 1f;
	[Range(0f, 1f)]
	public float waterResistance = 0.2f;
	public float forwardsWeight = 1f;
	public float strafeWeight = 1f;
	public string[] evadeLayers = new string[] {"Bouncy", "Default"};

    [Header("Attack")]
    public AudioSource munch;
	public int attackDamage;
	public float attackCooldown = 1f;
	public GameObject attackPoint;
	public string[] attackLayerMask = new string[] {"Default"};
	private float attackTimer = 0f;
	private DestructibleObject objectAttacking = null;

	[Header("Visuals")]
	public float fullTurnAngle = 30f;
	public float rotationLerp = 1f;
	[Tooltip("Animation curve determining when the beaver looks at the boat vs. just looking forward, depending on distance to target.")]
	public AnimationCurve lookAtBoatVSDirectionBlending;
	public float beaverSinkSpeed = 1f;

	private Vector3 velocity;

	private void Start() {
		strafeWeight *= Random.Range(-1f, 1f);

		GameController gc = GameController.GetInstance();

		// look for target
		if(gc.raft != null) {
			target = gc.raft.transform;
		}
		else if(target == null) {
			GameObject o = GameObject.Find("Raft");
			if(o != null) {
				target = o.transform;
			}
		}

		// look for target collider
		if(target != null && targetCollider == null) {
			BoxCollider bc = target.gameObject.GetComponentInChildren<BoxCollider>();
			if(bc != null) {
				targetCollider = bc;
			}
		}
	}

	private void Update() {
		Animator anim = GetComponentInChildren<Animator>();
		GameController gc = GameController.GetInstance();
		RiverGenerator rg = gc.riverGenerator;
		Creature cr = GetComponent<Creature>();

		// apply acceleration
		velocity += Vector3.ClampMagnitude(GetAcceleration(), maxAcceleration) * Time.deltaTime;
		// apply water resistance
		Vector3 waterSpeed = rg.GetWaterSpeedAt(transform.position);
		velocity += (waterSpeed - velocity) * waterResistance * Time.deltaTime;

		// enforce max velocity relative to water speed
		velocity = Vector3.ClampMagnitude(velocity - waterSpeed, maxSpeed) + waterSpeed;
		
		velocity.Scale(new Vector3(1, 0, 1));

		// Apply gravity when dead
		if(cr.IsDead() && (anim.GetCurrentAnimatorStateInfo(0).IsName("Dead"))) {
			velocity += Vector3.down * beaverSinkSpeed * Time.deltaTime;
		}

		// move according to velocity
		transform.position += velocity * Time.deltaTime;

		// calculate the beaver's rotation and animation blending
		if(!cr.IsDead()){
			Vector3 oldRotation = transform.rotation.eulerAngles;
			Quaternion desiredRotation = transform.rotation;

			// figure out where the beaver wants to look
			if(target != null && lookAtBoatVSDirectionBlending != null) {
				float distanceToTarget = Vector3.Distance(target.position, transform.position);

				// Use more precise distance if target collider is available
				if(targetCollider != null) {
					distanceToTarget = Vector3.Distance(targetCollider.ClosestPoint(transform.position), transform.position);
				}

				// blend between looking to boat and looking forwards depending on distance
				float lookAtTarget = lookAtBoatVSDirectionBlending.Evaluate(distanceToTarget);
				desiredRotation = Quaternion.Lerp(
					Quaternion.LookRotation(velocity - waterSpeed, Vector3.up), 
					Quaternion.LookRotation(target.position - transform.position, Vector3.up),
					lookAtTarget
				);
			}

			// "Lerp" the beaver towards the desired rotation over time
			transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, rotationLerp * Time.deltaTime);

			// Set the beavers apparent curvature depending on rotation change
			float rotationChange = Mathf.DeltaAngle(oldRotation.y, transform.rotation.eulerAngles.y) / Time.deltaTime;
			anim.SetFloat("Blend", rotationChange / fullTurnAngle);

			// determine if attacking
			if(attackTimer == 0f && !anim.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
				// look for attackable objects
				SphereCollider attackSphere = attackPoint.GetComponent<SphereCollider>();
				Collider[] attackResult = Physics.OverlapSphere(attackPoint.transform.position, attackSphere.radius, LayerMask.GetMask(attackLayerMask));
				foreach (Collider col in attackResult)
				{
					DestructibleObject d = col.gameObject.GetComponentInParent<DestructibleObject>();
					if(d != null && d.objectMaterial == DestructibleObject.DestructibleObjectMaterial.WOOD) {
						// attack!
						objectAttacking = d;
						anim.SetTrigger("Attack");
						attackTimer = attackCooldown;
					}
				}
			}
		}

		// Update timers
		attackTimer -= Mathf.Min(Time.deltaTime, attackTimer);
	}

	// Is called by animation event
	public void DealAttackDamage() {
		if(objectAttacking != null) {
			objectAttacking.DealDamage(attackDamage);
			ParticleSystem ps = attackPoint.GetComponentInChildren<ParticleSystem>();
			if(ps != null) {
				ps.Play();
                // ps.Emit(ps.emission.GetBurst(0).minCount);
                munch.PlayOneShot(munch.clip);
			}
		}
	}

	private Vector3 GetAcceleration() {
		Vector3 acc = Vector3.zero;
		Creature cr = GetComponent<Creature>();
		SphereCollider sc = GetComponent<SphereCollider>();
		
		// avoid other objects
		Collider[] colliders = Physics.OverlapSphere(
			transform.position, 
			sc.radius, 
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
						sc.radius,
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

	private void OnValidate() {
		if (targetCollider != null && targetCollider.GetType() == typeof(MeshCollider) && !((MeshCollider) targetCollider).convex) {
			Debug.LogError("Target collider can't be non-convex mesh!");
		}
	}
}
