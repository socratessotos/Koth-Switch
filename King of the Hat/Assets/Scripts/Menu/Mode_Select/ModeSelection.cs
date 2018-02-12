using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ModeInfo {
    public Game.Mode mode;
    public string measure;
    public int[] measureOptions;
}

public class ModeSelection : MonoBehaviour {

    [Header("Menu Options to Add")]
    public GameObject mode;
    public GameObject modeMeasureTitle;
    public GameObject modeMeasure;
    public GameObject playButton;
    [Space(10)]

    [Header("Game Mode Screen")]
    public Text modeNameText;
    public Text measureTitleText;
    public Text measureTitleTextMask;
    public Text measureText;
    [Space(10)]

    [Header("Mode Setup")]
    public ModeNav nav;
    public ModeInfo[] modes;
    [Space(10)]

    [Header("Animators")]
    public Animator modeLeftArrow;
    public Animator modeRightArrow;
    public Animator measureLeftArrow;
    public Animator measureRightArrow;
    public Animator modeTextAnim;
    public Animator measureTextAnim;

    int rowIndex = 0;
    int modeIndex = 0;
    int modeMeasureIndex = 0;

    void OnEnable() {

        InitalizeModeSelectonScreen();

    }

    void InitalizeModeSelectonScreen() {

        rowIndex = 0;
        modeIndex = 0;
        modeMeasureIndex = 0;

        mode.SetActive(true);
        modeMeasureTitle.SetActive(false);
        modeMeasure.SetActive(false);
        playButton.SetActive(false);

        SetModeTitleText();
        SetMeasureTitleText();
        SetMeasureValueText();

        nav.menuOptions[0] = mode.GetComponent<RectTransform>();
        nav.MoveSelectionTo(0);

    }

    public void ConfirmMode() {

        modeMeasureIndex = 0;
        ShowNextModeValue(0);

        SetModeTitleText();
        SetMeasureTitleText();

        nav.menuOptions[0] = modeMeasure.GetComponent<RectTransform>();
        nav.MoveSelectionTo(0);

        modeMeasureTitle.SetActive(true);
        modeMeasure.SetActive(true);

        rowIndex = 1;

    }

    void SetModeTitleText() {
        modeNameText.text = GameController.instance.game.GameModeToString(modes[modeIndex].mode);
    }

    void SetMeasureTitleText() {
        measureTitleText.text = modes[modeIndex].measure;
        measureTitleTextMask.text = modes[modeIndex].measure;
    }

    void SetMeasureValueText() {
        measureText.text = modes[modeIndex].measureOptions[modeMeasureIndex].ToString();
    }

    public void UnConfirmMode() {

        nav.menuOptions[0] = mode.GetComponent<RectTransform>();
        nav.MoveSelectionTo(0);

        modeMeasureTitle.SetActive(false);
        modeMeasure.SetActive(false);

        rowIndex = 0;
    }

    public void ConfirmModeSettings() {

        nav.menuOptions[0] = playButton.GetComponent<RectTransform>();
        nav.MoveSelectionTo(0);

        playButton.SetActive(true);

        rowIndex = 2;

        GameController.instance.game.SetGameMode(modes[modeIndex].mode, modes[modeIndex].measureOptions[modeMeasureIndex]);
    }

    public void UnConfirmSettings() {

        nav.menuOptions[0] = modeMeasure.GetComponent<RectTransform>();
        nav.MoveSelectionTo(0);

        playButton.SetActive(false);

        rowIndex = 1;
    }

    public int GetRowIndex() {
        return rowIndex;
    }

    public void ShowNextMode(int direction) {

        modeIndex += direction;

        if (modeIndex < 0) modeIndex = modes.Length - 1;
        if (modeIndex > modes.Length - 1) modeIndex = 0;

        SetModeTitleText();

        if(direction > 0) {
            AnimateButton(modeRightArrow, "Bounce");
        } else if(direction < 0) {
            AnimateButton(modeLeftArrow, "Bounce");
        }

        AnimateButton(modeTextAnim, "Bounce");

    }

    public void ShowNextModeValue(int direction) {

        modeMeasureIndex += direction;

        if (modeMeasureIndex < 0) modeMeasureIndex = modes[modeIndex].measureOptions.Length - 1;
        if (modeMeasureIndex > modes[modeIndex].measureOptions.Length - 1) modeMeasureIndex = 0;

        SetMeasureValueText();

        if (direction > 0) {
            AnimateButton(measureRightArrow, "Bounce");
        } else if (direction < 0) {
            AnimateButton(measureLeftArrow, "Bounce");
        }

        AnimateButton(measureTextAnim, "Bounce");

    }

    void AnimateButton(Animator anim, string trigger) {
        anim.SetTrigger(trigger);
    }

}
