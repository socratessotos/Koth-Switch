using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (ParticleSystem))]
public class RemoveParticleSystem : MonoBehaviour {

	ParticleSystem ps;

	void Awake () {
		ps =  GetComponent<ParticleSystem> ();
	}

	// Update is called once per frame
	void LateUpdate () {

		if (ps.particleCount == 0)
			Destroy (gameObject);

	}

}
