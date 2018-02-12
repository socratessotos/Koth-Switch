using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterCamera : MonoBehaviour {

	[HideInInspector]
	public Camera thisCamera;
	private int downResFactor = 1;
	private Vector3 initialPosition;

	private string globalTextureName = "_GlobalReflectionTex";

	void Start () {

		thisCamera = GetComponent<Camera> ();

		thisCamera.targetTexture = new RenderTexture(thisCamera.pixelWidth >> downResFactor, thisCamera.pixelHeight >> downResFactor, 16);
		thisCamera.targetTexture.filterMode = FilterMode.Bilinear;

		Shader.SetGlobalTexture (globalTextureName, thisCamera.targetTexture);

		transform.position = new Vector3 (Camera.main.transform.position.x, transform.position.y, Camera.main.transform.position.z);
		initialPosition = transform.position;

//		Matrix4x4 mat = thisCamera.projectionMatrix;
//		mat *= Matrix4x4.Scale(new Vector3(1, -1, 1));
//		thisCamera.projectionMatrix = mat;

	}

	void Update () {

		transform.position = new Vector3 (Camera.main.transform.position.x, Mathf.Floor((initialPosition.y - (Camera.main.transform.position.z - initialPosition.z) / 2) * 100) / 100, Camera.main.transform.position.z);

	}

}
