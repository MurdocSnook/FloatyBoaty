using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
	private Vector3 lastPosisiton;

	public int damage = 5;

	private void Start() {
		lastPosisiton = transform.position;
	}

	private void FixedUpdate() {
		RaycastHit hit;
		Vector3 dif = transform.position - lastPosisiton;
		bool success = Physics.Raycast(lastPosisiton, dif.normalized, out hit, dif.magnitude);

		if(success) {
			Creature poorGuy = hit.collider.gameObject.GetComponent<Creature>();
			poorGuy.DealDamage(damage);
		}

		lastPosisiton = transform.position;
	}
}
