using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleThrowAbility : RepeatedClickAbilities {

    public override void OnNextClick() {
        base.OnNextClick();

        if (currentClickCount <= numberOfExtraClicksAllowed)
            NextThrow();

    }

    void NextThrow() {
        hat.Throw(hat.owner.GetDirectionalInput(), 0);
    }

    public override void OnRefreshAbility() {
        base.OnRefreshAbility();

    }

    public override void OnDisableAbility() {
        base.OnDisableAbility();

        currentClickCount = numberOfExtraClicksAllowed + 1;

    }

}
