using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Renderer))]
public class AnimatedSpriteSheet : MonoBehaviour {

	public Vector2 numberOfFrames = Vector2.one;
	public float frameLength = 0.1f;

	Vector2 frameSize;
	Vector2 currentFrame;
	Material mat;

	void Start () {

		//StartAnimating ();

	}
	
	// Use this for initialization
	public void StartAnimating () {
		
		frameSize = new Vector2 (1 / numberOfFrames.x, 1 / numberOfFrames.y);
		mat = GetComponent <Renderer> ().material;
		mat.mainTextureScale = frameSize;

		SetFrame ();
		InvokeRepeating ("PlayNextFrame", 0f, frameLength);

	}

	void SetFrame () {
		SetFrame (Vector2.zero);
	}

	void SetFrame (Vector2 currentFrame) {
		mat.mainTextureOffset = new Vector2 (currentFrame.x * frameSize.x, currentFrame.y * frameSize.y);
	}

	void PlayNextFrame () {

		currentFrame.x++;
		if (currentFrame.x >= numberOfFrames.x) {

			currentFrame.x = 0;
			currentFrame.y++;

			if (currentFrame.y >= numberOfFrames.y) {
				currentFrame = Vector2.zero;
			}

		}

		SetFrame (currentFrame);
		
	}


}
