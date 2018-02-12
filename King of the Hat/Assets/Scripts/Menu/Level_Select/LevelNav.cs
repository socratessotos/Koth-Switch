using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LevelNav : ControllerNav {

    LevelSelection levelSelection;
    RectTransform levelSelectionTransform;

    public Vector3 lerpStartPoint;
    public Vector3 lerpEndPoint;

    public Vector3 defaultImageScale;
    public Vector3 largeImageScale;

    float lerpT = 0;

    void Awake() {
        levelSelection = GetComponentInChildren<LevelSelection>();
        levelSelectionTransform = levelSelection.GetComponent<RectTransform>();

    }

    void OnEnable() {
        DestroyLevel();
        ShowLevel();
    }

    void Start() {
        //default values
        levelSelection.SetPosition(index);
        levelSelection.GrowImage(index);
    }

	public override void BackButtonPress () {
        DestroyLevel();
		GameController.instance.menuNav.GoToScreen ("Character_Select");
	}

    public override void Move(Vector2 _input) {

        if (Mathf.Abs(_input.y) > 0.5f)
            return;
        
        index += (int) Mathf.Sign(_input.x);

        if (index < 0) index = 0;
        if (index > menuOptions.Length - 1) index = menuOptions.Length - 1;

        lerpT = 0;
        SetLerpParameters(index);

        DestroyLevel();
        ShowLevel();

    }

    public void SetLerpParameters(int _index) {

        lerpStartPoint = levelSelectionTransform.localPosition;
        lerpEndPoint = new Vector2(-(levelSelection.defaultPreviewWidth + levelSelection.spacing) * _index, -360);

        for (int i = 0; i < menuOptions.Length; i++) {
            menuOptions[i].localScale = defaultImageScale;
        }
    }

    public override void LerpToNextOption() {

        lerpT += Time.deltaTime;

        levelSelectionTransform.localPosition = Vector3.Lerp(lerpStartPoint, lerpEndPoint, lerpT / scrollRate);
        menuOptions[index].localScale = Vector3.Lerp(defaultImageScale, largeImageScale, lerpT / scrollRate);
    }

    void ShowLevel() {

        GameController.instance.game.SetLevel(levelSelection.levels[index].levelPrefab);
        GameController.instance.game.CreateLevelPreview();

        DisplayLevelName();

    }

    void DisplayLevelName() {
        levelSelection.levelNameText.text = levelSelection.levels[index].levelName;
        levelSelection.levelNameTextShadow.text = levelSelection.levels[index].levelName;
    }

    void DestroyLevel() {
        if (GameController.instance.game.level == null)
            return;

        DestroyLevelPreview();
        GameController.instance.game.level = null;
    }

	public void DestroyLevelPreview() {

		GameObject[] array = GameObject.FindGameObjectsWithTag ("Level");

		for(int i = 0; i < array.Length; i++) {

			Destroy (array [i]); 	 	 	
		}

    }

}