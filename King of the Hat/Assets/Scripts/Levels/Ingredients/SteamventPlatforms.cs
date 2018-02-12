using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamventPlatforms : MonoBehaviour {

    public ParticleSystem steamvent;
    public bool raised = false;

    public Vector3 raisedPosition;
    public Vector3 loweredPosition;

    Vector3 originalPosition;

    bool lerping = false;
    float lerpTimer = 0f;
    float lerpSpeed = 3f;

	void Start () {
        ToggleSteam();
        transform.localPosition = (raised) ? raisedPosition : loweredPosition;
        lerping = false;
    }

    void Update () {
		
        if(lerping) {

            lerpTimer += Time.deltaTime * lerpSpeed;    

            if(raised) {

                transform.localPosition = Vector3.Lerp(loweredPosition, raisedPosition, lerpTimer);

            } else {

                transform.localPosition = Vector3.Lerp(raisedPosition, loweredPosition, lerpTimer);

            }

            if (lerpTimer >= 1) {
                lerping = false;
                lerpTimer = 0;
            }

        }

	}

    public void TriggerPlatform() {
        raised = !raised;

        ToggleSteam();

        lerping = true;

    }

    void ToggleSteam() {
        ParticleSystem.EmissionModule em = steamvent.emission;
        em.enabled = raised;
    }

    void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        float size = .3f;

        Vector3 loweredGizmoPosition = (Application.isPlaying) ? loweredPosition : loweredPosition + transform.position;
        Gizmos.DrawCube(loweredGizmoPosition, Vector3.one * size);

        Vector3 raiseGizmoPosition = (Application.isPlaying) ? raisedPosition : raisedPosition + transform.position;
        Gizmos.DrawCube(raiseGizmoPosition, Vector3.one * size);

    }

}
