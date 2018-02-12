using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : Respawnable {

	public int life = 1;
	public GameObject deathEffect;
		
	void OnTriggerEnter2D (Collider2D other) {

		if (other.gameObject.CompareTag ("Hat")) {

			Hat h = other.GetComponent <Hat> ();

			if (h.isBeingThrown) {

				if (--life <= 0) {

					Instantiate (deathEffect, transform.position, Quaternion.identity);
					gameObject.SetActive (false);
					life = 1;

				}
					
			}
				
		}
			
	}

    public override void Reset() {

        gameObject.SetActive(true);

        life = 1;

    }

}
