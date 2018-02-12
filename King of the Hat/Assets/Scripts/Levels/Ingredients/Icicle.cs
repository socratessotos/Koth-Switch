using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (IcicleController))]
public class Icicle : Respawnable {

    public int maxHp = 2;
    int currentHp = 2;
    public float gravity = 1f;

    float shineTime;
    float shineTimer;

    bool isAttached = true;
    bool isFalling = false;

    IcicleController controller;
    SpriteRenderer spriteRenderer;
    Animator anim;
    Vector2 velocity;
    Vector2 lastVelocity;
    Vector3 initialSpriteSize;

    Vector3 initialPosition;

    ParticleSystem sparkles;

    public bool isCurrentlyAttached { get { return isAttached; } set { isAttached = value; } }
    public bool isCurrentlyFalling { get { return isAttached; } set { isAttached = value; } }

    [HideInInspector]
    public bool collisionSkip = false;

    void OnEnable() {
        Reset();   
    }

    void Awake () {
        controller = GetComponent<IcicleController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        velocity = Vector2.zero;
        sparkles = transform.GetChild(0).GetComponent<ParticleSystem>();

        initialSpriteSize = transform.localScale;
        lastVelocity = velocity;

        initialPosition = transform.position;

        ResetShineTime();
        Reset();
	}

    void FixedUpdate() {

        if (GameController.instance.game.state == Game.State.PAUSED) return;

        if (Time.time > shineTimer) 
            ResetShineTime();

        if(isFalling) {
            velocity.y -= (gravity * Time.fixedDeltaTime);

            controller.Move(velocity);

            if(controller.collisions.below) {
                isFalling = false;
                velocity.y = 0;

                VFXManager.instance.EmitAtPosition("Snow_Poof", 30, transform.position + Vector3.down * 1.2f, true);
                SetSparkleEmissionTo(2);
            }
        }
       
        Squash(velocity, lastVelocity);

        if (lastVelocity != velocity)
            lastVelocity = velocity;

    }

    public Vector2 GetVelocity() {
        return velocity;
    }

    public void SetVelocity(Vector2 _newVelocity) {
        velocity = _newVelocity;
    }

    public void SetVelocity(float _x, float _y) {
        velocity = new Vector2(_x, _y);
    }

    void Squash(Vector2 _velocity, Vector2 _lastVelocity) {

        float ax = Mathf.Abs(_velocity.x - _lastVelocity.x); //acc.x
        float ay = -Mathf.Abs(_velocity.y - _lastVelocity.y); //acc.y

        float squash = Mathf.Clamp((ax + ay) * 6, -0.5f, 0.5f);
        transform.localScale = initialSpriteSize + new Vector3(squash, -squash, 0);

    }

    public void BreakOff() {

        isAttached = false;
        isFalling = true;

        SetSparkleEmissionTo(50);
    }

    void ResetShineTime() {

        shineTime = Time.time + Random.Range(5f, 10f);
        shineTimer = shineTime;

        anim.Play("Shine");
    }

    public void Shake(int initialDirection) {
        
        

    }

    public void LoseHp() {

        anim.Play("HitFlash");

        currentHp--;

        if (currentHp <= 0) {
            Die();
        } else if (currentHp == 2) {
            VFXManager.instance.EmitAtPosition("Icicle_Explosion", 30, transform.position + Vector3.up * 1.5f, true);
        } else {
            VFXManager.instance.EmitAtPosition("Icicle_Explosion", 10, transform.position + Vector3.up * 0.3f, true);
        }

    }

    public void Die() {

        VFXManager.instance.EmitAtPosition("Icicle_Explosion", 100, transform.position + Vector3.up * 0.3f, true);
        
        gameObject.SetActive(false);

    }

    void SetSparkleEmissionTo(int value) {
        ParticleSystem.EmissionModule em = sparkles.emission;

        em.rateOverTime = value;
    }

    public override void Reset() {

        gameObject.SetActive(true);

        currentHp = maxHp;
        transform.position = initialPosition;

        isAttached = true;
        isFalling = false;

        SetSparkleEmissionTo(2);

    }

}