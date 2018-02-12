using UnityEngine;
using System.Collections;

[RequireComponent (typeof (Player))]
public class PlayerInput : MonoBehaviour {

	[Header("Axis Input")]
	public string horizontalAxis = "Horizontal";
	public string verticalAxis = "Vertical";
	[Space(5)]

	[Header("Button Input")]
	public KeyCode aButton = KeyCode.Space;
	public KeyCode bButton = KeyCode.X;
	[Space(5)]

	public bool receiveInput = true;

	Player player;

	[HideInInspector]
	public InputController input;
	public Player character {get{return player;}}

	void Start () {
		Init ();
	}

	void Update () {

		if (GameController.instance.game.state != Game.State.PAUSED
         && GameController.instance.game.state != Game.State.COUNTDOWN) {
			
			ReceiveControllerInput ();
			ReceiveKeyboardInput ();

			//print ("run input");

		}

	}

	public void Init () {
		if (player == null)
			player = GetComponent<Player> ();
	}

	public void ReceiveKeyboardInput () {

		if (receiveInput) {
			
			Vector2 directionalInput = new Vector2 (Input.GetAxisRaw (horizontalAxis), Input.GetAxisRaw (verticalAxis));
			player.SetDirectionalInput (directionalInput);

			if (Input.GetKeyDown (aButton)) {
				player.OnJumpInputDown ();
			}
			if (Input.GetKeyUp (aButton)) {
				player.OnJumpInputUp ();
			}

			if (Input.GetKeyDown (bButton)) {
				player.OnAbilityInputDown ();
			}

			if (Input.GetKeyUp (bButton)) {
				player.OnAbilityInputUp ();
			}

		}
		
	}

	public void ReceiveControllerInput () {

		if (input == null) return;
		
		Vector2 directionalInput = new Vector2 (input.snapshot.xAxis, input.snapshot.yAxis);
		player.SetDirectionalInput (directionalInput);

		Vector2 directionalThrowInput = new Vector2 (input.snapshot.xAxisThrow, input.snapshot.yAxisThrow);
		player.SetDirectionalThrowInput (directionalThrowInput);

		if (input.snapshot.aButton.down) {
			player.OnJumpInputDown ();
		}

		if (input.snapshot.aButton.up) {
			player.OnJumpInputUp ();
		}

		if (input.snapshot.bButton.down) {
			player.OnAbilityInputDown ();
		}

		if (input.snapshot.bButton.up) {
			player.OnAbilityInputUp ();
		}

	}

	public void SetInput (InputController _input) {
		input = _input;
	}

}
