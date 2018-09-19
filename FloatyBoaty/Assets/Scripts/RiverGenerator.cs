using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using EasyButtons;

// uses animator with GeneratorState behaviours as a state machine
[RequireComponent(typeof(Animator))]
public class RiverGenerator : MonoBehaviour {
	[Header("Generation")]
	public GameObject playerObject;
	public float generationDistance;

	public GameObject terrainContainer;

	[Header("Water movement")]
	public float baseWaterSpeed = 1f;

    private List<TerrainTemplate> buffer;
	private List<TerrainTemplate> currentlyLoadedTemplates;
    private int templatesGenerated;

    public int TemplatesGenerated
    {
        get
        {
            return templatesGenerated;
        }
    }

    // Use this for initialization
    void Start () {
		buffer = new List<TerrainTemplate>();
		currentlyLoadedTemplates = new List<TerrainTemplate>();
		templatesGenerated = 0;

		if(playerObject == null) {
			playerObject = GameController.GetInstance().raft.gameObject;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(Vector3.Distance(playerObject.transform.position, transform.position) < generationDistance) {
			Generate();
		}

		if (currentlyLoadedTemplates.Count > 0) {
			TerrainTemplate t = currentlyLoadedTemplates[0];
			if(Vector3.Distance(playerObject.transform.position, t.gameObject.transform.position) > generationDistance) {
				Destroy(t.gameObject);
			}
		}
	}

	[Button]
	// generates one segment
	void Generate() {
        // choose random segment
		// get templates frow buffer, while discarding null templates
		TerrainTemplate template = null;

		while (template == null) {
			if(buffer.Count == 0) {
				// get new templates
				GetComponent<Animator>().SetTrigger("transit");
				GetComponent<Animator>().SetFloat("random", Random.Range(0f, 1f));
				return;
			}
			int i = Random.Range(0, buffer.Count);
			template = buffer[i];
			buffer.RemoveAt(i);
		} 
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

		// increase counter
		templatesGenerated++;
	}

    public void RepopulateBuffer(TerrainTemplate[] selection, TerrainTemplate[] uniques, int number, float uniqueChance)
    {
		buffer.Clear();

		int i = 0;

		List<TerrainTemplate> uniquesBuf = new List<TerrainTemplate>();
		foreach (TerrainTemplate u in uniques)
		{
			if(Random.Range(0f, 1f) < uniqueChance) {
				int positionToInsert = Random.Range(0, buffer.Count + 1);
				buffer.Insert(positionToInsert, u);

				i++;
			}
		}

		// insert set of templates multiple times
		do {
			foreach (TerrainTemplate t in selection)
			{
				int positionToInsert = Random.Range(0, buffer.Count + 1);
				buffer.Insert(positionToInsert, t);

				i++;
			}
		} while (i < number);

		// remove excess
		while(i > number) {
			buffer.RemoveAt(0);
			i--;
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
}
