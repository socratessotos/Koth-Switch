using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[System.Serializable]
public class InGamePlayerDisplay {

	public string groupID;
	public Image emblem;
    public Text scoreText;
    public Animator anim;

	public int characterIndex = -1;

	public int skinIndex = 0;
	public int colorIndex = 0;

    public int teamIndex = -1;
}

public class GameController : MonoBehaviour {

    [Header("Dev Variables")]
    public bool countdownIsEnabled;
    public float gameTimeScale = 0.93f;
    public bool devToolsEnabled = false;
    public GameObject devToolPanel;
    bool canOpenOrCloseDevTools = true;
    [Space(5)]

	public static GameController instance;

	public Game game;
	public InputController[] playerInputs;
	public InGamePlayerDisplay[] playerDisplays;
	public Menu menuNav;
    public Menu gameCanvas;
    public GameModeUI gameModeUI;
    public GameObject pauseMenu;
    public WinScreen winScreenMenu;

    [Header ("Game Over")]
    public Text gameOverText;
    public Animator gameOverTextAnim;
    [Space(5)]

    [Header ("Timer")]
    public Text gameTimer;
    [Space(5)]

    [Header ("Countdown")]
    public Text countdownText;
    public Animator countdownAnim;
    [Space(5)]

    [Header("Survival Mode")]
    public Animator survivalModeScoreAnim;
    public Text survivalModeScoreText;
    [Space(5)]

    [Header("Teams")]
    public Text[] teamScoreTexts;

    [HideInInspector]
	public SmashCam smashCam;

    bool[] controllerHasDisconnected = new bool[] { false, false, false, false };

	// Use this for initialization
	void Awake () {

		instance = this;

		smashCam = Camera.main.GetComponent <SmashCam> ();
		game = new Game ();

		Camera.main.transparencySortMode = TransparencySortMode.Orthographic;
		Camera.main.opaqueSortMode = UnityEngine.Rendering.OpaqueSortMode.FrontToBack;

		Time.timeScale = gameTimeScale;

    }
	
	// Update is called once per frame
	void Update () {
        
        switch (game.state) {

		case Game.State.SETUP:
			break;

        case Game.State.INTRO:
            break;

        case Game.State.COUNTDOWN:
            HandleCountdownTimer();
            break;

		case Game.State.PLAYING:

            if(game.currentGameMode == Game.Mode.TIME) {
                HandleTimer();
            }

            if (game.IsOver()) {
                            
                game.state = Game.State.OVER;

                //gameModeUI.EnableUI();

                if (game.CheckIfTheresAWinner() || game.currentGameMode == Game.Mode.SURVIVAL) {
                    Time.timeScale = 0.4f;
                    Invoke("EndGame", 3f);
                } else {
                    Invoke("Reset", 3);                 
                }

            }

			CheckForPause ();
			break;

		case Game.State.PAUSED:

			CheckForPause ();

			break;

		case Game.State.INBETWEEN:
			break;

		case Game.State.REPLAY:
			break;

		case Game.State.OVER:
			break;

		default:
			break;

		}
		
	}

    public void ToggleDevToolsWindow() {
        devToolsEnabled = !devToolsEnabled;
        devToolPanel.SetActive(devToolsEnabled);

        canOpenOrCloseDevTools = false;
    }

    public void SetCanOpenOrCloseDevTools(bool enabled) {
        canOpenOrCloseDevTools = enabled;
    }

	// Update is called once per frame
	void LateUpdate () {

	}

	public void UpdateCharacterSelections (int _playerIndex, int _characterIndex, int _skinIndex, int _desiredColor) {

        List<int> freeColors = new List<int> { 0, 1, 2, 3, 4 };
        
		_playerIndex--;
        
		playerDisplays [_playerIndex].characterIndex = _characterIndex;
		playerDisplays [_playerIndex].skinIndex = _skinIndex;

		int _colorIndex = _desiredColor;

		for (int i = 0; i < playerDisplays.Length; i++) {
			if (i == _playerIndex) continue;

            if (playerDisplays[i].characterIndex == _characterIndex
				&& playerDisplays[i].skinIndex == _skinIndex){
                
                for(int c = 0; c < freeColors.Count; c++) {
                    if(playerDisplays[i].colorIndex == freeColors[c]) {
                        freeColors.RemoveAt(c);
                    }
                }   

            }

        }
        
        _colorIndex = (_desiredColor <= freeColors.Count) ? freeColors[_desiredColor] : freeColors[freeColors.Count];

        //_colorIndex = freeColors[0];
        playerDisplays [_playerIndex].colorIndex = _colorIndex;

	}

    void HandleCountdownTimer() {
        
        int count = (int) ((game.countdownTimer - 1) / 50);
        
        if(count > 0) {
            if(game.countdownTimer % 50 == 0) {
                switch(count) {
                    case 1:
                        countdownText.text = "Camera!";
                        countdownAnim.SetTrigger("Count");
                        break;
                    case 2:
                        countdownText.text = "Lights!";
                        countdownAnim.SetTrigger("Count");
                        break;
                    default:
                        break;
                }
            }   
        } else {
            countdownText.text = "Action!";
            countdownAnim.SetTrigger("Count");

            Invoke("DisableCountdownText", 1);

            game.state = Game.State.PLAYING;
        }

        game.countdownTimer--;

    }

    void DisableCountdownText() {
        countdownText.gameObject.SetActive(false);
    }

    void HandleTimer() {
        game.timeRemaining -= Time.deltaTime;

        if (game.timeRemaining <= 0)
            game.timeRemaining = 0;

        string minutes = Mathf.Floor(game.timeRemaining / 60).ToString();
        string seconds = (game.timeRemaining % 60).ToString("00");

        gameTimer.text = minutes + " : " + seconds;
    }

    public void SetGameOverText(string _text) {
        gameOverText.text = _text;
        gameOverText.gameObject.SetActive(true);

        Invoke("DisableGameOverText", 4);
    }

    void DisableGameOverText() {
        gameOverText.gameObject.SetActive(false);
    }

    public void ResetCharacterSelectScreen() {
        game.state = Game.State.SETUP;
        menuNav.GoToScreen("Character_Select");
    }

    void Reset () {
		game.Reset ();
	}

    void EndGame() {
        Time.timeScale = gameTimeScale;

        DetermineBadges();
        LoadWinScreen();
        
        DestroyLevel();
    }

    void DetermineBadges() {

        switch(game.currentGameMode) {
            case Game.Mode.LASTHAT:
                DetermineLastHatBadges();
                break;
            case Game.Mode.STOCK:
                DetermineStockBadges();
                break;
            case Game.Mode.TIME:
                DetermineTimeBadges();
                break;
            case Game.Mode.SURVIVAL:
                DetermineSurvivalBadges();
                break;
            case Game.Mode.TEAMS:
                DetermineTeamsBadges();
                break;
            default:
                break;
        }

    }

    void DetermineLastHatBadges() {
        //make array for player scores according to their player index
        float[] scores = new float[game.currentPlayers.Length];

        //fill it with # of rounds won, and -1 if the player is not in the game
        for(int i = 0; i < scores.Length; i++) {
            if (game.currentPlayers[i] == null)
                scores[i] = -1;
            else {
                scores[i] = (float) game.currentPlayers[i].numberOfRoundsWon;
            }
        }

        for(int i = 0; i < scores.Length; i++) {
            DetermineRank(i, scores);
        }

    }

    void DetermineTeamsBadges() {

        string winner;
        int _winningTeam = 0;

        if (game.teamScores[0] >= game.roundsPerGame) {
            _winningTeam = 0;
            winner = "Blue Team";
        } else {
            _winningTeam = 1;
            winner = "Red Team";
        }

        for (int i = 0; i < game.currentPlayers.Length; i++) {

            if (game.currentPlayers[i] == null) continue;

            if (game.currentPlayers[i].teamNumber == _winningTeam + 1)
                winScreenMenu.playerBoxes[i].AssignBadge(0);
            else
                winScreenMenu.playerBoxes[i].AssignBadge(1);            

        }

        winScreenMenu.SetWinText(winner);

    }

    void DetermineStockBadges() {
        //make array for player scores according to their player index
        float[] scores = new float[game.currentPlayers.Length];

        //fill it with # of rounds won, and -1 if the player is not in the game
        for (int i = 0; i < scores.Length; i++) {
            if (game.currentPlayers[i] == null)
                scores[i] = 0;
            else if(game.currentPlayers[i].timeOfDeath == 0) {
                scores[i] = Time.time;
            } else {
                scores[i] = game.currentPlayers[i].timeOfDeath;
            }
        }

        for (int i = 0; i < scores.Length; i++) {
            DetermineRank(i, scores);
        }

    }

    void DetermineSurvivalBadges() {

        //make array for player scores according to their player index
        float[] scores = new float[game.currentPlayers.Length];


        for (int i = 0; i < scores.Length; i++) {
            winScreenMenu.playerBoxes[i].AssignBadge(100);
        }

        winScreenMenu.SetWinText(survivalModeScoreText.text);

    }

    void DetermineTimeBadges() {
        //make array for player scores according to their player index
        float[] scores = new float[game.currentPlayers.Length];

        //fill it with # of rounds won, and -1 if the player is not in the game
        for (int i = 0; i < scores.Length; i++) {
            if (game.currentPlayers[i] == null)
                scores[i] = -1000000;
            else {
                float killsMinusDeaths = game.currentPlayers[i].character.GetNumberOfKills() - game.currentPlayers[i].character.GetNumberOfDeaths();
                scores[i] = killsMinusDeaths;
            }
        }

        for (int i = 0; i < scores.Length; i++) {
            DetermineRank(i, scores);
        }

    }

    void DetermineRank(int playerIndex, float[] scores) {

        float myScore = scores[playerIndex];

        int myRank = 0;

        for (int i = 0; i < scores.Length; i++) {
            if (i == playerIndex) continue;

            if (scores[i] > myScore) {
                myRank++;
            }
        }

        winScreenMenu.playerBoxes[playerIndex].AssignBadge(myRank);

        if(myRank == 0) {

            string winner = game.currentPlayers[playerIndex].characterName;

            winScreenMenu.SetWinText(winner);
        }

    }

    void DestroyLevel() {
        GameObject oldLevel = GameObject.FindGameObjectWithTag("Level");

		DestroyImmediate (oldLevel);
        smashCam.RemoveAllTargets();
    }

    void LoadWinScreen() {
        gameCanvas.CloseMenu();
        menuNav.GoToScreen("Win_Screen");
    }

    void FabricateGame () {

		game.SetLevel (GameObject.FindGameObjectWithTag ("Level").GetComponent <Level> ());

		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");

		for (int i = 0; i < players.Length; i++) {

            game.SetPlayer (i, players[i].GetComponent<PlayerInput> (), transform.GetChild(i).GetComponent<InputController> (), players[i].GetComponent<Player> ().hat.spriteRenderer.sprite, players[i].transform.name, 1, players[i].GetComponentInChildren<SpriteRenderer>().sprite);
			game.currentPlayers[i].ConnectInput ();
			game.currentPlayers[i].character.playerIndex = i;
			playerDisplays[i].emblem.sprite = game.currentPlayers[i].emblem;

			smashCam.AddTarget (players[i].transform, players[i].GetComponent<Player> ().hat.transform);

		}

		game.state = Game.State.PLAYING;
		
	}

	public void StartGame () {
		
		menuNav.CloseMenu ();
        gameCanvas.OpenMenu();
        game.Start ();

        GetComponent<SpawnCast>().castIsSpawned = false;

	}

    public void CheckForPause () {

		for (int i = 0; i < game.currentPlayers.Length; i++) {

			if (game.currentPlayers[i] != null && game.currentPlayers[i].inputSender.snapshot.startButton.down) {

				if (game.state == Game.State.PLAYING) {
					PauseGame ();

				} else if (game.state == Game.State.PAUSED) {
					UnPauseGame ();

				}

			}

		}

	}

	public void PauseGame () {
		game.state = Game.State.PAUSED;
		pauseMenu.SetActive (true);
	}

	public void UnPauseGame () {
		game.state = Game.State.PLAYING;
		pauseMenu.SetActive (false);
	}

	public void ReturnToMainMenu () {

        DestroyLevel();

        pauseMenu.SetActive(false);
        gameCanvas.CloseMenu();
        ResetCharacterSelectScreen();

	}

    public bool ControllerHasBeenDisconnected(int _index, bool stayDisconnected) {

        if (controllerHasDisconnected[_index]) {
            controllerHasDisconnected[_index] = stayDisconnected;
            return true;
        }
        else return false;

    }

    public void PlayerHasDisconnectedController(int _index) {
        controllerHasDisconnected[_index] = true;
    }

}
