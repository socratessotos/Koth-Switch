using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedBoostAbility : OnPickUpAbilities {

    public float moveSpeedIncrease = 3f;
    public float speedIncreaseTime = 2f;
    float speedIncreaseTimer;
    bool boosted;

    public string visualEffect;

    public void BoostSpeed() {

        if (boosted) {
            speedIncreaseTimer = Time.time + speedIncreaseTime;
            return;
        }

        boosted = true;
        speedIncreaseTimer = Time.time + speedIncreaseTime;

        player.moveSpeed += moveSpeedIncrease;
        player.airMoveSpeed += moveSpeedIncrease;

        PlayBoostedEffect();
        
    }

    void Update() {

        if(boosted) {
            if (Time.time > speedIncreaseTimer) {
                ResetSpeedToNormal();
            }
        }
        
    }

    void ResetSpeedToNormal() {
        boosted = false;
        //reset player speed here
        player.moveSpeed -= moveSpeedIncrease;
        player.airMoveSpeed -= moveSpeedIncrease;
    }

    void PlayBoostedEffect() {
        var steamEmitParams = new ParticleSystem.EmitParams();

        steamEmitParams.rotation3D = new Vector3(0, 0, -80);
        VFXManager.instance.EmitAtPosition(visualEffect, steamEmitParams, 1, Vector3.up * 0.1f + Vector3.left * 0.7f, player.playerIndex, 1, false, player.transform);

        steamEmitParams.rotation3D = new Vector3(0, 0, 80);
        VFXManager.instance.EmitAtPosition(visualEffect, steamEmitParams, 1, Vector3.up * 0.1f + Vector3.right * 0.7f, player.playerIndex, 1, false, player.transform);

    }

    protected override void Reset() {
        base.Reset();

        speedIncreaseTimer = Time.time;
        boosted = false;
    }

}
