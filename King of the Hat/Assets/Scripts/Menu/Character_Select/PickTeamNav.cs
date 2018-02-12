using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickTeamNav : ControllerNav {

    CharacterBox characterBox;

    Vector3 lerpStartPoint;
    Vector3 lerpEndPoint;

    float lerpT = 0;

    InputController inputController;

    void Start () {
        characterBox = GetComponentInParent<CharacterBox>();
        inputController = characterBox.input;

        index = 0;
        scrollTimer = Time.time;

        SetLerpParameters(index);

    }
    
    void Update() {

        input = Vector2.zero;

        if (lerping) {

            LerpToNextOption();

            if (Time.time > scrollTimer) {
                lerping = false;
            }

        }

        if (Input.GetKeyDown(KeyCode.Return)) {

            menuOptions[index].GetComponent<Button>().onClick.Invoke();

        }

        if (inputController.snapshot.aButton.down) {
            menuOptions[index].GetComponent<Button>().onClick.Invoke();
        }

        if (inputController.snapshot.bButton.down) {
            BackButtonPress();
        }

        if (inputController.snapshot.VerticalPressed() && !lerping) {
            input.y = -inputController.snapshot.yAxis;
        }

        if (input != Vector2.zero) {
            if (Time.time > scrollTimer) {
                Move(input);
                input = Vector2.zero;
                scrollTimer = Time.time + scrollRate;
                lerping = true;
            }
        }

    }

    public override void Move(Vector2 _input) {

        if (Mathf.Abs(_input.x) > 0.5f)
            return;

        index += (int)Mathf.Sign(_input.y);

        if (index < 0) index = 0;
        if (index > menuOptions.Length - 1) index = menuOptions.Length - 1;

        lerpT = 0;
        SetLerpParameters(index);

    }

    public void SetLerpParameters(int _index) {

        lerpStartPoint = selector.localPosition;
        lerpEndPoint = menuOptions[_index].localPosition + Vector3.left * 170;

    }

    public override void LerpToNextOption() {

        lerpT += Time.deltaTime;

        selector.localPosition = Vector3.Lerp(lerpStartPoint, lerpEndPoint, lerpT / scrollRate);


    }

    public override void BackButtonPress() {
        //characterBox.ToggleTeamMenu (false);
        //also give control back to char select box
    }

    public void ChooseTeam() {
        characterBox.teamNumber = index + 1;

        characterBox.UpdateTeamTextAndColor();

        GameController.instance.game.currentPlayers[characterBox.playerNumber - 1].SetTeamNumber(characterBox.teamNumber);

    }

}
