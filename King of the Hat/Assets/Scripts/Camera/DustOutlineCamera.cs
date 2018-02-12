using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class DustOutlineCamera : MonoBehaviour {

	public Color outlineColor = Color.white;
	public Material effectMaterial;

	private const string outlineColorStr = "_OutlineColor";

	void Start () {
		effectMaterial.SetColor (outlineColorStr, outlineColor);
	}

	void OnRenderImage(RenderTexture src, RenderTexture dst) {
		Graphics.Blit (src, dst, effectMaterial);
	}
}
