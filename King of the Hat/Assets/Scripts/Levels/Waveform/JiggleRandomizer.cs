using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MeshJigglifier))]
public class JiggleRandomizer : MonoBehaviour {

	public float timeBetweenJiggles;
	public float baseForce;
	public float forceVariance;
	public float jiggleRadius;

	private MeshJigglifier mj;
	private float nextJiggle;

	// Use this for initialization
	void Start () {

		mj = GetComponent <MeshJigglifier> ();

	}
	
	// Update is called once per frame
	void Update () {


		if (Time.time > nextJiggle) {

			mj.Splash (mj.GetRandomVertex (), Vector3.one * (baseForce + Random.Range (0, forceVariance)), jiggleRadius);
			nextJiggle += timeBetweenJiggles;

		}
		
	}
}
