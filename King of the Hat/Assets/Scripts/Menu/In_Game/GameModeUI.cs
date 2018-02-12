using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameModeUI : MonoBehaviour {

    //toggleUI
    [Header ("Dev Tools")]
    public GameObject emblemHolder;

    [Header("Last Hat Standing Game Mode")]
    public GameObject[] lastHatModeUI;
    [Space(10)]

    [Header("Stock Game Mode")]
    public GameObject[] stockModeUI;
    public StockHandler[] stockHandlers;
    [Space(10)]

    [Header ("Time Game Mode")]
    public GameObject[] timeModeUI;
    [Space(10)]

    [Header("Survival Game Mode")]
    public GameObject[] survivalModeUI;
    [Space(10)]

    [Header("Teams Mode")]
    public GameObject teamsModeUI;
    public GameObject[] eachTeamsUI;

    void OnEnable() {
        EnableModeSpecificUI();
    }

    bool toggleUI = true;
    void Update() {

        if (!GameController.instance.devToolsEnabled) return;
        
        if(Input.GetKeyDown(KeyCode.U)) {
            toggleUI = !toggleUI;
            emblemHolder.SetActive(toggleUI);

            for(int i = 0; i < lastHatModeUI.Length; i++) {
                lastHatModeUI[i].SetActive(toggleUI);
            }

        }

    }

    public void EnableModeSpecificUI() {

        CloseAllGameModeUI();

        switch (GameController.instance.game.currentGameMode) {
            case Game.Mode.LASTHAT:
                ToggleLastHatStandingUI(true);               
                break;

            case Game.Mode.STOCK:
                ToggleStockUI(true);

                for (int i = 0; i < lastHatModeUI.Length; i++) {
                    InitializeStockUI(i);
                }

                break;

            case Game.Mode.TIME:
                ToggleTimeUI(true);
                break;

            case Game.Mode.SURVIVAL:
                ToggleStockUI(true);
                ToggleSurvivalModeUI(true);

                for (int i = 0; i < lastHatModeUI.Length; i++) {
                    InitializeStockUI(i);
                }

                break;

            case Game.Mode.TEAMS:
                ToggleTeamsUI(true);
                
                break;
            default:
                break;
        }

    }

    public void ToggleLastHatStandingUI(bool enabled) {
        for(int i = 0; i < lastHatModeUI.Length; i++) {
            lastHatModeUI[i].SetActive(enabled);
        }

        //if (enabled) Invoke("DisableUI", 5f);

    }

    public void ToggleSurvivalModeUI(bool enabled) {
        for (int i = 0; i < survivalModeUI.Length; i++) {
            survivalModeUI[i].SetActive(enabled);
        }

    }

    public void EnableUI() {
        emblemHolder.SetActive(true);

        Invoke("DisableUI", 5);
    }

    public void DisableUI() {
        emblemHolder.SetActive(false);
    }

    public void ToggleStockUI(bool enabled) {
        for (int i = 0; i < stockModeUI.Length; i++) {
            stockModeUI[i].SetActive(enabled);
        }
    }

    public void InitializeStockUI(int _playerIndex) {
        stockHandlers[_playerIndex].UpdateStock(GameController.instance.game.gameHatHp);
    }

    public void ToggleTimeUI(bool enabled) {
        for (int i = 0; i < timeModeUI.Length; i++) {
            timeModeUI[i].SetActive(enabled);
        }
    }

    public void ToggleTeamsUI(bool enabled) {

        teamsModeUI.SetActive(enabled);

        if (!enabled) return;

        for(int i = 0; i < eachTeamsUI.Length; i++) {

            if (GameController.instance.game.teamScores[i] != -1) {
                eachTeamsUI[i].SetActive(true);
            } else {
                eachTeamsUI[i].SetActive(false);
            }

        }
    }

    public void CloseAllGameModeUI() {
        ToggleLastHatStandingUI(false);
        ToggleStockUI(false);
        ToggleTimeUI(false);
        ToggleSurvivalModeUI(false);
        ToggleTeamsUI(false);
    }

}
