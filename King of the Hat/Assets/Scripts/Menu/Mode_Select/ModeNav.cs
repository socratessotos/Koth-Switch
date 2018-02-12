using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeNav : ControllerNav {

    ModeSelection modeSelection;

    void Awake() {
        modeSelection = GetComponent<ModeSelection>();
    }

    public override void BackButtonPress() {

        switch(modeSelection.GetRowIndex()) {
            case 0:
                GameController.instance.menuNav.GoToScreen("Level_Select");
                break;
            case 1:
                modeSelection.UnConfirmMode();
                break;
            case 2:
                modeSelection.UnConfirmSettings();
                break;
            default:
                break;
        }
        
    }

    public override void Move(Vector2 _input) {

        switch(modeSelection.GetRowIndex()) {
            case 0:
                modeSelection.ShowNextMode((int)Mathf.Sign(_input.x));
                break;
            case 1:
                modeSelection.ShowNextModeValue((int)Mathf.Sign(_input.x));
                break;
            case 2:
                break;
            default:
                break;
        }

    }

}
