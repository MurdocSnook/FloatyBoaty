using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour {
	public int hp = 100;
	public GameObject[] damageStates;
	public DestructibleObjectMaterial objectMaterial = DestructibleObjectMaterial.WOOD;

	public enum DestructibleObjectMaterial {
		WOOD,
		STONE
	}

	private int maxHp;
	private int currentDamageState;

	private void Start() {
		maxHp = hp;
		currentDamageState = 0;
	}

	private void Update() {
		if(damageStates != null && damageStates.Length > 0) {
			int state = Mathf.Clamp(
				(int)((1f - (float) hp / maxHp) * damageStates.Length), 
				0, 
				damageStates.Length-1
			);

			if(state != currentDamageState) {
				ChangeState(state);
			}
		} 
	}

	private void ChangeState(int newState) {
		damageStates[currentDamageState].SetActive(false);
		damageStates[newState].SetActive(true);
		currentDamageState = newState;
	}

	public void DealDamage(int damage) {
		hp -= damage;
	}
}
