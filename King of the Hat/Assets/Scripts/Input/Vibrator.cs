using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

[RequireComponent (typeof (PlayerInput))]
public class Vibrator : MonoBehaviour {

	private Rewired.Player player; //the rewired player

	// Use this for initialization
	void Start () {
		player = GetComponent<PlayerInput> ().input.GetController ();
	}
	
	public virtual void Vibrate (float rightMotorValue, float leftMotorValue, float vibrationTime) {

		return;

		foreach(Joystick j in player.controllers.Joysticks) {

			if(!j.supportsVibration) continue;
			j.SetVibration(0, leftMotorValue, vibrationTime); // 1 second duration
			j.SetVibration(1, rightMotorValue, vibrationTime); // 1 second duration

		}

	}

}
