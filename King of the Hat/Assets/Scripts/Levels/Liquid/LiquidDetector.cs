using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidDetector : MonoBehaviour {


	void OnTriggerEnter2D (Collider2D other) {

		if (other.CompareTag ("Player")) {
			
			Controller2D p = other.gameObject.GetComponent<Controller2D> ();
			if (p != null) {
				transform.parent.GetComponent<Liquid>().Splash (other.transform.position.x, 1f);
			}

			//print ("hi!");

		}

		if (other.CompareTag ("Hat")) {

			//print ("hat");
			
			HatController h = other.gameObject.GetComponent<HatController> ();
			if (h != null) {
				transform.parent.GetComponent<Liquid>().Splash (other.transform.position.x,  1);
			}	

		}

	}

	void OnTriggerExit2D (Collider2D other) {

		if (other.CompareTag ("Player")) {

			Controller2D p = other.gameObject.GetComponent<Controller2D> ();
			if (p != null) {
				transform.parent.GetComponent<Liquid>().Splash (other.transform.position.x, 0.3f);
			}

			//print ("bye!");
		}

		if (other.CompareTag ("Hat")) {

			HatController h = other.gameObject.GetComponent<HatController> ();
			if (h != null) {
				transform.parent.GetComponent<Liquid>().Splash (other.transform.position.x, 0.3f);
			}	

		}

	}

}