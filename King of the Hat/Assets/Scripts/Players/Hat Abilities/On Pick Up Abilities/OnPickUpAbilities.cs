using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnPickUpAbilities : MonoBehaviour {

    [HideInInspector]
    public Player player;

    public virtual void ActivatePickUpAbility() {

    }

    protected virtual void Reset() {
        player = GetComponent<Player>();
    }

}
