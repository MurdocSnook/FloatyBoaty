using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour {
	public int hp;
	public AudioSource deathsound;
	public AudioSource hitsound;
	private bool dead;

	public void DealDamage(int damage) {
		hp -= damage;
		Animator a = GetComponentInChildren<Animator>();
		if(a != null) {
			a.SetTrigger("Hit");
		}
        hitsound.pitch = (Random.Range(0.8f, 1.3f));
        hitsound.Play();
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
		
        deathsound.Play();
    }
}
