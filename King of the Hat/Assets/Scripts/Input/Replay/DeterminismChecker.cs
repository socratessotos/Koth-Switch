using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeterminismChecker : MonoBehaviour {

	public PlayerInput player1;
	//public PlayerInput player2;

	InputController input1;
	//InputController input2;

	Replay replayPlayer1;
	//Replay replayPlayer2;

	bool isRecording;
	bool isPlaying;

	// Update is called once per frame
	void Update () {


		if (Input.GetKeyDown(KeyCode.Q)) {

			if (isPlaying) return;

			isRecording = !isRecording;

			if (isRecording) {

				StartRecording ();

			} else {

				StopRecording ();
				
			}

		}


		if (isRecording) {

			replayPlayer1.AddInput (input1.snapshot);
			//replayPlayer2.AddInput (input2.snapshot);

		}


		if (Input.GetKeyDown(KeyCode.W)) {

			if (isRecording) return;

			isPlaying = !isPlaying;

			if (isPlaying) {

				PlayReplay ();

			} else {

				print ("playing stopped");

			}

		}

		if (isPlaying) {

			input1.OverwriteInput (replayPlayer1.GetNextInput());
			//input2.OverwriteInput (replayPlayer2.GetNextInput());

			//print ("input overwrite");
			
		}


	}

	public void StartRecording () {

		input1 = player1.input;
		//input2 = player2.input;

		replayPlayer1 = new Replay (player1.transform.position);
		//replayPlayer2 = new Replay (player2.transform.position);

		print ("recording started");

	}

	public void StopRecording () {

		print ("recording stopped");

	}

	public void PlayReplay () {

		player1.transform.position = replayPlayer1.initialPosition;
		//player2.transform.position = replayPlayer2.initialPosition;

		print ("play started");
		
	}

}
