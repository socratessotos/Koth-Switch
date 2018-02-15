using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Skin {
		
	public AnimatorOverrideController altPlayer;
	public Sprite altHat;

    public int blueTeamColorIndex = 0;
    public int redTeamColorIndex = 1;

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

        MatchColorToTeam();

		ChangeItemPalette (hatPalette, alternateSkins[skindex].altColors[colorIndex].hatPalette);
		ChangeItemPalette (playerPalette, alternateSkins[skindex].altColors[colorIndex].playerPalette);

	}
    int oldColorIndex = 0;
    void MatchColorToTeam() {

        if (GameController.instance.game.currentGameMode != Game.Mode.TEAMS) return;

        oldColorIndex = colorIndex;

        if (GetComponent<Player>().teamNumber == 1) {
            colorIndex = alternateSkins[skindex].blueTeamColorIndex;
        } else { 
            colorIndex = alternateSkins[skindex].redTeamColorIndex;
        }

    }

	public void ChangeItemPalette (DynamicPalette item, ColorSwap[] palette) {
        
        item.colorsToSwap = palette;

        //if teams, check oldpalette and shift
        //foreach color in item.colorsToSwap, color shift down amount
        //careful that this doesnt overwrite, you may need to create a new array and transfer instead of dierectly changing
        if (GameController.instance.game.currentGameMode == Game.Mode.TEAMS) {

            foreach(ColorSwap c in item.colorsToSwap) {
                c.swapColor.r -= (0.2f * oldColorIndex);
                c.swapColor.g -= (0.2f * oldColorIndex);
                c.swapColor.b -= (0.2f * oldColorIndex);

            }

        }


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

    void Update() {

        if(Input.GetKeyDown(KeyCode.O)) {
            colorIndex++;
            if(colorIndex > alternateSkins[skindex].altColors.Length - 1) {
                colorIndex = 0;
            }

            oldColorIndex = 0;
            ChangeSkin();
        }    

    }

}
