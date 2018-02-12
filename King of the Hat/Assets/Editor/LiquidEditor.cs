using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof (Liquid))]
public class LiquidEditor : Editor {

	public override void OnInspectorGUI () {

		DrawDefaultInspector ();
		Liquid myLiquid = (Liquid) target;

		if (GUILayout.Button("Instantiate Liquid")) {
			myLiquid.InstantiateLiquid ();
		}

		if (GUILayout.Button("Destroy Liquid")) {
			myLiquid.DestroyLiquid ();
		}

	}

}
