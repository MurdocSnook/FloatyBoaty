using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {
	public RiverGenerator riverGenerator;
	public RaftController raft;

	public static GameController instance;

	public static GameController GetInstance() {
		if(instance == null) {
			instance = Object.FindObjectOfType<GameController>();
		}
		return instance;
	}

	private void Start() {
		if(instance == null) {
			instance = this;
		} 
		else if(instance != this) {
			Destroy(this);
		}
	}


	private void OnValidate() {
		if(riverGenerator == null) {
			Debug.LogError("Please assign the RiverGenerator.");
		}
		if(raft == null) {
			Debug.LogError("Please assign the Raft.");
		}
	}
}
