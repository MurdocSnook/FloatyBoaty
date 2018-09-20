using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverEvent : MonoBehaviour {
	public Utils.SpawnOption[] spawnOptions = new Utils.SpawnOption[] {new Utils.SpawnOption()};
	public int amount;
	public bool setParent;
	public SpawnPoint.SpawnPointType spawnpointType; 
	public float activationDistance = 12f;

	[HideInInspector]
	public TerrainTemplate terrainTemplate;

	private void Update() {
		GameObject raft = GameController.instance.raft.gameObject;

		if(Vector3.Distance(raft.transform.position, transform.position) < activationDistance) {
			// get a list of viable spawn points in random order
			List<SpawnPoint> points = new List<SpawnPoint>();
			foreach (SpawnPoint p in terrainTemplate.GetComponentsInChildren<SpawnPoint>())
			{
				if(!p.occupied && p.type == spawnpointType) {
					int positionToInsert = Random.Range(0, points.Count+1);
					points.Insert(positionToInsert, p);
				}
			}
			
			// spawn objects
			for(int i = 0; i < amount && i < points.Count; i++) {
				GameObject prefabToSpawn = Utils.Choose(spawnOptions);

				if(setParent) {
					GameObject instance = Instantiate(prefabToSpawn, points[i].transform);
				} else {
					GameObject instance = Instantiate(prefabToSpawn, points[i].transform.position, points[i].transform.rotation);
				}

				points[i].occupied = true;
			}

			this.gameObject.SetActive(false);
		}
	}
}
