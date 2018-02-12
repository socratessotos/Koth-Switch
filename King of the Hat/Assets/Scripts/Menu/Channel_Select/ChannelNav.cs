using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ChannelNav : ControllerNav {

    ChannelInfo channelInfo;
    
    Vector3 lerpStartPoint;
    Vector3 lerpEndPoint;

    float lerpT = 0;

    public Transform channelList;

    void Start() {
        channelInfo = GetComponent<ChannelInfo>();
        InitChannelList();
        index = 0;

        Move(new Vector2(0, -1));

    }

    void LateUpdate() {
        UpdateShowReel();
    }

    void InitChannelList() {

        int numberOfChildren = channelList.childCount;

        menuOptions = new RectTransform[numberOfChildren];

        for(int i = 0; i < menuOptions.Length; i++) {           
            menuOptions[i] = channelList.GetChild(i).GetComponent<RectTransform>();

            //channelInfo.channels[i].channelNameText = channelList.GetChild(i).transform.GetChild(1).GetComponentInChildren<Text>();
            //channelInfo.channels[i].channelNameText.text = channelInfo.channels[i].channelName;

        }


    }

    public override void BackButtonPress () {
		GameController.instance.menuNav.GoToScreen ("Main");
	}

    public override void Move(Vector2 _input) {

        if (Mathf.Abs(_input.y) > 0.5f)
            return;
        
        index += (int) Mathf.Sign(_input.x);

        if (index < 0) index = 0;
        if (index > menuOptions.Length - 1) index = menuOptions.Length - 1;

        lerpT = 0;
        SetLerpParameters(index);

        showReelCounter = 0;
        showReelIndex = 0;

        channelInfo.SetImagePreview(index);
        channelInfo.SetDescription(index);

    }

    public void SetLerpParameters(int _index) {

        lerpStartPoint = selector.localPosition;
        lerpEndPoint = menuOptions[_index].localPosition + Vector3.up * 170;

    }

    public override void LerpToNextOption() {

        lerpT += Time.deltaTime;

        selector.localPosition = Vector3.Lerp(lerpStartPoint, lerpEndPoint, lerpT / scrollRate);

    }

	public void ChooseMode (ModeData data) {
        
        if(channelInfo.channels[index].channelIsLocked) {
            return;
        }

		GameController.instance.game.SetGameMode(data.mode, data.modeValue);
        GameController.instance.menuNav.GoToScreen(channelInfo.channels[index].nextMenu);

    }

    float showReelCounter = 0;
    int showReelIndex = 0;
    void UpdateShowReel() {

        showReelCounter++;
        
        if(showReelCounter % 600 == 0) {
            showReelIndex++;
            ShowNextImage();
            showReelCounter = 0;    
        }

    }

    void ShowNextImage() {
        channelInfo.SetNextImagePreview(index, showReelIndex % channelInfo.channels[index].channelPreviews.Length);
    }

}