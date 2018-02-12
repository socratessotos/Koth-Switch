using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class IcicleController2 : RaycastController {

    public CollisionInfo collisions;
    Icicle icicle;

    public override void Start() {
        base.Start();

        //remove the the current layer from collisions
        //collisionMask = collisionMask & ~(1 << gameObject.layer);
        icicle = GetComponent<Icicle>();

    }

    public void Move(Vector2 moveAmount, bool standingOnPlatform = false) {

        UpdateRaycastOrigins();

        collisions.Reset();
        collisions.moveAmountOld = moveAmount;

        if (moveAmount.x != 0) {
            collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
        }

        HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0) {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount, Space.World);

    }

    void HorizontalCollisions(ref Vector2 moveAmount) {

    }

    void VerticalCollisions(ref Vector2 moveAmount) {

        //if (hat.collisionSkip) return;

        Vector2 oldVelocity = icicle.GetVelocity();

        float directionY = Mathf.Sign(moveAmount.y);
        float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {

            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            for (int j = 0; j < hits.Length; j++) {

                if (hits[j]) {

                    if (hits[j].transform.gameObject == transform.gameObject) continue;    

                    moveAmount.y = (hits[j].distance - skinWidth) * directionY;
                    rayLength = hits[j].distance;

                    if (!hits[j].collider.CompareTag("Hat") && !hits[j].collider.CompareTag("Player") && hits[j].transform.gameObject != transform.gameObject) {
                        collisions.below = directionY == -1;
                        collisions.above = directionY == 1;
                    } else {
                        //hat.velocity.y = -hat.velocity.y / 2;
                    }

                }

            }

        }

        if (collisions.shouldReflect) collisions.shouldReflectY = false;

    }

    public struct CollisionInfo {
        public bool above, below;
        public bool left, right;

        public bool isHittingPlayer;
        public bool isHittingPlayerY;
        public bool shouldReflect;
        public bool shouldReflectY;
        public bool hitHat;

        public Vector2 moveAmountOld;
        public int faceDir;
        public bool fallingThroughPlatform;
        public bool hasKilledAPlayer;

        public void Reset() {

            above = below = false;
            left = right = false;
            shouldReflect = false;
            isHittingPlayer = false;

            shouldReflectY = false;
            isHittingPlayerY = false;

        }

    }

}
