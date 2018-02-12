using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundDestroyer : MonoBehaviour {

	void OnTriggerExit2D (Collider2D other) {

		if (!other.CompareTag ("Player") && !other.CompareTag ("Hat")) {

			Destroy (other.gameObject);

		}

	}

}
