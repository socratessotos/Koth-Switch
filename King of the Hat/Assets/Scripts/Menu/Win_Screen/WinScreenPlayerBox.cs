using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinScreenPlayerBox : MonoBehaviour {

    enum State { ENTERING, STATS, READY, EXITING }

    State currentState = State.ENTERING;

    //[HideInInspector]
    public InputController input;

    [Header ("Badges")]
    public GameObject[] badges;
    [Space (10)]

    [Header ("Preview")]
    public Image charImage;
    public Image characterPreview;
    public Animator characterPreviewAnimator;
    [Space(10)]

    [Header("Stats")]
    public Text killsValue;
    public Text deathsValue;
    public Text throwsValue;
    public Text accuracyValue;
    [Space(10)]

    [Header("Continue Text")]
    public Text pressAText;
    [Space(10)]

    [Header("Animators")]
    public GameObject banner;
    public Animator pressATextAnim;
    public GameObject readyText;
    public GameObject playerBox;
    public GameObject stats;
    [Space(10)]

    Animator anim;

    ScrollRect scrollRect;

    void Awake() {
        anim = GetComponent<Animator>();
        scrollRect = GetComponent<ScrollRect>();
    }

    public void Init() {
        ToggleStatsOff();
        ToggleReadyTextOff();
        TogglePressATextOff();
        SetCharacterPreview();

        gameObject.SetActive(true);
        currentState = State.ENTERING;
    }

    void SetCharacterPreview() {
        characterPreview.sprite = charImage.sprite;
        characterPreview.SetNativeSize();
        characterPreview.rectTransform.sizeDelta *= 4;

        characterPreviewAnimator.runtimeAnimatorController = charImage.GetComponent<Animator>().runtimeAnimatorController;
        characterPreviewAnimator.SetTrigger("Not Ready");
    }

    public void SetStats(int _kills, int _deaths, int _throws, int _accuracy) {
        killsValue.text = _kills.ToString();
        deathsValue.text = _deaths.ToString();
        throwsValue.text = _throws.ToString();
        accuracyValue.text = _accuracy.ToString() + "%";
    }

    void Update() {

        if (input == null)  
            return;

        if(input.snapshot.yAxis != 0 && currentState == State.STATS) {

            scrollRect.velocity = Vector2.up * input.snapshot.yAxis * 500;

        }

        if(input.snapshot.aButton.down) {
            
            switch(currentState) {
                case State.ENTERING:
                    break;                
                case State.STATS:                    
                    ToggleReadyTextOn();
                    TogglePressATextOff();
                    anim.SetTrigger("Ready");
                    currentState = State.READY;
                    break;
                case State.READY:
                    break;
                case State.EXITING:
                    break;
                default:
                    break;
            }

        }

        if(input.snapshot.bButton.down) {

            if(currentState == State.READY) {
                //go back to stats
                ToggleReadyTextOff();
                TogglePressATextOn();
                anim.SetTrigger("UnReady");
                currentState = State.STATS;
            }

        }

    }

    public bool IsReady() {
        if (currentState == State.READY)
            return true;
        else
            return false;
    }

    public bool IsExiting() {
        if (currentState == State.EXITING)
            return true;
        else
            return false;
    }

    public void AssignBadge(int badgeIndex) {

        for(int i = 0; i < badges.Length; i++) {

            if (i == badgeIndex)
                badges[i].SetActive(true);
            else
                badges[i].SetActive(false);

        }

    }

    void TogglePressATextOn() {
        if (currentState != State.READY)
            pressAText.gameObject.SetActive(true);
    }

    void TogglePressATextOff() {
        pressAText.gameObject.SetActive(false);
    }

    void ToggleReadyTextOn() {
        readyText.SetActive(true);
    }

    void ToggleReadyTextOff() {
        readyText.SetActive(false);
    }

    void ToggleStatsOff() {
        stats.SetActive(false);
    }

    void ToggleStatsOn() {
        stats.SetActive(true);
    }

    void SetStatsPosition(int _x, int _y) {
        stats.GetComponent<RectTransform>().localPosition = new Vector2(_x, _y);
    }

    public void SetStateToStats() {
        currentState = State.STATS;

        SetStatsPosition(0, -444);

        anim.SetTrigger("EnterToStats");
        Invoke("TogglePressATextOn", 0.2f);
    }

    public void SetStateToExiting() {
        
        currentState = State.EXITING;

        characterPreviewAnimator.SetTrigger("Ready");

    }

    public void ReadyUp() {
        ToggleReadyTextOn();
        TogglePressATextOff();
        anim.SetTrigger("Ready");
        currentState = State.READY;
    }

}