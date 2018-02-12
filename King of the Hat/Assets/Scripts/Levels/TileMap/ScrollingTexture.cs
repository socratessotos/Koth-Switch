using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent (typeof (Renderer))]
public class ScrollingTexture : MonoBehaviour {

	public Vector2 scrollVelocity;

	Renderer rend;
	Material mat;
    Vector2 initialOffset;

	// Use this for initialization
	void Start () {
		rend = GetComponent <Renderer> ();
		mat = rend.material;
        initialOffset = mat.mainTextureOffset;
	}

	void Update () {

		ScrollTexture ();

	}
		
	void ScrollTexture () {
		mat.mainTextureOffset = Time.time * scrollVelocity + initialOffset;
	}

}