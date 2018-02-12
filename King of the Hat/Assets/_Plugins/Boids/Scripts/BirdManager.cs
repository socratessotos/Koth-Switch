using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdManager : MonoBehaviour {

	//constants at the top is easier to manage
	public const float MOVE_RATIO = 6.0f / 100.0f;
	public static Vector2 DEFAULT_SKIN_WIDTH = new Vector2 (0.2f, 0.2f);

	public Bird birdPrefab;
	public LayerMask birdFrightMask;

	[MinMaxRangeAttribute(0, 10)]
	public MinMaxRange birdRange;

	public float birdWaitTimeMin, birdWaitTimeAdd;
	public float birdDirectionChangeChance;
	public float birdOverlapDistance;
	public float birdFrightDistance;
	public float birdFleeRadius;
	public float birdMaxFleeTime;

	[HideInInspector]
	public Bounds bounds;

	private List<Bird> birds;
	private int birdCount;

	void Awake () {

		bounds = GetComponent<BoxCollider2D> ().bounds;

		birdCount = (int) birdRange.GetRandomValue ();
		birds = new List <Bird> ();

		for (int i = 0; i < birdCount; i++) {

			Bird b = Instantiate <Bird> (birdPrefab);
			b.transform.position = getRandomPos (b.transform.localScale/2);
			b.manager = this;

			addBird (b);

		}

	}

	public Vector3 getRandomPos (Vector2 halfScale) {

		Vector3 newPos = new Vector3 ();

		newPos.x = bounds.min.x + halfScale.x + Random.value * ((bounds.max.x - halfScale.x) - bounds.min.x);
		newPos.y = bounds.max.y;
		newPos.z = 0;

		return newPos;

	}
		
	public bool isBirdThere (Bird bird, float targetPos) {
	
		foreach (Bird b in birds) {

			if (b == bird)
				continue;

			float distance = Mathf.Abs (b.transform.position.x - targetPos);
			if (distance < birdOverlapDistance)
				return true;
			
		}

		return false;

	}

	public BirdManager getNewZone () {
		BirdManager[] bm = GameObject.FindObjectsOfType<BirdManager> ();

		if (bm.Length == 1)
			return null;

		int thiss = 0, rand;

		for (int i = 0; i < bm.Length; i++) {
			if (bm [i] == this)
				thiss = i;
		}

		do {
			rand = Random.Range (0, bm.Length);
		} while (rand == thiss);

		return bm[rand];

	}

	public void addBird (Bird b) {

		birds.Add (b);
		b.transform.parent = transform;

	}

	public void removeBird (Bird b) {

		birds.Remove (b);
		b.transform.parent = null;

	}
		
}
