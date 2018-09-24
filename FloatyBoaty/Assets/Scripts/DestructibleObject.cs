using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour {
	public int hp = 100;
	public GameObject[] damageStates;
	public DestructibleObjectMaterial objectMaterial = DestructibleObjectMaterial.WOOD;
    public AudioSource statechangesound;
    public AudioSource hitsound;

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
				(int)((1f - (float) hp / maxHp) * (damageStates.Length - 1)), 
				0, 
				damageStates.Length-1
			);
			if(hp <= 0) {
				state = damageStates.Length - 1;
			}

			if(state != currentDamageState) {
				ChangeState(state);
                statechangesound.pitch = (Random.Range(0.6f, 1.5f));
                statechangesound.Play();
            }
		} 
	}

	private void ChangeState(int newState) {
		damageStates[currentDamageState].SetActive(false);
		damageStates[newState].SetActive(true);
		currentDamageState = newState;
	}

	public GameObject GetCurrentActiveObject() {
		return damageStates[currentDamageState];
	}

	public void DealDamage(int damage) {
		hp -= damage;
        hitsound.pitch = (Random.Range(0.8f, 1.3f));
        hitsound.PlayOneShot(hitsound.clip);
    }
}
