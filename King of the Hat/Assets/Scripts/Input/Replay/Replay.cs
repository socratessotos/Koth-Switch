using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Replay {

	public Vector3 initialPosition;
	public Stack <InputSnapshot> inputs;

	public Replay (Vector3 _initialPosition) {

		initialPosition = _initialPosition;
		inputs = new Stack <InputSnapshot> ();

	}

	public void AddInput (InputSnapshot frameInput) {
		InputSnapshot nshot = new InputSnapshot (frameInput);

		inputs.Push (nshot);
	} 

	public InputSnapshot GetNextInput () {

		if (inputs.Count > 0) return inputs.Pop ();
		else return new InputSnapshot ();

	} 

	public InputSnapshot CheckNextInput () {
		return inputs.Peek ();
	} 

}
