using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatedClickAbilities : MonoBehaviour {
    
    [HideInInspector]
    public Hat hat;

    [Header("Repeated Clicks")]
    public int numberOfExtraClicksAllowed = 1;
    public int currentClickCount = 0;
    public bool onlyIfDangerous = false;

    public virtual void Start() {
        hat = GetComponent<Hat>();
    }

    public void NextClick() {
        currentClickCount++;

        OnNextClick();
    }

    public virtual void OnNextClick() { }

    public void RefreshAbility() {
        currentClickCount = 0;

        OnRefreshAbility();
    }

    public virtual void OnRefreshAbility() { }

    public void DisableAbility() {
        //currentClickCount = 10000;

        OnDisableAbility();
    }

    public virtual void OnDisableAbility() { }

}
