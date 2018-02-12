using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SpriteRenderer))]
public class QuadSpriteOutline : MonoBehaviour {

	public Color outlineColor;
	public int spriteSize = 32;

	SpriteRenderer characterSprite;
	SpriteRenderer[] outline;

	// Use this for initialization
	void Start () {

		characterSprite = GetComponent <SpriteRenderer> ();

		outline = new SpriteRenderer[4];

		for (int i = 0; i < outline.Length; i++) {

			GameObject newGO = new GameObject ();

			outline[i] = newGO.AddComponent <SpriteRenderer> ();
			outline[i].material = Resources.Load <Material> ("Materials/Outline");
			outline[i].color = outlineColor;
			outline[i].transform.SetParent (transform, false);

			switch (i) {

			case 0:
				newGO.name = "Outline Top";
				newGO.transform.position += new Vector3 (0.0f, 1.0f / spriteSize, 0.0f);
				break;

			case 1:
				newGO.name = "Outline Bottom";
				newGO.transform.position += new Vector3 (0.0f, -1.0f / spriteSize, 0.0f);
				break;

			case 2:
				newGO.name = "Outline Left";
				newGO.transform.position += new Vector3 (-1.0f / spriteSize, 0.0f, 0.0f);
				break;

			case 3:
				newGO.name = "Outline Right";
				newGO.transform.position += new Vector3 (1.0f / spriteSize, 0.0f, 0.0f);
				break;

			default:
				break;



			}

		}

	}
	
	// Update is called once per frame
	void LateUpdate () {

		for (int i = 0; i < outline.Length; i++) {

			outline[i].sprite = characterSprite.sprite;
			outline[i].flipX = characterSprite.flipX;
			outline[i].flipY = characterSprite.flipY;
			
		}
			
	}

}
