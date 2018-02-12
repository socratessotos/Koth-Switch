using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PsychicLine : MonoBehaviour {

    public Transform owner;

    MultipleThrowAbility secondThrowAbility;
    LineRenderer line;
    bool connected = true;

    void Awake() {
        line = GetComponent<LineRenderer>();
        secondThrowAbility = GetComponent<MultipleThrowAbility>();
    }

    void Update () {
		
        if(secondThrowAbility.currentClickCount < secondThrowAbility.numberOfExtraClicksAllowed) {

            if (!line.enabled) {
                line.enabled = true;
            }

        } else {

            if (line.enabled) {
                line.enabled = false;
            }

        }

        UpdateActivePositions();

    }

    void UpdateActivePositions() {
        Vector3 ownerPosition = owner.position + Vector3.up * 1.2f;
        Vector3 hatPosition = transform.position + Vector3.up * 0.5f;
        Vector3 midPointPosition = new Vector3((ownerPosition.x + hatPosition.x) / 2, (ownerPosition.y + hatPosition.y) / 2, 0);

        line.SetPosition(0, owner.position + Vector3.up * 1.2f);
        line.SetPosition(1, midPointPosition);
        line.SetPosition(2, transform.position + Vector3.up * 0.5f);
    }

    void AnimateBreak() {
        
    }

    private void Reset() {
        
    }

}
