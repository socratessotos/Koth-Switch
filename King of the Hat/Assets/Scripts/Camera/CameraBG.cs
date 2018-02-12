using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBG : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		transform.localScale = Vector3.one * (Camera.main.orthographicSize/4);
	}
}
