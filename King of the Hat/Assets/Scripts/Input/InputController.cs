using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class InputController : MonoBehaviour {

	public int playerId = 0; // The Rewired player id of this character

	[Header("Axis Input")]
	public string horizontalAxis = "Horizontal";
	public string verticalAxis = "Vertical";
	[Space(5)]

	[Header("Button Input")]
	public string aButton = "AButton";
	public string bButton = "BButton";
	public string startButton = "StartButton";
	public string selectButton = "SelectButton";

	//struct that stores a current snapshot of the state of the game's input
	public InputSnapshot snapshot;

	private Rewired.Player player; //the rewired player

	// Use this for initialization
	protected void Awake () {

		// Get the Rewired Player object for this player and keep it for the duration of the character's lifetime
		player = ReInput.players.GetPlayer(playerId);

		//initialize the snapshot
		snapshot = new InputSnapshot ();
		
	}
	
	// Update is called once per frame
	void Update () {
		UpdateInputSnapshot ();

	}

	// Update is called once per frame
	protected void LateUpdate () {
		snapshot.Reset ();
	}

	public virtual void UpdateInputSnapshot () {
		
		snapshot.xAxis = player.GetAxisRaw("Move Horizontal");
		snapshot.yAxis = player.GetAxisRaw("Move Vertical");

		snapshot.xAxisThrow = player.GetAxisRaw("Throw Stick Horizontal");
		snapshot.yAxisThrow = player.GetAxisRaw("Throw Stick Vertical");

		//snapshot.xAxis = ConstrainAxisTo16Angles (snapshot.xAxis);
		//snapshot.yAxis = ConstrainAxisTo16Angles (snapshot.yAxis);

		//snapshot.xAxis *= 2f;
		//snapshot.xAxis = ConstrainAxisTo16Angles (snapshot.xAxis);
		//snapshot.xAxis /= 2f;

		//snapshot.yAxis *= 2f;
		//snapshot.yAxis = ConstrainAxisTo16Angles (snapshot.yAxis);
		//snapshot.yAxis /= 2f;

		if (player.GetButtonDown ("Jump")) {
            JumpButtonDown();
		}

		if (player.GetButtonUp ("Jump")) {
            JumpButtonUp();
        }

		if (player.GetButtonDown ("Throw")) {
            ThrowButtonDown();
		}

		if (player.GetButtonUp ("Throw")) {
            ThrowButtonUp();
		}

		if (player.GetButtonDown ("Start")) {
            StartButtonDown();
        }

		if (player.GetButtonUp ("Start")) {
            StartButtonUp();
        }

//		if (Input.GetButtonDown (selectButton)) {
//			snapshot.selectButton.down = true;
//			snapshot.selectButton.pressed = true;
//		}
//
//		if (Input.GetButtonUp (selectButton)) {
//			snapshot.selectButton.up = true;
//			snapshot.selectButton.pressed = false;
//		}

	}

    public void JumpButtonDown() {
        snapshot.aButton.down = true;
        snapshot.aButton.pressed = true;
    }

    public void JumpButtonUp() {
        snapshot.aButton.up = true;
        snapshot.aButton.pressed = false;
    }

    public void ThrowButtonDown() {
        snapshot.bButton.down = true;
        snapshot.bButton.pressed = true;
    }

    public void ThrowButtonUp() {
        snapshot.bButton.up = true;
        snapshot.bButton.pressed = false;
    }

    public void StartButtonDown() {
        snapshot.startButton.down = true;
        snapshot.startButton.pressed = true;
    }

    public void StartButtonUp() {
        snapshot.startButton.up = true;
        snapshot.startButton.pressed = false;
    }

    public void OverwriteInput (InputSnapshot input) {
		
		snapshot = input;

	}

	public static float ConstrainAxisTo16Angles (float axis) {

        float x1 = 0.3f;
        float x2 = 0.6f;
        float x3 = 1f;

        #if UNITY_SWITCH
        x1 = 0.4f;
        x2 = 0.6f;
        x3 = 1f;
        #endif

		if (axis >= 0) {

			if (axis <= x1) {
				axis = 0;
			} else if (axis <= x2) {
				axis = axis;
			} else {
				axis = x3;
			}


		} else {

			if (axis >= -x1) {
				axis = 0;
			} else if (axis >= -x2){
				axis = axis;
			} else {
				axis = -x3;
			}

		}

		return axis;
		
	}

	public Rewired.Player GetController () {
		return player;
	}
		
}
