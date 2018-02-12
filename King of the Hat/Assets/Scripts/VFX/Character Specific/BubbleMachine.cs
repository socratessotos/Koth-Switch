using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMachine : MonoBehaviour {
    
    //also do a directional thing by getting player's velocity

    PlayerAnimationVFXController animController;
    string currentState;
    int currentHash;

    int animCounter = 0;

    static int idle;

    void Awake() {
        animController = GetComponent<PlayerAnimationVFXController>();

    }

    void FixedUpdate() {

        int nameHash = animController.anim.GetCurrentAnimatorStateInfo(0).fullPathHash;

        ConstantBubbles();

        if(currentHash != nameHash) {
            currentHash = nameHash;

            BurstBubbles();
        }


        if (animCounter++ > 1000)
            animCounter = 0;

    }

    void ConstantBubbles() {

        int _count = 0;


        if (animController.state == PlayerAnimationVFXController.AnimState.FAST_FALL) {
            _count = 5;
        } 


        if (animCounter % 3 == 0) {
            if (animController.state == PlayerAnimationVFXController.AnimState.FALL) {
                _count = 1;
            }
        }

        if (animCounter % 10 == 0) {

            if (animController.state == PlayerAnimationVFXController.AnimState.IDLE) {
                _count = 1;
            } else if (animController.state == PlayerAnimationVFXController.AnimState.RUN) {
                _count = 3;
            } 

        }

        VFXManager.instance.EmitAtPosition("Washing_Machine_Bubbles", _count, transform.position, true);

    }

    public void BurstBubbles() {

        int _count = 0;

        if (animController.state == PlayerAnimationVFXController.AnimState.JUMP) {
            _count = 20;
        } else if (animController.state == PlayerAnimationVFXController.AnimState.LAND) {
            _count = 30;
        }

        VFXManager.instance.EmitAtPosition("Washing_Machine_Bubbles", _count, transform.position, true);

    }

}
