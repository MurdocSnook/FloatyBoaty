using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnparentOnStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.SetParent(null, true);	
	}
}
