using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

[System.Serializable]
public class PlayableCharacter {
	
	public string groupID;
    public string fightStyle;
    public Sprite preview;
    public Sprite previewLocked;
    public RuntimeAnimatorController animController;
	public Sprite emblem;
	public PlayerInput characterPrefab;
    public CharacterColorChanger characterColors;
    public bool locked;

}

public class CharacterSelect : MonoBehaviour {

	public CharacterBox[] characterBoxes;
    public Sprite[] bannerColors;
	public PlayableCharacter[] characters;

    [Header("References")]
    public GameObject startButtonVFX;
    public Text title;
    public Text titleMask;

    void Awake() {
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
    }

    void OnEnable() {

        SetTitle();
        ResetReadyState();

    }

    void SetTitle() {

        string _currentGameMode = GameController.instance.game.GameModeToString(GameController.instance.game.currentGameMode);

        title.text = _currentGameMode;
        titleMask.text = _currentGameMode;

    }

	void Update () {

        CheckIfAnyPlayerHasDisconnectedController();

        for (int i = 0, l = GameController.instance.playerInputs.Length; i < l; i++) {

			if (!GameController.instance.playerInputs[i].snapshot.isAddedToGame && GameController.instance.playerInputs[i].snapshot.aButton.down) {
				RequestToJoinGame (GameController.instance.playerInputs[i], i);
			} else if (!GameController.instance.playerInputs[i].snapshot.isAddedToGame && GameController.instance.playerInputs[i].snapshot.bButton.down) {
				GameController.instance.menuNav.GoToScreen ("Channels");
			}
            
            if (GameController.instance.playerInputs[i].GetController() == null) {
                characterBoxes[i].LeaveGame();
            }

        }

		CheckIfSelectionIsComplete ();
	
	}

	void RequestToJoinGame (InputController input, int controllerIndex) {

        if (characterBoxes[controllerIndex].input == null) {
            characterBoxes[controllerIndex].JoinGame(input);
            input.snapshot.aButton.down = false;
        }

        /*
        for (int i = 0; i < characterBoxes.Length; i++) {

			if (characterBoxes[i].input == null) {
				characterBoxes[i].JoinGame (input);
				input.snapshot.aButton.down = false;
				break;
			}

		}
		*/
	}

	void CheckIfSelectionIsComplete () {
		int activePlayers = 0;
		int completePlayers = 0;

		for (int i = 0; i < characterBoxes.Length; i++) {
			if (characterBoxes[i].input != null) activePlayers++;
			if (characterBoxes[i].selectionComplete) completePlayers++;
		}

        bool onlyOneTeam = true;
        int lastTeamNumberCheck = 0;

        for (int i = 0; i < characterBoxes.Length; i++) {
            if (characterBoxes[i].input == null) continue;

            if(GameController.instance.game.currentGameMode == Game.Mode.LASTHAT) {
                onlyOneTeam = false;
                break;
            }

            if (i == 0) lastTeamNumberCheck = characterBoxes[i].teamNumber;
            else if (characterBoxes[i].teamNumber != lastTeamNumberCheck)
                onlyOneTeam = false;

        }

        if(onlyOneTeam) {
            return;
        }

		if (completePlayers >= 1 && activePlayers == completePlayers) {

			for (int i = 0, l = GameController.instance.playerInputs.Length; i < l; i++) {
				if (GameController.instance.playerInputs[i].snapshot.isAddedToGame && (GameController.instance.playerInputs[i].snapshot.startButton.down || GameController.instance.playerInputs[i].snapshot.aButton.down)) {
                    GameController.instance.menuNav.GoToScreen ("Level_Select");
				}
			}

			startButtonVFX.SetActive (true);
		} else {
			startButtonVFX.SetActive (false);
		}
	}
    
    void ResetReadyState() {
        for (int i = 0; i < characterBoxes.Length; i++) {
            startButtonVFX.SetActive(false);
            characterBoxes[i].PlayerIsReady(false);
            characterBoxes[i].DeselectCharacter();
        }
    }

    void OnControllerDisconnected(ControllerStatusChangedEventArgs args) {

        if (GameController.instance.playerInputs[args.controllerId].snapshot.isAddedToGame)
            GameController.instance.PlayerHasDisconnectedController(args.controllerId);        
            
    }

    void CheckIfAnyPlayerHasDisconnectedController() {
        for (int i = 0; i < characterBoxes.Length; i++) {
            if (GameController.instance.ControllerHasBeenDisconnected(i, false))
                characterBoxes[i].LeaveGame();
        }
    }

}
