using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BirdColorScheme {

	public string groupID;
	public Color32 mainColor;
	public Color32 bellyColor;
	public Color32 outlineColor;
	public Color32 hatColor;

	public BirdColorScheme (Color32 _mainColor, Color32 _bellyColor, Color32 _outlineColor, Color32 _hatColor) {

		mainColor = _mainColor;
		bellyColor = _bellyColor;
		outlineColor = _outlineColor;
		hatColor = _hatColor;
		
	}

}


[RequireComponent (typeof (DynamicPalette))]
public class ColorRandomizer : MonoBehaviour {

	public BirdColorScheme[] possibleColorSchemes;

	private DynamicPalette dynamicPalette;

	// Use this for initialization
	public void Start () {

		dynamicPalette = GetComponent <DynamicPalette> ();
		dynamicPalette.Init ();

		BirdColorScheme chosenPalette = possibleColorSchemes [Random.Range (0, possibleColorSchemes.Length)];
		dynamicPalette.colorsToSwap [0].swapColor = chosenPalette.mainColor;
		dynamicPalette.colorsToSwap [1].swapColor = chosenPalette.bellyColor;
		dynamicPalette.colorsToSwap [2].swapColor = chosenPalette.outlineColor;
		dynamicPalette.colorsToSwap [3].swapColor = chosenPalette.hatColor;

		dynamicPalette.ChangeColor ();
			

	}

}
