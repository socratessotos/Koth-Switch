using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMode {

	public int numPlayers;

}

public class Game {

	public enum State {SETUP, INTRO, COUNTDOWN, PLAYING, PAUSED, REPLAY, INBETWEEN, OVER}
    public enum Mode {LASTHAT, PRACTICE, SURVIVAL, SHOWDOWN, TEAMS, HATKING, HATSKETBALL, HATKEYSACK, STOCK, TIME }

	public InGamePlayer[] currentPlayers;
	public Level level;
	public State state;
    public Mode currentGameMode;

	public int livesPerRound = 1;
	public int roundsPerGame = 5;
    public int timeLimitPerRound = 5;
    public float timeRemaining = 5;
    public int gameHatHp = 3;

    public int[] teamScores = new int[4];

    public float countdownTimer = 240;

    float startTime;

	public Game () {

		currentPlayers = new InGamePlayer[4];
        ResetTeamScores();

		level = null;

		state = State.SETUP;

	}

	public void SetLevel (Level l) {
		level = l;
	}

	public void SetPlayer (int index, PlayerInput inputReader, InputController inputSender, Sprite emblem, string characterName, int teamNumber, Sprite characterPreview) {
		currentPlayers [index] = new InGamePlayer (inputReader, inputSender, emblem, characterName, teamNumber, characterPreview, GameController.instance.gameModeUI.stockHandlers[index]);
	}

    public void SetPlayerToNull (int index) {
		currentPlayers [index] = null;
	}

	public void CreateLevel () {

		level = Level.Instantiate (level);
		level.gameObject.transform.position = Vector3.zero;
		level.gameObject.transform.rotation = Quaternion.identity;
        level.gameObject.transform.localScale = Vector3.one;
		//level.FindRespawnableObjects ();

        level.CreateCamera();
        //Camera.main.GetComponent<SmashCam>().InitiateTheatricEntranceCam(level.startingSpawnPoints);

    }

    public void CreateLevelPreview() {

        level = Level.Instantiate(level);
        level.gameObject.transform.position = Vector3.down * 3f;
        level.gameObject.transform.rotation = Quaternion.identity;
        level.gameObject.transform.localScale = Vector3.one;
		//level.FindRespawnableObjects ();

        level.CreateCamera();

    }

	public void CreatePlayers () {

		for (int i = 0; i < currentPlayers.Length; i++) {

            if (currentPlayers[i] != null) {

                PlayerInput playerInstance = PlayerInput.Instantiate (currentPlayers[i].inputReader);
				currentPlayers[i] = new InGamePlayer (playerInstance, currentPlayers[i].inputSender, currentPlayers[i].emblem, currentPlayers[i].characterName, currentPlayers[i].teamNumber, currentPlayers[i].characterPreview, GameController.instance.gameModeUI.stockHandlers[i]);
				currentPlayers[i].inputReader.Init ();
				currentPlayers[i].character.playerIndex = i;
                currentPlayers[i].character.teamNumber = currentPlayers[i].teamNumber;
				currentPlayers[i].ConnectInput ();

				playerInstance.gameObject.transform.position = level.startingSpawnPoints[i].position;
				playerInstance.gameObject.transform.rotation = Quaternion.identity;
				playerInstance.gameObject.layer = LayerMask.NameToLayer ("Player " + (i + 1));

                if (GameController.instance.game.currentGameMode == Game.Mode.TEAMS) {

                } else {
                    GameController.instance.playerDisplays[i].emblem.enabled = true;
                    GameController.instance.playerDisplays[i].emblem.sprite = currentPlayers[i].emblem;
                    GameController.instance.playerDisplays[i].scoreText.gameObject.SetActive(true);
                    //GameController.instance.playerDisplays[i].emblem.SetNativeSize();

                }

                GameController.instance.smashCam.AddTarget (currentPlayers[i].character.transform, currentPlayers[i].character.hat.transform);

				playerInstance.transform.parent = level.transform;

				playerInstance.GetComponent <CharacterColorChanger> ().Init (
			    GameController.instance.playerDisplays[i].skinIndex,
			    GameController.instance.playerDisplays[i].colorIndex);

                currentPlayers[i].SetStockImages();
                SetScoreText(i);

                if(currentGameMode == Game.Mode.SURVIVAL) {
                    playerInstance.gameObject.layer = LayerMask.NameToLayer("Player 1");
                }

                if(currentGameMode == Game.Mode.TEAMS) {
                    playerInstance.gameObject.layer = LayerMask.NameToLayer("Player " + currentPlayers[i].teamNumber);
                }

            } else {

				GameController.instance.playerDisplays[i].emblem.enabled = false;
                GameController.instance.playerDisplays[i].scoreText.gameObject.SetActive(false);

            }

        }

	}

    public void Start () {

		CreateLevel ();
		CreatePlayers ();

        startTime = Time.time;

        if(GameController.instance.countdownIsEnabled) {
            state = State.COUNTDOWN;

            countdownTimer = 150f;
            GameController.instance.countdownText.gameObject.SetActive(true);
        } else {
            state = State.PLAYING;
        }

		InitGameMode ();
        //GameController.instance.gameModeUI.EnableUI();

    }

	public void InitGameMode () {
        
        switch(currentGameMode) {
            case Mode.PRACTICE:
                GameObject targetSpawner = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Target_Spawner"));
                targetSpawner.transform.parent = level.transform.root;
                break;
            case Mode.LASTHAT:
                
                break;

            case Mode.SURVIVAL:
                UpdateSurvivalModeScore();

                GameObject survivalSpawner = GameObject.Instantiate<GameObject>(Resources.Load<GameObject>("Survival_Spawner"));
                survivalSpawner.transform.parent = level.transform.root;

                break;

            case Mode.TEAMS:
                
                for(int i = 0; i < currentPlayers.Length; i++) {

                    if (currentPlayers[i] == null) continue;
                    else {
                        if(currentPlayers[i].teamNumber == i + 1) {
                            teamScores[i] = 0;
                        }
                    } 

                }

                GameController.instance.gameModeUI.ToggleTeamsUI(true);

                break;

            default:
                break;
        }
			
	}

    public void LoseLife (int _playerIndex) {

        if (_playerIndex == -1) return;
        if (_playerIndex > currentPlayers.Length - 1) return;
        
        currentPlayers[_playerIndex].numberOfLives--;

        if (currentGameMode == Mode.STOCK) {
            currentPlayers[_playerIndex].UpdateHatLivesUI();
        }

        if (currentGameMode == Mode.SURVIVAL) {
            currentPlayers[_playerIndex].UpdateStockUI();
        }

    }

    public void RequestRespawn (int playerIndex) {

        int respawnOffset;

        switch (currentGameMode) {
            case Mode.LASTHAT:
                
                break;
            case Mode.STOCK:

                respawnOffset = Random.Range(0, 4);
                
                if (currentPlayers[playerIndex].isAlive) {
                    currentPlayers[playerIndex].character.transform.position = level.startingSpawnPoints[(playerIndex + respawnOffset) % 4].position;
                    currentPlayers[playerIndex].character.Respawn ();
                } else {
                    currentPlayers[playerIndex].timeOfDeath = Time.time;
                }

                break;
                
            case Mode.TIME:
                respawnOffset = Random.Range(0, 4);

                 currentPlayers[playerIndex].character.transform.position = level.startingSpawnPoints[(playerIndex + respawnOffset) % 4].position;
                 currentPlayers[playerIndex].character.Respawn ();

                break;

            case Mode.SURVIVAL:

                respawnOffset = Random.Range(0, 4);

                if (currentPlayers[playerIndex].isAlive) {
                    currentPlayers[playerIndex].character.transform.position = level.startingSpawnPoints[(playerIndex + respawnOffset) % 4].position;
                    currentPlayers[playerIndex].character.Respawn();
                } else {
                    currentPlayers[playerIndex].timeOfDeath = Time.time;
                }

                break;

            case Mode.TEAMS:
                
                

                break;

            default:
                break;
        }

	}

	public bool IsOver () {

        if (currentGameMode == Mode.TIME) {

            if (timeRemaining <= 0)
                return true;
            else
                return false;

        } else if (currentGameMode == Mode.SURVIVAL) {

            int activePlayers = 0;
            int survivingPlayers = 0;

            for (int i = 0; i < currentPlayers.Length; i++) {

                if (currentPlayers[i] != null) {

                    if (currentPlayers[i].isAlive) {
                        survivingPlayers++;
                    }

                    activePlayers++;

                }

            }

            if (survivingPlayers == 0) return true;
            else return false;

        } else if (currentGameMode == Game.Mode.TEAMS) {

            int[] teamSums = new int[4];

            for (int i = 0; i < currentPlayers.Length; i++) {
                if (currentPlayers[i] == null) continue;

                if(currentPlayers[i].isAlive)
                    teamSums[currentPlayers[i].teamNumber - 1]++;
            }

            int teamsAlive = 0;
            int teamIndex = 0;
            for(int i = 0; i < teamSums.Length; i++) {
                if (teamSums[i] > 0) {
                    teamsAlive++;
                    teamIndex = i;
                } 
            }

            if (teamsAlive == 1) {
                //award score to team
                IncrementTeamScore(teamIndex + 1);
                return true;
            }

            else return false;

        } else {

            int activePlayers = 0;
            int survivingPlayers = 0;

            for (int i = 0; i < currentPlayers.Length; i++) {

                if (currentPlayers[i] != null) {

                    if (currentPlayers[i].isAlive) {
                        survivingPlayers++;
                    }

                    activePlayers++;

                }

            }

            if (activePlayers != 1 && survivingPlayers == 1) {

                for (int i = 0; i < currentPlayers.Length; i++)

                    if (currentPlayers[i] != null && currentPlayers[i].isAlive) {

                        if (currentGameMode == Mode.LASTHAT)
                            IncrementScore(i);

                    }
                return true;

            } else if (survivingPlayers < 1) {

                return true;

            } else {

                return false;
            }

        }

	}
    
    void IncrementScore(int _playerIndex) {

        //GameController.instance.gameModeUI.EnableUI();
        GameController.instance.playerDisplays[_playerIndex].anim.SetTrigger("Win");
        currentPlayers[_playerIndex].numberOfRoundsWon++;

        SetScoreText(_playerIndex);

    }

    void IncrementTeamScore(int _teamIndex) {

        //GameController.instance.gameModeUI.EnableUI();
        //GameController.instance.playerDisplays[_playerIndex].anim.SetTrigger("Win");

        teamScores[_teamIndex - 1]++;

        SetTeamScoreText(_teamIndex - 1);

    }

    int CalculateTotalPointsOfTeam(int _teamIndex) {

        int teamSum = 0;

        for(int i = 0; i < currentPlayers.Length; i++) {
            if (currentPlayers[i] == null) continue;

            if(currentPlayers[i].teamNumber == _teamIndex) {
                teamSum += currentPlayers[i].numberOfRoundsWon;
            }
        }

        return teamSum;
    }

    public void UpdateSurvivalModeScore() {

        GameController.instance.survivalModeScoreAnim.SetTrigger("Win");
        
        int _newScore = 0;

        for (int i = 0; i < currentPlayers.Length; i++) {
            if (currentPlayers[i] == null) continue;

            _newScore += currentPlayers[i].character.GetNumberOfKills();
        }

        GameController.instance.survivalModeScoreText.text = _newScore.ToString();
        
    }

    public void Update2v2Score() {


    }

    void AnimateScoreIncrease() {

    }

    void SetScoreText(int _playerIndex) {
		
        GameController.instance.playerDisplays[_playerIndex].scoreText.text = currentPlayers[_playerIndex].numberOfRoundsWon.ToString();

    }

    void SetTeamScoreText(int _teamIndex) {

        GameController.instance.teamScoreTexts[_teamIndex].text = teamScores[_teamIndex].ToString();

    }

    public bool CheckIfTheresAWinner() {
    /*
        if (currentGameMode == Mode.SURVIVAL) {

            for (int p = 0; p < currentPlayers.Length; p++) {
                if (currentPlayers[p].numberOfLives > 0) {
                    return false;
                } else continue;
            }

            return true;
        }
        */
        for (int i = 0; i < currentPlayers.Length; i++) {
            if (currentPlayers[i] == null) continue;

            if (currentGameMode == Mode.LASTHAT) {
                
				if (currentPlayers[i].numberOfRoundsWon >= roundsPerGame) {

                    SetRandomEndGameQuote();
                    return true;

                }

            } else if (currentGameMode == Mode.STOCK) {
				
                if (!currentPlayers[i].isAlive) continue;
                return true;

            } else if (currentGameMode == Mode.TIME) {
                GameController.instance.SetGameOverText("TIME UP!");
                return true;

			} else if (currentGameMode == Mode.PRACTICE) {




			} else if(currentGameMode == Mode.TEAMS) {

                if(OneTeamWins()) {

                    return true;

                }

            }
              
        }

        return false;
    }

    bool OneTeamWins() {

        for(int s = 0; s < teamScores.Length; s++) {
            if(teamScores[s] >= roundsPerGame) {
                SetRandomEndGameQuote();
                return true;
            }
        }

        return false;
    }

    void SetRandomEndGameQuote() {
        switch (Random.Range(0, 5)) {
            case 0:
                GameController.instance.SetGameOverText("HAT'S A WRAP!");
                break;
            case 1:
                GameController.instance.SetGameOverText("HAT'S ALL FOLKS!");
                break;
            case 2:
                GameController.instance.SetGameOverText("HAT'S THE GAME!");
                break;
            case 3:
                GameController.instance.SetGameOverText("GAME, SET, & HAT!");
                break;
            case 4:
                GameController.instance.SetGameOverText("IS HAT ALL YOU GOT?");
                break;
            default:
                GameController.instance.SetGameOverText("HAT'S A WRAP!");
                break;
        }
    }

	public void Reset () {

        int offset = Random.Range(0, 4);

		for (int i = 0; i < currentPlayers.Length; i++) {

			if (currentPlayers[i] == null) continue;

			currentPlayers[i].character.transform.position = level.startingSpawnPoints[(i + offset) % 4].position;
			currentPlayers[i].Reset ();

		}

		level.RespawnObjects ();

		state = State.PLAYING;
		
	}

    void ResetTeamScores() {
        for(int i = 0; i < teamScores.Length; i++) {
            teamScores[i] = -1;
        }
    }

    public void SetGameMode(Game.Mode mode, int modeValue) {

        switch (mode) {

            case Game.Mode.LASTHAT:
                currentGameMode = Mode.LASTHAT;
                roundsPerGame = modeValue;
                livesPerRound = 1;
                break;

            case Game.Mode.TEAMS:
                currentGameMode = Mode.TEAMS;
                roundsPerGame = modeValue;
                livesPerRound = 1;
                break;

            case Game.Mode.STOCK:
                currentGameMode = Mode.STOCK;
                livesPerRound = modeValue;
                break;

            case Game.Mode.TIME:
                currentGameMode = Mode.TIME;
                timeLimitPerRound = modeValue * 60;
                timeRemaining = timeLimitPerRound;
                livesPerRound = 1;
                break;

	        case Game.Mode.PRACTICE:
			    currentGameMode = Mode.PRACTICE;
			    timeLimitPerRound = modeValue * 60;
			    timeRemaining = timeLimitPerRound;
			    livesPerRound = 1;
			    break;

            case Game.Mode.SURVIVAL:
                currentGameMode = Mode.SURVIVAL;
                roundsPerGame = 10000;
                livesPerRound = 3;
                break;

            default:
                break;
        }

        SetStartingPlayerLives();

    }

    void SetStartingPlayerLives() {
        for(int i = 0; i < currentPlayers.Length; i++) {
            if (currentPlayers[i] == null) continue;

            currentPlayers[i].numberOfLives = livesPerRound;
            //currentPlayers[i].UpdateStockUI();
        }
    }

    public string GameModeToString(Game.Mode mode) {
        string s = "";    

        switch(mode) {
            case Game.Mode.LASTHAT:
                s = "Last Hat Standing";
                break;
            case Game.Mode.TEAMS:
                s = "Teams";
                break;
            case Game.Mode.STOCK:
                s = "Stock";
                break;
            case Game.Mode.TIME:
                s = "Time";
                break;
            case Game.Mode.SURVIVAL:
                s = "Survival Mode";
                break;
            default:
                s = "";
                break;
        }

        return s;
    }

	public void End () {

	}

	public class InGamePlayer {

		public PlayerInput inputReader;
		public InputController inputSender;
        public Sprite emblem;
        public Sprite characterPreview;
        public int numberOfLives;
		public int numberOfRoundsWon;
        public string characterName;
        public int teamNumber;
        public StockHandler stockHandler;
        public float timeOfDeath;

		public Player character {get {return inputReader.character;}}
		public bool isAlive {get {return numberOfLives > 0;}}
        
        public InGamePlayer(PlayerInput _inputScript, InputController _controllerScript, Sprite _emblem, string _characterName, int _teamNumber, Sprite _characterPreview, StockHandler _stockHandler) {

            inputReader = _inputScript;
            inputSender = _controllerScript;
            emblem = _emblem;
            characterPreview = _characterPreview;
            characterName = _characterName;
            teamNumber = _teamNumber;
            stockHandler = _stockHandler;

            numberOfRoundsWon = 0;
            numberOfLives = GameController.instance.game.livesPerRound;
            timeOfDeath = 0;

        }

        public void SetTeamNumber(int _num) {
            teamNumber = _num;
        }

        public void SetStockImages() {
            stockHandler.SetStockImages(emblem);
        }

        public void UpdateHatLivesUI() {
            stockHandler.UpdateStock(character.hat.currentHp);
        }

        public void UpdateStockUI() {
            stockHandler.UpdateStock(numberOfLives);
        }

        public void ConnectInput () {
			inputReader.input = inputSender;
		}

		public void Reset () {

			character.Respawn ();

            if(GameController.instance.game.currentGameMode != Mode.STOCK || GameController.instance.game.currentGameMode != Mode.SURVIVAL)
			    numberOfLives = 1;

		}

	}

    

}

