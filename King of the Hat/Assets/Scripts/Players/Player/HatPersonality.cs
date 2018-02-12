using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatPersonality : MonoBehaviour {

    public bool dancesInTheYAxis = false;
    public AnimationCurve yAxisAnimationCurve;
    public float maxHeight = 1f;
    public float yTime = 1f;
    float initialY;

    bool dancing = false;

    Hat hat;
    HatController hatController;

	void Start () {
        hat = GetComponent<Hat>();
        hatController = GetComponent<HatController>();
    }

    void Update() {

        if ((!hat.isCurrentlyAttached && !hat.isBeingAttached && !hat.isBeingThrown)
        && hatController.collisions.below
        && !dancing) {
            StartDancing();
        }

    }

    public void StartDancing() {
        if (dancesInTheYAxis) {
            StopCoroutine("YDance");
            StartCoroutine("YDance");
        }
    }

    public void StopDancing() {
        dancing = false;

        if(dancesInTheYAxis) StopCoroutine("YDance");
    }

    IEnumerator YDance() {

        dancing = true;

        Vector3 initialPositon = transform.position;
        Vector3 peakPosition = transform.position + Vector3.up * maxHeight;

        float timer = 0;

        while(timer <= yTime) {

            transform.position = Vector3.Lerp(initialPositon, peakPosition, yAxisAnimationCurve.Evaluate(timer / yTime));
            timer += Time.deltaTime;
            yield return null;

        }

        if(!hat.isCurrentlyAttached && !hat.isBeingAttached && !hat.isBeingThrown) {
            StartDancing();
        }

    }

}
