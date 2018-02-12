using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterObject : MonoBehaviour {

	public const string HEIGHT = "_Height";
	public const string DIVIDER = "_Divider";
	public const string IMAGE_MIN = "_ImgMin";

	private MeshRenderer mesh;
	private WaterCamera wCamera;

	void Awake () {
		mesh = GetComponent<MeshRenderer> ();

		wCamera = FindObjectOfType<WaterCamera> ();
	}

	void Update () {
		//Camera.main.pixelHeight;
		float y1 = wCamera.thisCamera.WorldToScreenPoint (new Vector3(0, mesh.bounds.min.y, 0)).y;
		float y2 = wCamera.thisCamera.WorldToScreenPoint (new Vector3(0, mesh.bounds.max.y, 0)).y;

		float height = y2 - y1;
		float min = (wCamera.thisCamera.pixelHeight - y2) / wCamera.thisCamera.pixelHeight;

		float scaledHeight = height / wCamera.thisCamera.pixelHeight;
		mesh.material.SetFloat (HEIGHT, scaledHeight);
		//mesh.material.SetFloat (DIVIDER, 1 / (scaledHeight + min));
		//image min is distance from top of the image to the top of the screen!!!!!!!!
		//mesh.material.SetFloat (IMAGE_MIN, min);
	}
}
