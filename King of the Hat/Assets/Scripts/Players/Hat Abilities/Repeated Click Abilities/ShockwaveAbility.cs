using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockwaveAbility : RepeatedClickAbilities {

    [Header("Shockwave")]
    public LayerMask collisionMask;
    public float blastRadius = 5f;

    public override void OnNextClick() {
        base.OnNextClick();

        if (!hat.isBeingThrown && onlyIfDangerous) return;

        if (currentClickCount <= numberOfExtraClicksAllowed)
            Shockwave();
    }

    public void Shockwave() {
        hat.EndThrow();

        CircleOverlap();

        VFXManager.instance.EmitAtPosition("Hat_Shockwave", 1, transform.position + Vector3.up * 0.5f, false);

    }

    void CircleOverlap() {
        collisionMask = collisionMask & ~(1 << gameObject.layer);

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position + Vector3.up * 0.5f, blastRadius, collisionMask);

        for (int i = 0; i < hits.Length; i++) {

            if (hits[i]) {

                if (hits[i].transform.CompareTag("Player")) {

                    hits[i].transform.GetComponent<Player>().BlowBack((hits[i].transform.position - transform.position).normalized, 50, 0.1f);

                }

                if (hits[i].transform.CompareTag("Hat")) {

                    Hat hat = hits[i].transform.GetComponent<Hat>();

                    if (!hat.isCurrentlyAttached && !hat.isBeingAttached) {
                        hits[i].transform.GetComponent<Hat>().BlowBack((hits[i].transform.position - transform.position).normalized, 0.6f);
                    }

                }

            }

        }

    }

    public override void OnRefreshAbility() {
        base.OnRefreshAbility();

    }

    public override void OnDisableAbility() {
        base.OnDisableAbility();

    }

}
