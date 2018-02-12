using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(CubeController))]
public class Cube : TransferDanger {

    public const float DANGER_SPEED_CUTOFF_X = 0.35f;
    public const float DANGER_SPEED_CUTOFF_Y = 0.1f;

    public float gravity = 1f;
    public float dangerousGravity = 0.1f;
    public float friction = 1f;
    public float drag = 1f;
    public float dangerousDrag = 0.1f;
    public float bounceDampening = 1f;

    CubeController controller;

    Vector3 initialSpriteSize;

    Vector3 spawnPosition;

    SpriteRenderer sprite;

    void Start() {
        controller = GetComponentInChildren<CubeController>();
        sprite = GetComponentInChildren<SpriteRenderer>();

        initialSpriteSize = transform.localScale;

        spawnPosition = transform.position - Vector3.up;
    }

    void FixedUpdate() {

        if (GameController.instance.game.state == Game.State.PAUSED) return;

        if (isDangerous) {

            if (IsDangerOver())
                EndDangerous();

        }

        CalculatePosition();

    }

    void CalculatePosition() {

        if(controller.collisions.shouldReflect) {
            velocity.x = -velocity.x * bounceDampening;
        }

        if (controller.collisions.shouldReflectY) {
            velocity.y = -velocity.y * bounceDampening;
        }

        if (Mathf.Abs(velocity.x) > friction * Time.deltaTime) {
            velocity.x -= Mathf.Sign(velocity.x) * ((controller.collisions.below) ? friction : ((isDangerous) ? dangerousDrag : drag)) * Time.fixedDeltaTime;
        } else {
            velocity.x = 0.0f;
        }

        velocity.y -= gravity * Time.fixedDeltaTime;

        //move
        controller.Move(velocity);

        if (controller.collisions.above) {
            velocity.y = 0f;
        }

        if (controller.collisions.below) {
            velocity.y = 0f;
        }

        Squash(velocity, lastVelocity);

        if ((int)(velocity.x * 100) != (int)(lastVelocity.x * 100))
            lastVelocity.x = velocity.x;

        if ((int)(velocity.y * 100) != (int)(lastVelocity.y * 100))
            lastVelocity.y = velocity.y;

    }

    bool IsDangerOver() {
        if (Mathf.Abs(velocity.x) < DANGER_SPEED_CUTOFF_X && velocity.y < 0 && Mathf.Abs(velocity.y) < DANGER_SPEED_CUTOFF_Y * 3) return true;
        return (Mathf.Abs(velocity.x) < DANGER_SPEED_CUTOFF_X && Mathf.Abs(velocity.y) < DANGER_SPEED_CUTOFF_Y);
    }

    void EndDangerous() {
        isDangerous = false;
        
        sprite.color = Color.white;
    }

    public override void Transfer(Vector2 direction) {

        base.Transfer(direction);
        
        isDangerous = true;

        velocity = direction;

        sprite.color = Color.red;

    }

    public void Break() {

        VFXManager.instance.EmitAtPosition("Break_Rock", 10, transform.position, true);

        Destroy(gameObject);

    }

    void Squash(Vector2 _velocity, Vector2 _lastVelocity) {

        float ax = Mathf.Abs(_velocity.x - _lastVelocity.x); //acc.x
        float ay = -Mathf.Abs(_velocity.y - _lastVelocity.y); //acc.y

        float squash = Mathf.Clamp((ax + ay) * 3, -0.2f, 0.2f);
        //print(ax + " " + ay + " " + squash);
        transform.localScale = initialSpriteSize + new Vector3(squash, -squash, 0);

    }

}
