using UnityEngine;

public class InputSnapshot {

	public float xAxis;
	public float yAxis;
	public float xAxisThrow;
	public float yAxisThrow;
	public ButtonSnapshot aButton;
	public ButtonSnapshot bButton;
	public ButtonSnapshot selectButton;
	public ButtonSnapshot startButton;

	public InputSnapshot () {
		
	}

	public InputSnapshot (InputSnapshot copy) {
		
		xAxis = copy.xAxis;
		yAxis = copy.yAxis;
		xAxisThrow = copy.xAxisThrow;
		yAxisThrow = copy.yAxisThrow;
		aButton = copy.aButton;
		bButton = copy.bButton;

	}

	public bool HorizontalPressed () {
		if (Mathf.Abs(xAxis) > 0.2f && xAxis != lastXAxis)
			return true;

		return false;
	}

	public bool VerticalPressed () {
		if (Mathf.Abs(yAxis) > 0.2f && yAxis != lastYAxis)
			return true;

		return false;
	}

	public float lastXAxis;
	public float lastYAxis;
	public bool isAddedToGame;

	public void Reset () {

		lastXAxis = xAxis;
		lastYAxis = yAxis;

		xAxis = 0.0f;
		yAxis = 0.0f;

		xAxisThrow = 0.0f;
		yAxisThrow = 0.0f;

		aButton.Reset ();
		bButton.Reset ();
		selectButton.Reset ();
		startButton.Reset ();

	}

}

public struct ButtonSnapshot {
	public bool down;
	public bool pressed;
	public bool up;

	public ButtonSnapshot (ButtonSnapshot copy) {

		down = copy.down;
		pressed = copy.pressed;
		up = copy.up;

	}

	public void Reset () {
		down = false;
		up = false;
	}

}
