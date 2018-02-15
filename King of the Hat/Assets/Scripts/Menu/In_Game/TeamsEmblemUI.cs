using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamsEmblemUI : MonoBehaviour {

    public int teamNumber = 1;
    int numberOfPlayersOnThisTeam = 0;

    Image[] emblems = new Image[4];

	void OnEnable () {
        FindEmblems();
        numberOfPlayersOnThisTeam = GetNumberOfPlayersOnThisTeam();
        ToggleEmblems();
        InitEmblems();
	}
	
    void FindEmblems() {
        for(int i = 0; i < emblems.Length; i++) {
            emblems[i] = transform.GetChild(i).GetComponent<Image>();
        }
    }

    int GetNumberOfPlayersOnThisTeam() {
        int _nb = 0;

        for (int i = 0; i < GameController.instance.game.currentPlayers.Length; i++) {

            if (GameController.instance.game.currentPlayers[i] == null) continue;

            if (GameController.instance.game.currentPlayers[i].teamNumber == teamNumber) 
                _nb++;

        }

        return _nb;

    }

    void ToggleEmblems() {
        
        for(int i = 0; i < emblems.Length; i++) {
            if(i >= numberOfPlayersOnThisTeam) {
                emblems[i].gameObject.SetActive(false);
            } else {
                emblems[i].gameObject.SetActive(true);
            }
        }

    }

    void InitEmblems() {

        int _nextIndex = 0;

        for(int i = 0; i < GameController.instance.game.currentPlayers.Length; i++) {

            if (GameController.instance.game.currentPlayers[i] == null) continue;
            
            if (GameController.instance.game.currentPlayers[i].teamNumber == teamNumber) {

                emblems[_nextIndex++].sprite = GameController.instance.game.currentPlayers[i].emblem;

            }

        }

    }

}
