using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skin {
		
	public AnimatorOverrideController altPlayer;
	public Sprite altHat;

	public AlternateColor[] altColors;
	
}

[System.Serializable]
public class AlternateColor {

	public ColorSwap[] hatPalette;
	public ColorSwap[] playerPalette;

}

public class CharacterColorChanger : MonoBehaviour {

	public int skindex = 0;
	public int colorIndex = 0;

	public DynamicPalette hatPalette;
	public DynamicPalette playerPalette;

	public Animator currentBodySkin;
	public SpriteRenderer currentHatSkin;

	public Skin[] alternateSkins;

	public void ChangeSkin () {

		currentHatSkin.sprite = alternateSkins[skindex].altHat;
		currentBodySkin.runtimeAnimatorController = alternateSkins[skindex].altPlayer;

		ChangeItemPalette (hatPalette, alternateSkins[skindex].altColors[colorIndex].hatPalette);
		ChangeItemPalette (playerPalette, alternateSkins[skindex].altColors[colorIndex].playerPalette);

	}

	public void ChangeItemPalette (DynamicPalette item, ColorSwap[] palette) {

		item.colorsToSwap = palette;
		item.ChangeColor ();

	}

	public void Init (int _skindex, int _colorIndex) {

		//print  (_skindex + " || " + _colorIndex);

		skindex = _skindex;
		colorIndex = _colorIndex;

		hatPalette.Init ();
		playerPalette.Init ();

		ChangeSkin ();

	}


}
