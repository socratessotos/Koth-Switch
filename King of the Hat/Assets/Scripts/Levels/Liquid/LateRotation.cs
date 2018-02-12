using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateRotation : MonoBehaviour {

	public Vector3 eulerAngles;

	// Use this for initialization
	void Start () {
		transform.rotation = Quaternion.Euler (eulerAngles);
	}
	

}
