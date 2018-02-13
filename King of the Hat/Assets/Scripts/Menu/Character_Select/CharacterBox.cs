using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CharacterBox : MonoBehaviour {

	//reference to the character select screen
	public CharacterSelect selectScreen;
	public int playerNumber = 0;
    public int teamNumber = 0;

	//variables used for displaying the character preview
	public Image characterPreview;
    public Animator characterAnimation;
    public Text characterName;
    public Animator characterNameAnim;
    public Text teamText;
    public Image banner;
    int index = 0;
    int skinIndex = 0;
    int colorIndex = 0;
	public int characterIndex {get{return index;}}

	public GameObject selectionVFX;
    public Text joinText;
	[HideInInspector]
	public bool selectionComplete;

    public GameObject teamMenu;
    bool isPickingTeam = false;

	[HideInInspector]
	public InputController input;

    //scroll rate
    public float scrollRate = 0.2f;
    float scrollTimer;

	void Awake () {
        scrollTimer = Time.time;
	}

    void OnEnable() {
        UpdateBannerColor();
    }

	void Update () {

		if (input == null) {
            return;
        }

        if (input.snapshot.HorizontalPressed () && !isPickingTeam) {
			if (!selectionComplete) {
                if(Time.time > scrollTimer) {
                    if (input.snapshot.xAxis > 0) ShowNextCharacter(); else ShowLastCharacter();
                    scrollTimer = scrollRate + Time.time;
                }
			}
		}

        if (input.snapshot.VerticalPressed() && !isPickingTeam) {
            if (!selectionComplete) {
                if (Time.time > scrollTimer) {
                    if (input.snapshot.yAxis > 0) ShowNextColor(); else ShowPreviousColor();
                    scrollTimer = scrollRate + Time.time;
                }
            }
        }

        if (input.snapshot.aButton.down) {

            switch(GameController.instance.game.currentGameMode) {

                case Game.Mode.LASTHAT:
                    if (!selectionComplete) {
                        SelectCharacter();
                        PlayerIsReady(true);
                    }

                    break;

                case Game.Mode.TEAMS:
                    
                    if(!selectionComplete && !isPickingTeam) {
                        SelectCharacter();
                        ToggleTeamMenu(true);
                        
                    } else {
                        PlayerIsReady(true);
                        ToggleTeamMenu(false);
                    }

                    break;

                default:
                    if (!selectionComplete) {
                        SelectCharacter();
                        PlayerIsReady(true);
                    }

                    break;
            }

			
		}

		if (input.snapshot.bButton.down) {

            switch (GameController.instance.game.currentGameMode) {

                case Game.Mode.LASTHAT:
                    if (selectionComplete) {
                        DeselectCharacter();
                        PlayerIsReady(false);
                    } else {
                        LeaveGame();
                        return;
                    }

                    break;

                case Game.Mode.TEAMS:

                    if(isPickingTeam) {
                        DeselectCharacter();
                        ToggleTeamMenu(false);
                        teamText.enabled = false;
                        return;
                    }

                    if(selectionComplete) {

                        ToggleTeamMenu(true);
                        PlayerIsReady(false);
                        teamText.enabled = false;

                    } else {
                        LeaveGame();
                        return;
                    }

                    break;

                default:
                    if (selectionComplete) {
                        DeselectCharacter();
                        PlayerIsReady(false);
                    } else {
                        LeaveGame();
                        return;
                    }

                    break;
            }

        }
	}

	public void JoinGame (InputController _input) {
        banner.GetComponent<Animator>().SetTrigger("Join");
    
        //show the character preview
        joinText.enabled = false;

		characterPreview.transform.parent.gameObject.SetActive (true);
		input = _input;
		input.snapshot.isAddedToGame = true;

        ShowCharacterPreview();

        GameController.instance.UpdateCharacterSelections (playerNumber, index, 0, 0);

    }

    public void LeaveGame () {
        banner.GetComponent<Animator>().SetTrigger("Leave");

        joinText.enabled = true;
        PlayerIsReady(false);

        //hide the character preview
        characterPreview.transform.parent.gameObject.SetActive (false);
		input.snapshot.isAddedToGame = false;
		input = null;

		GameController.instance.UpdateCharacterSelections (playerNumber, -1, 0, 0);

	}

	public void ShowNextCharacter () {

		//increment index while wrapping to the array length
		index = (++index < selectScreen.characters.Length) ? index : 0;
        colorIndex = 0;

        GameController.instance.UpdateCharacterSelections (playerNumber, index, skinIndex, 0);

        ShowCharacterPreview();

    }

    public void ShowLastCharacter () {
		
		//increment index while wrapping to the array length
		index = (--index >= 0) ? index : selectScreen.characters.Length - 1;
        colorIndex = 0;

        GameController.instance.UpdateCharacterSelections (playerNumber, index, skinIndex, 0);

        ShowCharacterPreview();

    }

    public void ShowNextColor() {

        colorIndex = (++colorIndex < selectScreen.characters[index].characterColors.alternateSkins[skinIndex].altColors.Length) ? colorIndex : 0;

        GameController.instance.UpdateCharacterSelections(playerNumber, index, skinIndex, colorIndex);

    }

    public void ShowPreviousColor() {

        colorIndex = (--colorIndex >= 0) ? colorIndex : selectScreen.characters[index].characterColors.alternateSkins[skinIndex].altColors.Length - 1;

        GameController.instance.UpdateCharacterSelections(playerNumber, index, skinIndex, colorIndex);

    }

    public void SelectCharacter () {
        if (selectScreen.characters[index].locked)
            return;

		GameController.instance.game.SetPlayer (playerNumber - 1, selectScreen.characters[index].characterPrefab, input, selectScreen.characters[index].emblem, selectScreen.characters[index].groupID, teamNumber, selectScreen.characters[index].preview);
        UpdateBannerColor();
    }

    public void PlayerIsReady(bool ready) {
        selectionVFX.SetActive(ready);
        selectionComplete = ready;

        if(selectionComplete) {
            characterAnimation.SetTrigger("Ready");
            characterNameAnim.SetTrigger("Select Character");
            characterNameAnim.ResetTrigger("Deselect Character");
        } else {
            characterAnimation.SetTrigger("Not Ready");
            characterNameAnim.SetTrigger("Deselect Character");
        }

    }

	public void DeselectCharacter () {
		GameController.instance.game.SetPlayerToNull (playerNumber - 1);
	}

    void ShowCharacterPreview() {
        
        if(selectScreen.characters[index].locked) {
            characterName.text = "Locked";
            characterPreview.sprite = selectScreen.characters[index].previewLocked;
            characterPreview.SetNativeSize();
            characterPreview.rectTransform.sizeDelta *= 4;
        } else {
            characterName.text = selectScreen.characters[index].groupID;
            characterPreview.sprite = selectScreen.characters[index].preview;
            characterPreview.SetNativeSize();
            characterPreview.rectTransform.sizeDelta *= 4;
            characterAnimation.runtimeAnimatorController = selectScreen.characters[index].animController;
        }

        characterNameAnim.SetTrigger("Enter");

    }

    public void ToggleTeamMenu(bool active) {

        isPickingTeam = active;

        teamMenu.SetActive(active);

    }

    public void UpdateTeamTextAndColor() {

        teamText.enabled = true;

        string teamName;

        switch(teamNumber) {
            case 1: teamName = "Blue Team";
                teamText.color = Color.blue;
                break;
            case 2: teamName = "Red Team";
                teamText.color = Color.red;
                break;
            case 3: teamName = "Green Team";
                teamText.color = Color.green;
                break;
            case 4: teamName = "Yellow Team";
                teamText.color = Color.yellow;
                break;
            default: teamName = "Error";
                break;
        }

        teamText.text = teamName;

    }

    public void UpdateBannerColor() {

        banner.sprite = selectScreen.bannerColors[teamNumber - 1];

    }

}