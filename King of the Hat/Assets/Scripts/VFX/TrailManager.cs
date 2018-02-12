using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (TrailRenderer))]
public class TrailManager : MonoBehaviour {

	public SpriteRenderer spriteRenderer;
	TrailRenderer trail;
	Material mat;

	void Awake () {
		trail = GetComponent <TrailRenderer> ();
		mat = trail.material;
	}

	// Update is called once per frame
	void Update () {

	}

}
