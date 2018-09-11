using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	private Vector3 lastPosisiton;

	public int damage = 5;
	public bool isPiercing = false;
	public float inactiveDistance = .5f;

	private Vector3 initialPosition;

	private void Start() {
		initialPosition = transform.position;
		lastPosisiton = transform.position;
	}

	private void Update() {
		RaycastHit hit;
		Vector3 dif = transform.position - lastPosisiton;
		bool success = Physics.Raycast(lastPosisiton, dif.normalized, out hit, dif.magnitude);

		if(success) {
			if(Vector3.Distance(initialPosition, hit.point) >= inactiveDistance) {	
				Creature creature = hit.collider.gameObject.GetComponent<Creature>();

				if(creature != null) {
					creature.DealDamage(damage);
				}

				if(!isPiercing) {
					Destroy(this.gameObject);
				}
			}
		}

		lastPosisiton = transform.position;
	}
}
