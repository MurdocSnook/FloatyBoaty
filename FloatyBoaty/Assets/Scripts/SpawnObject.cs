using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

public class SpawnObject : MonoBehaviour {
	public bool setParent = true;
	public bool spawnOnStart = true;
	
    [System.Serializable]
    public class SpawnOption
    {
        public GameObject prefab = null;
        public float weight = 1;
    }

	public SpawnOption[] spawnOptions = new SpawnOption[] {new SpawnOption()};

	private void Start() {
		if(spawnOnStart) {
			Spawn();
		}
	}
	
	[Button]
	public void Spawn() {
		SpawnOption t = ChooseSpawnOption();
		if(t != null && t.prefab != null) {
			GameObject obj = Instantiate(t.prefab, transform.position, transform.rotation);
			if(setParent) {
				obj.transform.SetParent(transform);
			}
		}
	}

	private SpawnOption ChooseSpawnOption()
    {
        float weightSum = 0;
        foreach (SpawnOption t in spawnOptions)
        {
            weightSum += t.weight;
        }
        float choice = UnityEngine.Random.Range(0f, weightSum);
        for (int i = 0; i < spawnOptions.Length; i++)
        {
            choice -= spawnOptions[i].weight;
            if (choice <= 0)
            {
                return spawnOptions[i];
            }
        }
        return spawnOptions[spawnOptions.Length - 1];
    }

	private void OnValidate() {
		if(spawnOptions.Length == 0) {
			Debug.LogError("Please add at least one entry to templates (prefab attribute can be null).");
		}
	}
}
