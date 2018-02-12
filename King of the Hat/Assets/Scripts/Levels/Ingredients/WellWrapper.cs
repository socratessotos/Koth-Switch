using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class WellWrapper : MonoBehaviour {

    public GameObject exit;

    void OnTriggerEnter2D(Collider2D other) {
        
        if (other.gameObject.CompareTag("Player")) {

            if (other.GetComponent<Player>().GetVelocity().y >= 0) return;

            Wrap(other.transform);

            Player p = other.GetComponent<Player>();

            if (p.hat.isCurrentlyAttached) {
                p.hat.transform.position = p.transform.position;
            }

            PlayerAnimationVFXController anim = p.GetComponentInChildren<PlayerAnimationVFXController>();
            anim.ToggleFastFallTrail(true);

        } else if (other.gameObject.CompareTag("Hat")) {

            Hat h = other.GetComponent<Hat>();

            if (other.GetComponent<Hat>().velocity.y >= 0) return;

            Wrap(h.transform);

            if (h.isCurrentlyAttached) {
                h.owner.transform.position = h.transform.position;
            } else {
                h.initialThrowY = exit.transform.position.y;
            }


        }

        CameraFX.instance.ShakeCamera(0.1f, 0.05f, 1f);
        
    }

    public void Wrap(Transform t) {

        t.position = exit.transform.position + Vector3.up;

        if(t.CompareTag("Player")) {
            t.GetComponent<Controller2D>().collisions.shouldBounceOutOfWell = true;
        } else if(t.CompareTag("Hat")) {
            Hat hat = t.GetComponent<Hat>();

            if (hat.isBeingThrown)
                hat.velocity = new Vector2(0, Mathf.Abs(hat.velocity.y));
            else
                hat.velocity = new Vector2(0, 0.15f);
        }

    }

}
