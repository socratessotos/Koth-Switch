using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnDangerFinishAbilities : MonoBehaviour {

    [HideInInspector]
    public Hat hat;
    
    public virtual void ActivateOnDangerFinishAbility() {
        
    }

    protected virtual void Reset() {
        hat = GetComponent<Hat>();
    }

}
