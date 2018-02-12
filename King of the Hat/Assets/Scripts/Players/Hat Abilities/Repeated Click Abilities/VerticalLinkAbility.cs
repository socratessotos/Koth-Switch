using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalLinkAbility : RepeatedClickAbilities {

    bool linked = false;

    void FixedUpdate() {
        
        if(linked) {
            hat.velocity = hat.owner.GetDirectionalInput() * 17 * Time.fixedDeltaTime;

            if (hat.isCurrentlyAttached || hat.isBeingAttached) linked = false;

        } else {

        }

    }

    public override void OnNextClick() {
        base.OnNextClick();

        if (!hat.isBeingThrown && onlyIfDangerous) return;

        if (currentClickCount <= numberOfExtraClicksAllowed)
            LinkToOwner();
    }

    void LinkToOwner() {
        linked = !linked;
    }

    public override void OnRefreshAbility() {
        base.OnRefreshAbility();
        linked = false;
    }

    public override void OnDisableAbility() {
        base.OnDisableAbility();
        linked = false;
    }

}
