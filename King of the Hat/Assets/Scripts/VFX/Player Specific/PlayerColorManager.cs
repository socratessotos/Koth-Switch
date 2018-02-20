using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerColorInfo {
    
    public Color32 hatGlowColor;
    public Gradient hatFireColor;
    public Gradient hatLoseLifeColor;
    public Color32 playerColor;
    public Gradient hatTrailColor;

}

public class PlayerColorManager : MonoBehaviour {

    public static PlayerColorManager instance;

    public PlayerColorInfo[] players;

    void Awake() {

        instance = this;

    }

}
