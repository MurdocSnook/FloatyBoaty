﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {
	public int hp;
	private bool dead;

	public void DealDamage(int damage) {
		hp -= damage;
		Animator a = GetComponentInChildren<Animator>();
		if(a != null) {
			a.SetTrigger("Hit");
		}
	}

	public bool IsDead() {
		return dead;
	}
	
	private void Update() {		
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
        GetComponent<AudioSource>().Play();
    }
}
