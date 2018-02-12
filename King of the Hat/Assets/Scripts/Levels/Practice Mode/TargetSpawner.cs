using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour {

	public Target[] targetPrefabs;

	List<Target> targets = new List<Target> ();

	void Start () {

		targets.Add (CreateNewTarget ());
		targets.Add (CreateNewTarget ());
		targets.Add (CreateNewTarget ());
        targets.Add (CreateNewTarget ());
        targets.Add (CreateNewTarget ());

    }

    void Update () {

		for (int i = 0; i < targets.Count; i++) {

			//print (targets[i]);

			if (targets [i] == null)
				targets[i] = CreateNewTarget ();

		}

	}

	public Target CreateNewTarget () {

		int x = Random.RandomRange (-20, 20);
		int y = Random.RandomRange (-10, 10);

		do {

			x = Random.RandomRange (-20, 20);
			y = Random.RandomRange (-10, 10);

		} while (Physics2D.OverlapBox (new Vector2 (x, y), new Vector2 (2, 2), 0));

		Target t = Instantiate <Target> (targetPrefabs [Random.Range (0, targetPrefabs.Length)]);
		t.transform.position = new Vector3 (x, y, 0);
		t.transform.parent = this.transform;
		return t;

	}

}
