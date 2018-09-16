using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EasyButtons;

public class RiverGenerator : MonoBehaviour {
	[Header("Generation")]
	public GameObject playerObject;
	public float generationDistance;

	public GameObject terrainContainer;
	public TerrainTemplate[] templates;
	public int numberOfLoadedTemplates = 6;

	[Header("Water movement")]
	public float baseWaterSpeed = 1f;

	private List<TerrainTemplate> buffer;
	private List<TerrainTemplate> currentlyLoadedTemplates;

	// Use this for initialization
	void Start () {
		buffer = new List<TerrainTemplate>();
		currentlyLoadedTemplates = new List<TerrainTemplate>();
	}
	
	// Update is called once per frame
	void Update () {
		if(currentlyLoadedTemplates.Count < numberOfLoadedTemplates) {
			Generate();
		} else {
			int emergencyBreak = 1000;
			while (true) {
				Vector3 p1 = currentlyLoadedTemplates[currentlyLoadedTemplates.Count/2 - 1].gameObject.transform.position;
				Vector3 p2 = currentlyLoadedTemplates[currentlyLoadedTemplates.Count/2].gameObject.transform.position;
				
				if(Vector3.Distance(p1, playerObject.transform.position) < Vector3.Distance(p2, playerObject.transform.position)) {
					break;
				}
				
				Generate();

				if(emergencyBreak-- < 0) {
					throw new UnityException("Emergency Break hit, River Generator endless loop.") ;
				}
			}
		}
	}

	[Button]
	// generates one segment
	void Generate() {
		// repopulate buffer on demand
		if(buffer.Count == 0) {
			foreach (TerrainTemplate t in templates)
			{
				int positionToInsert = Random.Range(0, buffer.Count + 1);
				buffer.Insert(positionToInsert, t);
			}
		}

		// choose random segment
		int i = Random.Range(0, buffer.Count);
		TerrainTemplate template = buffer[i];
		buffer.RemoveAt(i);
		GameObject prefab = template.gameObject;

		// instantiate at current position
		GameObject instance = Instantiate(prefab, 
			transform.position, 
			transform.rotation, //transform.rotation,
			terrainContainer.transform
		);

		GameObject exitObj = instance.GetComponent<TerrainTemplate>().exitMarker;
		GameObject entranceObj = instance.GetComponent<TerrainTemplate>().entranceMarker;

		// adjust the instance's rotation for entrance rotation
		instance.transform.rotation = instance.transform.rotation 
			* Quaternion.Inverse(entranceObj.transform.localRotation);
		
		// adjust the instance's position for entrance position
		instance.transform.position = instance.transform.position 
			+ (transform.position - entranceObj.transform.position);

		// set position and rotation of generator to exit 
		transform.position = exitObj.transform.position;
		transform.rotation = exitObj.transform.rotation;

		// deactivate exit and entrance objects
		exitObj.SetActive(false);
		entranceObj.SetActive(false);

		// add to the loaded templates
		currentlyLoadedTemplates.Add(instance.GetComponent<TerrainTemplate>());
		if(currentlyLoadedTemplates.Count > numberOfLoadedTemplates) {
			Destroy(currentlyLoadedTemplates[0].gameObject);
			currentlyLoadedTemplates.RemoveAt(0);
		}
	}

	public Vector3 GetWaterSpeedAt(Vector3 position) {
		return GetWaterDirectionAt(position) * baseWaterSpeed;
	}

	public Vector3 GetWaterDirectionAt(Vector3 position) {
		float minDist = float.PositiveInfinity;
		Vector3 flowDir = Vector3.zero;

		foreach (TerrainTemplate t in currentlyLoadedTemplates)
		{
			List<Vector3> curve = t.Curve;
			for(int i = 0; i < curve.Count; i++) {
				if(Vector3.Distance(curve[i], position) < minDist) {
					minDist = Vector3.Distance(curve[i], position);
					int p1 = Mathf.Max(i-1, 0);
					int p2 = Mathf.Min(i+1, curve.Count-1);
					flowDir = (curve[p2] - curve[p1]).normalized;
				}
			}
		}

		return flowDir;
	}

	private void OnValidate() {
		if(numberOfLoadedTemplates < 2) {
			Debug.LogError("numberOfLoadedTemplates should be at least 2.");
		}
	}
}
