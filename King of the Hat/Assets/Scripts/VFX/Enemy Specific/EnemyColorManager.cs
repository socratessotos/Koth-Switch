using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyColorInfo {
    
    public Color32 hatGlowColor;
    public Gradient hatFireColor;
    public Gradient hatLoseLifeColor;
    public Color32 enemyColor;

}

public class EnemyColorManager : MonoBehaviour {

    public static EnemyColorManager instance;

    public EnemyColorInfo[] enemies;

    void Awake() {

        instance = this;

    }

}
