using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {
	public RiverGenerator riverGenerator;
	public RaftController raft;
	public GameObject deathRoom;

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

	private void Update() {
		Transform head = VRTK_DeviceFinder.HeadsetCamera();
		if(head != null && head.position.y <= 0) {
			raft.gameObject.SetActive(false);
			deathRoom.SetActive(true);
			VRTK.VRTK_DeviceFinder.PlayAreaTransform().position = deathRoom.transform.position;
		}
	}

	public void QuickRestartGame() {
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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
