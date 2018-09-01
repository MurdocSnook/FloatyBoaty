using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverGenerator : MonoBehaviour {
	public GameObject playerObject;
	public float generationDistance;

	public GameObject terrainContainer;
	public TerrainTemplate[] templates;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		while (Vector3.Distance(transform.position, playerObject.transform.position) < generationDistance) {
			Generate();
		}
	}

	// generates one segment
	void Generate() {
		// choose random segment
		int i = Random.Range(0, templates.Length);
		TerrainTemplate template = templates[i];
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
	}
}
