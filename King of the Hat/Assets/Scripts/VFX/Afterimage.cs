using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Afterimage : MonoBehaviour {

	public bool startEnabled;

	//Color, distance from start, time on screen
	[Header ("Afterimage Settings")]
	public string sortingLayer;
	public Color startColor;
	public Color fadeToColor;
	public float lifeTime;
	[Range (1, 10)]
	public int count;

	public Material mat;

	private SpriteRenderer render;
	private SpriteRenderer[] children;

	private bool makeAfterImages;

	// Vars to make afterimages
	private float currentTime;
	private float frameTime;
	private List<int> needUpdate = new List<int> ();

	void Awake () {
		render = GetComponent<SpriteRenderer> ();

		if (startEnabled)
			TurnOn ();

		frameTime = 1.0f / count;
	}

	void OnEnable () {

		TurnOn ();

		for (int i = 0; i < count; i++) {

			children[i].gameObject.SetActive (true);

		}
		
	}


	void OnDisable () {

		for (int i = 0; i < count; i++) {

			children[i].gameObject.SetActive (false);

		}

	}

	void Update () {
		//DELETE
		transform.Translate (new Vector2 (3, 0) * Time.deltaTime);
		if (Input.GetKeyDown (KeyCode.A))
			Toggle ();

		if (needUpdate.Count == 0)
			return;

		if (currentTime >= lifeTime) {
			currentTime = 0;

			for (int i = count - 1; i >= 0; i--) {
				if (!needUpdate.Contains (i))
					continue;

				if (i != 0) {
					SetChild (i, children [i - 1]);

					children [i].enabled = children[i - 1].enabled;
				} else {
					children [i].enabled = makeAfterImages;
					SetChild (i, render);
				}

				if (children [i].enabled) {
					needUpdate.Add (i);
					if (i + 1 < count)
						needUpdate.Add (i + 1);
				}
			}

		} else
			currentTime += Time.deltaTime;

		for (int i = 0; i < count; i++) {
			if (children[i].enabled)
				children [i].color = Color.Lerp (startColor, fadeToColor, i / (float) count + frameTime * (currentTime / lifeTime));
		}
	}

	private void FillArray () {
		if (children == null) {
			children = new SpriteRenderer[count];
			
			for (int i = 0; i < count; i++) {
				GameObject g = new GameObject (gameObject.name + "_afterimage_" + i);

				children [i] = g.AddComponent<SpriteRenderer> ();
				children [i].sprite = render.sprite; //*
				children [i].color = render.color; //*
				children [i].flipX = render.flipX; //*
				children [i].flipY = render.flipY; //*
				children [i].sharedMaterial = mat;
				children [i].sortingLayerID = SortingLayer.NameToID (sortingLayer);
				children [i].sortingOrder = count - i;
				children [i].enabled = false;

				//g.transform.position = transform.position; //* and rotation
				g.transform.localScale = Vector3.one * 1.6f;

				//g.transform.parent = transform;
			}
		}
	}
		
	private void SetChild (int pos, SpriteRenderer r) {
		if (pos < 0 || pos >= count)
			return;

		children [pos].sprite = r.sprite;
		children [pos].color = r.color;
		children [pos].flipX = r.flipX;
		children [pos].flipY = r.flipY;

		children [pos].transform.position = r.transform.position;
		//Debug.Log (children [pos].transform.position + " " + r.transform.position);
		children [pos].transform.rotation = r.transform.rotation;

	}

	public void TurnOn () {
		makeAfterImages = true;
		FillArray ();
		needUpdate.Add (0);
	}

	public void TurnOff () {
		makeAfterImages = false;
	}

	public void Toggle () {
		if (makeAfterImages)
			TurnOff ();
		else
			TurnOn ();
	}

	void OnDestroy () {

		for (int i = 0; i < count; i++) {

			Destroy (children [i].gameObject);

		}

	}

}
