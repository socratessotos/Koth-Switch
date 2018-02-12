using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : RaycastController {

    public CollisionInfo collisions;
    Cube cube;

    public override void Start() {
        base.Start();

        cube = GetComponent<Cube>();
    }

    public void Move(Vector2 moveAmount, bool standingOnPlatform = false) {

        moveAmount.x = RoundToDecimal(moveAmount.x, 1);
        moveAmount.y = RoundToDecimal(moveAmount.y, 1);

        UpdateRaycastOrigins();

        collisions.Reset();

        HorizontalCollisions(ref moveAmount);
        if (moveAmount.y != 0) {
            VerticalCollisions(ref moveAmount);
        }

        transform.Translate(moveAmount, Space.World);

    }

    void HorizontalCollisions(ref Vector2 moveAmount) {

        //if (hat.collisionSkip) return;
        Vector2 oldVelocity = cube.velocity;

        float directionX = (cube.velocity.x == 0) ? 0 : collisions.faceDir;

        float rayLength = Mathf.Abs(moveAmount.x) + skinWidth;

        if (Mathf.Abs(moveAmount.x) < skinWidth) {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++) {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit) {

                if(hit.transform.gameObject == transform.gameObject) continue;

                if (hit.distance == 0) {
                    //continue;
                }

                if (hit.collider.CompareTag("Ignore")) {
                    continue;
                }

                if (hit.collider.CompareTag("Solid")) {
                    
                    if (!cube.isDangerous) cube.Break();
                    else collisions.shouldReflect = true;

                    continue;
                }              

                if (hit.collider.CompareTag("Lava")) {

                    if (cube.velocity.y < 0) cube.Break();

                    continue;

                }

                if (hit.collider.CompareTag("Player")) {

                    Player otherPlayer = hit.collider.GetComponent<Player>();

                    if(cube.isDangerous) {
                        Vector3 hitDirection = ((Vector2) otherPlayer.transform.position - hit.point).normalized;
                        otherPlayer.BlowBack(hitDirection, 40, 0.2f);                                    
                    }

                    if(cube.velocity.x > 1 || cube.velocity.x < -1)
                        cube.Break();

                }

                if (hit.collider.CompareTag("Hat")) {

                    Hat otherHat = hit.transform.GetComponent<Hat>();
                    
                    if(!otherHat.isBeingThrown) {
                        otherHat.BlowBack(otherHat.transform.position - transform.position, cube.velocity.magnitude);
                        cube.Break();
                    }

                }

                moveAmount.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1;
                collisions.right = directionX == 1;

                if ((collisions.right || collisions.left)) {
                    collisions.shouldReflect = true;
                }

            }

        }

    }

    void VerticalCollisions(ref Vector2 moveAmount) {

    float directionY = Mathf.Sign(moveAmount.y);
    float rayLength = Mathf.Abs(moveAmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {

            Vector2 rayOrigin = raycastOrigins.bottomLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
            RaycastHit2D[] hits = Physics2D.RaycastAll(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            for (int j = 0; j < hits.Length; j++) {

                if (hits[j]) {

                    if (hits[j].transform.gameObject == transform.gameObject) continue;

                    if (hits[j].transform.CompareTag("Lava")) {

                        if (cube.velocity.y < 0) cube.Break();

                        continue;

                    }

                   if (hits[j].transform.CompareTag("Solid")) {

                        if (cube.isDangerous) cube.Break();

                        if (cube.velocity.y < 0) cube.Break();

                    }

                    if (hits[j].transform.CompareTag("Hat")) {

                        Hat otherHat = hits[j].transform.GetComponent<Hat>();

                        if(!otherHat.isBeingThrown)
                            otherHat.BlowBack(otherHat.transform.position - transform.position, cube.velocity.magnitude);
                        
                    }

                    if (hits[j].transform.CompareTag("Player")) {

                        Player otherPlayer = hits[j].transform.GetComponent<Player>();

                        if (cube.isDangerous) {
                            Vector3 hitDirection = otherPlayer.transform.position - transform.position;
                            otherPlayer.BlowBack(hitDirection, 40, 0.2f);
                            cube.Break();
                        } else if (cube.velocity.y >= 0) {
                            continue;
                        } if (cube.velocity.y < 0) {
                            cube.Break();
                            otherPlayer.GetFootStooled(0.2f, false);
                        }

                    }

                    moveAmount.y = (hits[j].distance - skinWidth) * directionY;
                    rayLength = hits[j].distance;

                    if (!hits[j].transform.CompareTag("Player") && !hits[j].transform.CompareTag("Hat") && hits[j].transform.gameObject != transform.gameObject) {
                        collisions.below = directionY == -1;
                        collisions.above = directionY == 1;
                    }
                   
                }

            }

        }

    }

    float RoundToDecimal(float value, int numberOfPlaces) {

        value = Mathf.Floor(value * Mathf.Pow(10, numberOfPlaces)) / Mathf.Pow(10, numberOfPlaces);
        return value;

    }

    public struct CollisionInfo {

        public bool below;
        public bool above;
        public bool left;
        public bool right;
        public bool shouldReflect;
        public bool shouldReflectY;
        public int faceDir;

        public void Reset() {
            below = false;
            above = false;
            left = false;
            right = false;

            shouldReflect = false;
            shouldReflectY = false;
            faceDir = 1;
        }

    }

}


