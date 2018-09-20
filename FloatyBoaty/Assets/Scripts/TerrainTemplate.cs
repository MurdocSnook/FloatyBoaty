using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyButtons;

[ExecuteInEditMode]
public class TerrainTemplate : MonoBehaviour {
	public GameObject entranceMarker;
	public GameObject exitMarker;

	[Header("Water current curve parameters")]
	public float smoothness = 1f;
	public int iterationSteps = 3;

    private List<Vector3> curve;
    public List<Vector3> Curve
    {
        get
        {
            return curve;
        }
    }

	[Header("Spawning")]
	public RiverEvent[] possibleEvents;

    // Use this for initialization
    void Start () {
		CalulateCurve();
	}

	// Update is called once per frame
	void Update () {
		#if UNITY_EDITOR
			CalulateCurve();
		#endif
	}

	void CalulateCurve() {
		// use subdivisions to smooth out the curve
		curve = new List<Vector3>();
		float offset = smoothness * 0.333f * Vector3.Distance(entranceMarker.transform.position, exitMarker.transform.position);
		// first 4 points, the two middle points change depending on the smoothness
		curve.Add(entranceMarker.transform.position);
		curve.Add(entranceMarker.transform.position + entranceMarker.transform.right * offset);
		curve.Add(exitMarker.transform.position - exitMarker.transform.right * offset);
		curve.Add(exitMarker.transform.position);

		// iteratively smooth, doubling the points each step
		for(int i = 0; i < iterationSteps; i++) {
			List<Vector3> newCurve = new List<Vector3>();

			newCurve.Add(curve[0]);
			for(int j = 0; j < curve.Count - 1; j++) {
				// We add two intermediary points per segment, but don't re-add the existing points
				// We leave out a point for the last and first segment, because they are not necesessary there
				if(j != 0)
					newCurve.Add(Vector3.Lerp(curve[j], curve[j+1], 0.25f));
				if(j != curve.Count - 2)
					newCurve.Add(Vector3.Lerp(curve[j], curve[j+1], 0.75f));
			}
			newCurve.Add(curve[curve.Count - 1]);
			curve = newCurve;
		}
	}

	public void SpawnEvent() {
		if(possibleEvents.Length == 0) {
			return;
		}

		GameObject prefab = possibleEvents[Random.Range(0, possibleEvents.Length)].gameObject;
		GameObject instance = Instantiate(prefab, entranceMarker.transform.position, entranceMarker.transform.rotation);
		instance.GetComponent<RiverEvent>().terrainTemplate = this;
	}

	private void OnDrawGizmos() {
		if(curve != null) {
			foreach (Vector3 pos in curve) {
				Gizmos.DrawSphere(pos, 1);
			}
		}
	}

	private void OnValidate() {
		if (entranceMarker == null || exitMarker == null) {
			Debug.LogError("Entrance and exit markers must be assigned.", this);
		}
	}
}
