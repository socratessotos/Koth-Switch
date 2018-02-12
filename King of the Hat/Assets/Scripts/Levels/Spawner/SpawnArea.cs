using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnItem {

	public string groupID;
	public GameObject gameObject;
	public int chanceOfSpawning;

}

[RequireComponent (typeof (BoxCollider2D))]
public class SpawnArea : MonoBehaviour {

	public SpawnItem[] spawnables;
	public float spawnWait;

	private Bounds bounds;
	private float timeToSpawn;

	void Awake () {
		
		bounds.extents = GetComponent <BoxCollider2D> ().size/2;

	}

	void Update () {

		if (Time.time > timeToSpawn) {

			SpawnRandomObject (

				new Vector3 (
					(int) Random.Range (transform.position.x - bounds.extents.x, transform.position.x + bounds.extents.x),
					(int) Random.Range (transform.position.y - bounds.extents.y, transform.position.y + bounds.extents.y),
					0),
				
				Quaternion.identity);

			timeToSpawn = spawnWait + Time.time;

		}

	}

	public void SpawnSpecificObject (int index, Vector3 position, Quaternion rotation) {

		GameObject go = Instantiate <GameObject> (spawnables[index].gameObject, position, rotation);
		go.name = spawnables[index].gameObject.name;
		go.transform.parent = transform.parent;

	}

	public void SpawnRandomObject (Vector3 position, Quaternion rotation) {

		int r = (int) Random.value * 100;
		int chanceOfSpawning = 0;

		for (int i = 0; i < spawnables.Length; i++) {
		
			chanceOfSpawning += spawnables[i].chanceOfSpawning;

			if (r < chanceOfSpawning) {
				SpawnSpecificObject (i, position, rotation);
				break;
			}
					

		}
		
	}


}
