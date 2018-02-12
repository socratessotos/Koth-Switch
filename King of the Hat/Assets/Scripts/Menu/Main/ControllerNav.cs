using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ControllerNav : MonoBehaviour {

	public RectTransform[] menuOptions;
	public RectTransform selector;
	public int selectorPadding;
	public int optionsPerRow;
    public float scrollRate = 0.2f;

    protected float scrollTimer;
	public int index = 0;
	protected Vector2 input;

    protected bool lerping = false;

    // Use this for initialization
    void Start () {
		MoveSelectionTo (index);

        scrollTimer = Time.time;
	}

	// Update is called once per frame
	void Update () {

		input = Vector2.zero;

        if(lerping) {

            LerpToNextOption();

            if (Time.time > scrollTimer) {
                lerping = false;
            }

        }

        if (Input.GetKeyDown(KeyCode.Return)) {

            menuOptions[index].GetComponent<Button>().onClick.Invoke();

        }

        for (int i = 0, l = GameController.instance.playerInputs.Length; i < l; i++) {

            if (GameController.instance.playerInputs[i].snapshot.aButton.down) {
                menuOptions[index].GetComponent<Button>().onClick.Invoke();
            }

            if (GameController.instance.playerInputs[i].snapshot.bButton.down) {
                BackButtonPress();
            }

            if (GameController.instance.playerInputs[i].snapshot.HorizontalPressed() && !lerping) {
                input.x = GameController.instance.playerInputs[i].snapshot.xAxis;
            }

            if (GameController.instance.playerInputs[i].snapshot.VerticalPressed() && !lerping) {
                input.y = -GameController.instance.playerInputs[i].snapshot.yAxis;
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
		
	}

	public void MoveSelectionTo (int _index) {
        if (selector == null)
            return;

		selector.anchoredPosition = menuOptions[_index].anchoredPosition;
		selector.sizeDelta = menuOptions[_index].sizeDelta + Vector2.one * selectorPadding;
	}

    public virtual void Move (Vector2 _input) {

		if (Mathf.Abs(_input.x) > 0.5f)
			return;

		index += (int) Mathf.Sign(_input.y);

		if (index < 0) index = 0;
		if (index > menuOptions.Length - 1) index = menuOptions.Length - 1;

		MoveSelectionTo (index);

    }

    public virtual void MoveToindex(int _index) {

    }

    public virtual void LerpToNextOption() {

    }

	/*public void Move (Vector2 _input) {

        index += (int) Mathf.Sign(_input.y);

        if (index < 0) index = menuOptions.Length - 1;
		if (index > menuOptions.Length - 1) index = 0;

        MoveSelectionTo (index);

	}*/

	public virtual void BackButtonPress () {
		
	}

}