using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class LevelInfo {
    public string levelName;
	public Sprite preview;
	public Level levelPrefab;

}

[RequireComponent (typeof (RectTransform))]
public class LevelSelection : MonoBehaviour {

	public GameObject emptyUI;

    [Header("Level Screen")]
    public float defaultPreviewWidth = 235;
	public int previewsPerLine = 5;
	public float spacing = 20f;
	public float previewAspect = 16/9;
	[Space (10)]

	[Header ("Level Setup")]
    public Text levelNameText;
    public Text levelNameTextShadow;
    public LevelNav nav;
	public LevelInfo[] levels;
    [Space (10)]

	RectTransform rectTransform;
	float previewWidth;
	float previewHeight;

	void Awake () {
		//get the attached rect transform
		rectTransform = GetComponent <RectTransform> ();

		//create the level preview images
		//and initialize their values
		InitializeLevelScreen ();

	}

	public void InitializeLevelScreen () {

        //figure out how wide the level preview images should be
        previewWidth = defaultPreviewWidth;

		//figure out how high the level preview images should be
		previewHeight = previewWidth/previewAspect;

		//init the nav array to the size of the levels
		nav.menuOptions = new RectTransform[levels.Length];
		//nav.optionsPerRow = previewsPerLine;

		//create previews for the levels
		for (int i = 0; i < levels.Length; i++) {
			CreateLevelPreview (i);
		}

	}

	public void CreateLevelPreview (int index) {

		//create a new GameObject
		GameObject myGO = Instantiate (emptyUI);
		myGO.name = "Level_Preview_" + "0" + (index + 1);

		//reset the position
		myGO.transform.SetParent (transform, false);

		//create the image
		Image preview = myGO.AddComponent <Image> ();
		preview.sprite = levels [index].preview;

		//add a button
		Button button = myGO.AddComponent <Button> ();

		button.onClick.AddListener(() => LoadLevel(index));
        button.onClick.AddListener(() => nav.DestroyLevelPreview());
        button.onClick.AddListener(() => GameController.instance.StartGame ());
        //button.onClick.AddListener(() => GameController.instance.menuNav.GoToScreen("Mode_Select"));

		//get a reference to the attached rectTransform
		RectTransform rect = myGO.GetComponent<RectTransform> ();

		//set the size of the preview
		rect.sizeDelta = new Vector2 (previewWidth, previewHeight);

		//set the anchor point to middle
		rect.anchorMin = new Vector2 (0.5f, 0.5f);
		rect.anchorMax = new Vector2 (0.5f, 0.5f);

        //set the position of the preview
        rect.anchoredPosition = new Vector2(
            (index/* % previewsPerLine*/) * previewWidth + (rect.sizeDelta.x / 2) + (spacing * index) - previewWidth/2, 0);

        //rect.anchoredPosition = new Vector2 (
		//	(index%previewsPerLine) * previewWidth + (rect.sizeDelta.x/2) + ((index % previewsPerLine) * spacing), 
		//	-(index/previewsPerLine) * previewHeight - (rect.sizeDelta.y/2) - ((index / previewsPerLine) * spacing));

		//add the rect to the navigation
		nav.menuOptions [index] = rect;

        
	}

    /*public void SetLerpParameters(int _index) {

        nav.lerpStartPoint = rectTransform.localPosition;
        nav.lerpEndPoint = new Vector2(-(defaultPreviewWidth + spacing) * _index, -360);

        for (int i = 0; i < nav.menuOptions.Length; i++) {
            nav.menuOptions[i].localScale = nav.defaultImageScale;
        }
    }*/

    public void SetPosition(int _index) {

        rectTransform.localPosition = new Vector2(-(defaultPreviewWidth + spacing) * _index, -360);

    }

    public void GrowImage(int _index) {

        for(int i = 0; i < nav.menuOptions.Length; i++) {

            if(i == _index)
                nav.menuOptions[i].localScale = nav.largeImageScale;
            else
                nav.menuOptions[i].localScale = nav.defaultImageScale;

        }

    }

    //instantiates a level
    public void LoadLevel (int index) {
		//Instantiate <Level> (levels[index].levelPrefab);
		GameController.instance.game.SetLevel (levels[index].levelPrefab);
	}

}
