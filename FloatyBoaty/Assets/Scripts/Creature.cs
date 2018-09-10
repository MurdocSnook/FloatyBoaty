using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {
	public int hp;
	private bool dead;

	public void DealDamage(int damage) {
		hp -= damage;
	}
	public void Update() {
		Animator anim = GetComponent<Animator>();
		
		if (hp <= 0 && !dead) {
			dead = true;
			Die();
		}
	}

	protected void Die() {
		Animator anim = GetComponent<Animator>();
		if(anim != null) {
			anim.SetTrigger("Die");
		}
	}
}
