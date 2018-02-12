using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (ParticleSystem))]
public class OptimizeParticleSystem : MonoBehaviour {

	ParticleSystem ps;

	void Awake () {
		ps =  GetComponent<ParticleSystem> ();
	}

	// Update is called once per frame
	void Update () {

		if (ps.particleCount == 0)
			gameObject.SetActive (false);

	}

}
