using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteAbberation : MonoBehaviour {

	public string sortingLayer;
	public float angle;
	public float distance;

    public Color[] colors;

	public float lerpScale {
		get {
			return realLerpScale;
		}

		set {
			realLerpScale = Mathf.Clamp01(value);
			changeDistance ();
		}
	}

	private SpriteRenderer render;
	private SpriteRenderer[] children;
	private const int SIZE = 2;
	private float realLerpScale;

	void Awake () {
		render = GetComponent<SpriteRenderer> ();
	}

	public void Abberate () {
		fillArray ();
	}

	public void clearAbberations () {
		if (children == null)
			return;

		realLerpScale = 0;
		for (int i = 0; i < SIZE; i++)
			Destroy (children [i].gameObject);
		children = null;
	}

	private void changeDistance () {
		if (children == null)
			return;

		float dist = Mathf.Lerp (0, distance, realLerpScale);

		if (dist == 0) {
			for (int i = 0; i < SIZE; i++)
				children [i].gameObject.transform.localPosition = Vector3.zero;
			return;
		}

		float ang = angle * Mathf.Deg2Rad;

		Vector3 temp = children [0].gameObject.transform.localPosition;
		children [0].gameObject.transform.localPosition = new Vector3 (dist * Mathf.Cos(ang), dist * Mathf.Sin(ang), temp.z);

		temp = children [1].gameObject.transform.localPosition;
		children [1].gameObject.transform.localPosition = new Vector3 (dist * Mathf.Cos(ang + Mathf.PI), dist * Mathf.Sin(ang + Mathf.PI), temp.z);
	}

	private void fillArray () {
		if (children == null) {
			children = new SpriteRenderer[SIZE];

			for (int i = 0; i < SIZE; i++) {
				GameObject g = new GameObject (gameObject.name + "_CURSED_ABBERATION_" + i);

				children [i] = g.AddComponent<SpriteRenderer> ();
				children [i].sprite = render.sprite;
				children [i].color = render.color;
				children [i].flipX = render.flipX;
				children [i].flipY = render.flipY;
				children [i].sharedMaterial = render.sharedMaterial;
				children [i].sortingLayerID = SortingLayer.NameToID (sortingLayer);
				children [i].sortingOrder =  -i;
				children [i].enabled = true;

                g.transform.parent = transform;
                g.transform.position = transform.position;
				g.transform.localScale = transform.localScale;

                children[i].material.SetColor("_OverrideColor", colors[i]);

            }
        }
	}
}
