using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxObject : MonoBehaviour {

	//the layer for determining how close the object is to the camera
	public ParallaxLayer parallaxLayer;

	//how fast the object will move, based on its layer
	//this value is set in the parallax manager
	float speed;
	Vector2 initialPosition;

	// Use this for initialization
	void Awake () {
		speed = ParallaxManager.ParallaxLayerToMoveSpeed (parallaxLayer);
		initialPosition = new Vector2 (transform.position.x, transform.position.y);
	}
	
	// Update is called once per frame
	void Update () {

		float newX = initialPosition.x - (Camera.main.transform.position.x * speed);
		//float newY = initialPosition.y - (Camera.main.transform.position.y * speed);
		float newY = initialPosition.y;

		transform.position = new Vector3 (newX, newY, transform.position.z);
		
	}
}
