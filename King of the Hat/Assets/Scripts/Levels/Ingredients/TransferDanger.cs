using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransferDanger : MonoBehaviour {

    public bool isDangerous = false;
    public Vector2 velocity = Vector2.zero;
    public Vector2 lastVelocity = Vector2.zero;

    public GameObject currentFire;
    [HideInInspector]
    public ParticleSystem dangerFire;
    [HideInInspector]
    public ParticleSystem dangerFireFront;

    public virtual void Transfer(Vector2 direction) {

        isDangerous = true;

    }

}
