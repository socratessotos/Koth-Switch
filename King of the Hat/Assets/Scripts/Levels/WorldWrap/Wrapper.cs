using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (BoxCollider2D))]
public class Wrapper : MonoBehaviour {

	BoxCollider2D col;

	void Awake () {
		col = GetComponent <BoxCollider2D> ();
	}

	void OnTriggerExit2D (Collider2D other) {

		if (other.gameObject.CompareTag ("Player")) {

			Wrap (other.transform);

			Player p = other.GetComponent<Player> ();

			if (p.hat.isCurrentlyAttached) {
				p.hat.transform.position = p.transform.position;
			}

            PlayerAnimationVFXController anim = p.GetComponentInChildren<PlayerAnimationVFXController>();
            anim.ToggleFastFallTrail(true);

		} 

		else if (other.gameObject.CompareTag ("Hat")) {

			Hat h = other.GetComponent<Hat> ();

			Wrap (h.transform);

			if (h.isCurrentlyAttached) {
				h.owner.transform.position = h.transform.position;
			}
				

		}

		CameraFX.instance.ShakeCamera (0.1f, 0.05f, 1f);
		
	}

	public void Wrap (Transform t) {


		if (t.position.x <= col.bounds.min.x + 1) {

			t.position = new Vector3 (col.bounds.max.x - 1, t.position.y, t.position.z);

			//print ("yo1");
		}

		else if (t.position.x >= col.bounds.max.x - 1) {

			t.position = new Vector3 (col.bounds.min.x + 1, t.position.y, t.position.z);

			//print ("yo2");
		}

		else if (t.position.y <= col.bounds.min.y + 1) {

			t.position = new Vector3 (t.position.x, col.bounds.max.y - 1, t.position.z);

			//print ("yo3");
		}

		else if (t.position.y >= col.bounds.max.y - 1) {

			t.position = new Vector3 (t.position.x, col.bounds.min.y + 1, t.position.z);

            if (t.CompareTag("Hat")) {
                t.GetComponent<Hat>().initialThrowY = col.bounds.min.y + 1;
            }

            //print ("yo4");
        }

	}

}
