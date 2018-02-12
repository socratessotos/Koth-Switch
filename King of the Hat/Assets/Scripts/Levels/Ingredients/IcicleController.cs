using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcicleController : RaycastController {

    public CollisionInfo collisions;
    Icicle icicle;

    public override void Start() {
        base.Start();

        icicle = GetComponent<Icicle>();
    }

    public void Move(Vector2 moveAmount, bool standingOnPlatform = false) {

        UpdateRaycastOrigins();

        collisions.Reset();
        
        if (moveAmount.y != 0) {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount, Space.World);

        //if (standingOnPlatform) {
        //    collisions.below = true;
        //}

    }
    
    void VerticalCollisions(ref Vector2 moveAmount) {

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

                    if (hits[j].transform.CompareTag("Hat")) {

                        Hat hat = hits[j].transform.GetComponent<Hat>();

                        if (!hat.isBeingThrown && Mathf.Abs(hat.velocity.y - icicle.GetVelocity().y) > 0.3f)
                            hat.KillOwner(hat.owner);

                        hat.velocity.y = icicle.GetVelocity().y;

                    }

                    if (hits[j].transform.CompareTag("Player")) {

                        Player player = hits[j].transform.GetComponent<Player>();

                        player.GetStunned(0.2f);
                        icicle.Die();

                    }

                    moveAmount.y = (hits[j].distance - skinWidth) * directionY;
                    rayLength = hits[j].distance;
                    
                    if (!hits[j].transform.CompareTag("Player") && !hits[j].transform.CompareTag("Hat") && hits[j].transform.gameObject != this.transform.gameObject) {
                        collisions.below = directionY == -1;
                    }

                }

            }

        }

    }
    

    public struct CollisionInfo {
        public bool below;

        public void Reset() {
            below = false;
        }

    }

}
