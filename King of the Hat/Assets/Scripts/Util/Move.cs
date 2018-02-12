using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour {

	public Vector2 velocity;

	void Awake () {

		velocity *= Time.fixedDeltaTime;

	}
	
	// Update is called once per frame
	void FixedUpdate () {

		transform.Translate (velocity.x, velocity.y, 0);

	}

}
