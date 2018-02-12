using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ChannelData {

    public RectTransform channelTransform;
    
    public string channelName;
    public Text channelNameText;
    public Image channelIcon;
    public Game.Mode gameMode;
    public Sprite[] channelPreviews;
    public string nextMenu;

    [TextArea]
    public string channelGameplayDescription;

    [TextArea]
    public string channelUnlockDescription;

    public bool channelIsLocked;
    public GameObject lockIcon;

}

public class ChannelInfo : MonoBehaviour {

    [Header("Channels")]
    public ChannelData[] channels;
    [Space(5)]

    [Header("References")]
    public Image channelPreviewBox;
    public Text channelPreviewBoxTEMPKICKSTARTERLABEL;
    public Text descriptionBoxTitle;
    public Animator lockedRedFlashAnim;
    public Color unlockedDescriptionTitleColor;
    public Color lockedDescriptionTitleColor;
    public Text channelDescriptionBox;
    [Space(5)]

    [Header("Data")]
    public Material lockedMaterial;

    void OnEnable() {

        DetermineLockedContent();

    }

    public void SetImagePreview(int _index) {

        channelPreviewBox.sprite = channels[_index].channelPreviews[0];

    }

    public void SetNextImagePreview(int _index, int _counter) {
        channelPreviewBox.sprite = channels[_index].channelPreviews[_counter];
    }

    public void SetDescription(int _index) {

        string description;

        description = (channels[_index].channelIsLocked) ? channels[_index].channelUnlockDescription : channels[_index].channelGameplayDescription;

        channelDescriptionBox.text = description;

    }

    void DetermineLockedContent() {

        for(int i = 0; i < channels.Length; i++) {
            channels[i].channelIcon.material = channels[i].channelIsLocked ? lockedMaterial : null;
            channels[i].lockIcon.SetActive(channels[i].channelIsLocked ? true : false);
        }

    }

    public void AnimateRedFlashOnDescriptionBox() {

    }

}
