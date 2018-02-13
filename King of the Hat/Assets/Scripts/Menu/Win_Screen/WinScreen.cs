using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class WinScreen : MonoBehaviour {

    public Text winText;
    public Text winTextMask;

    public WinScreenPlayerBox[] playerBoxes;
    public Sprite[] bannerColors;

    float[] timers = new float[4];
    public float quitTime = 4f;

    int biggestTimerIndex = 0;

    void Awake() {
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
    }

    void OnEnable() {
        InitializeWinScreen();

        ResetTimers();

    }

    void InitializeWinScreen() {

        ClearInput();
        ConnectInput();

        SetPlayerBoxes();
        UpdateBanners();

    }

    void Update() {

        CheckIfAnyPlayerHasDisconnectedController();

        CheckIfPlayersReady();

        CheckIfPlayerIsForceQuitting();

    }

    void CheckIfPlayersReady() {
 
        for (int i = 0; i < playerBoxes.Length; i++) {
            if (playerBoxes[i].input == null) continue;
            if (!playerBoxes[i].IsReady()) return; 
        }

        for(int i = 0; i < playerBoxes.Length; i++) {
            if (playerBoxes[i].input == null) continue;
            playerBoxes[i].SetStateToExiting();
        }

        Invoke("LeaveWinScreen", 1.5f);
    }

    void CheckIfPlayerIsForceQuitting() {

        for (int i = 0, l = timers.Length; i < l; i++) {

            if (GameController.instance.playerInputs[i].snapshot.startButton.pressed) {

                timers[i] += Time.fixedDeltaTime;

                if (timers[i] > timers[biggestTimerIndex])
                    biggestTimerIndex = i;

                if (timers[i] >= quitTime)
                    LeaveWinScreen();

            } else {
                timers[i] = 0;
            }

        }

    }

    void LeaveWinScreen() {
        ResetTimers();
        GameController.instance.ResetCharacterSelectScreen();
    }

    public void SetWinText(string name) {

        if(GameController.instance.game.currentGameMode == Game.Mode.SURVIVAL) {
            winText.text = name + " ZOMBIES DEFEATED";
            winTextMask.text = name + " ZOMBIES DEFEATED";
        } else {
            winText.text = name + " WINS";
            winTextMask.text = name + " WINS";
        }
        
    }

    void ConnectInput() {
        for (int i = 0; i < playerBoxes.Length; i++) {           
            if (GameController.instance.game.currentPlayers[i] != null)
                playerBoxes[i].input = GameController.instance.playerInputs[i];          
        }
    }

    void ClearInput() {
        for (int i = 0; i < playerBoxes.Length; i++) {
            playerBoxes[i].input = null;
        }
    }

    void SetPlayerBoxes() {
        for(int i = 0; i < playerBoxes.Length; i++) {

            if (playerBoxes[i].input != null) {

                Player player = GameController.instance.game.currentPlayers[i].character;

                playerBoxes[i].SetStats(player.GetNumberOfKills(), player.GetNumberOfDeaths(), player.GetNumberOfThrowsThisGame(), (int)player.GetAccuracy());

                playerBoxes[i].Init();

            } else {
                playerBoxes[i].gameObject.SetActive(false);
            }
        }
    }

    IEnumerator ActivatePlayerBoxes() {

        float t = 0;

        for (int i = 0; i < playerBoxes.Length; i++) {

            if (playerBoxes[i].input != null) {

                playerBoxes[i].gameObject.SetActive(true);

                while (t < 0.3f) {
                    t += Time.deltaTime;
                    yield return null;
                }

                t = 0;

            }
        }

        for(int i = 0; i < playerBoxes.Length; i++) {
            if(playerBoxes[i].input != null) {
                playerBoxes[i].SetStateToStats();
            }
        }

    }

    void OnControllerDisconnected(ControllerStatusChangedEventArgs args) {

        if (GameController.instance.playerInputs[args.controllerId].snapshot.isAddedToGame) {
            GameController.instance.PlayerHasDisconnectedController(args.controllerId);
        }

    }

    void CheckIfAnyPlayerHasDisconnectedController() {
        for (int i = 0; i < playerBoxes.Length; i++) {

            if (playerBoxes[i].IsExiting()) return;

            if (GameController.instance.ControllerHasBeenDisconnected(i, true) && !playerBoxes[i].IsReady())
                playerBoxes[i].ReadyUp();
        }
    }

    void ResetTimers() {
        for (int i = 0; i < timers.Length; i++) {
            timers[i] = 0;
        }
    }

    void UpdateBanners() {
        
        for(int i = 0; i < playerBoxes.Length; i++) {

            if (GameController.instance.game.currentPlayers[i] == null) continue;

            playerBoxes[i].banner.GetComponent<Image>().sprite = bannerColors[GameController.instance.game.currentPlayers[i].teamNumber - 1];

        }

    }

}
