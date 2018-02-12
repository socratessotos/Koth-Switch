using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (SpriteRenderer))]
public class DynamicPalette : MonoBehaviour {

	public ColorSwap[] colorsToSwap;

	Texture2D colorSwapTex;
	Color[] spriteColors;
	SpriteRenderer spriteRenderer;

	public void Init (){
		spriteRenderer = GetComponent <SpriteRenderer> ();
	}

	public void InitColorSwapTex()
	{
		Texture2D _colorSwapTex = new Texture2D(256, 1, TextureFormat.RGBA32, false, false);
		_colorSwapTex.filterMode = FilterMode.Point;

		for (int i = 0; i < _colorSwapTex.width; ++i)
			_colorSwapTex.SetPixel(i, 0, new Color(0.0f, 0.0f, 0.0f, 0.0f));

		_colorSwapTex.Apply();

		spriteRenderer.material.SetTexture("_SwapTex", _colorSwapTex);

		spriteColors = new Color[_colorSwapTex.width];
		colorSwapTex = _colorSwapTex;
	}

	public void ResetColors () {

		for (int i = 0; i < colorSwapTex.width; ++i)
			colorSwapTex.SetPixel(i, 0, new Color(0.0f, 0.0f, 0.0f, 0.0f));

		colorSwapTex.Apply();
		
	}

	public void SwapPalette () {

		spriteColors = new Color[colorSwapTex.width];

		for (int i = 0; i < colorsToSwap.Length; i++) {
			spriteColors[colorsToSwap[i].rValue] = colorsToSwap[i].swapColor;
		}

		for (int i = 0; i < colorSwapTex.width; ++i)
			colorSwapTex.SetPixel(i, 0, spriteColors[i]);

		colorSwapTex.Apply();

	}

	public void ChangeColor () {

		InitColorSwapTex ();
		SwapPalette ();

	}

}