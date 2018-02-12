using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squasher : MonoBehaviour {

    Vector2 lastVelocity;

}

public class PlayerAnimationVFXController : MonoBehaviour {

    public const string IDLE = "01_Idle";
    public const string RUN = "02_Run";
    public const string DASH = "03_DASH";
    public const string JUMP = "04_Jump";
    public const string FALL = "05_Fall";
    public const string WALL_SLIDE = "06_Wall_Slide";
    public const string WALL_JUMP = "07_Wall_Jump";
    public const string DEATH = "08_Death";
    public const string THEATRICAL_ENTRANCE = "09_Theatrical_Entrance";
    public const string THEATRICAL_VICTORY = "10_Theatrical_Victory";
    public const string HOVER = "11_Hover";
    public const string SUPER_JUMP = "12_Super_Jump";

    public enum AnimState { IDLE, RUN, DASH, JUMP, FALL, FAST_FALL, LAND, WALL_SLIDE, WALL_JUMP, DEATH, THEATRICAL_ENTRANCE, THEATRICAL_VICTORY, HOVER, SUPER_JUMP, STUNNED }
    public AnimState state;

    public Controller2D controller;
    public Player player;
    public SpriteRenderer deathInsignia;
    public SpriteRenderer spriteRenderer;
    public Animator anim;
    public float maxSquash = 0.2f;
    public float squashFactor = 0.1f;

    public GameObject fastFallLines;

    Vector2 lastVelocity;

    bool showingCharge = false;
    bool hasLanded = false;

    Vector3 floorPosition;
    SpriteAbberation spriteAbberation;

    void Awake() {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();

        if (anim == null)
            anim = GetComponent<Animator>();
        
        if (spriteAbberation == null)
            spriteAbberation = spriteRenderer.transform.GetComponent<SpriteAbberation>();

        lastVelocity = player.GetVelocity();

    }

    // Update is called once per frame
    void FixedUpdate() {

        particlesTimer++;
        if (particlesTimer > 10000)
            particlesTimer = 0;

        RaycastHit2D hit = Physics2D.Raycast(player.transform.position, Vector2.down, 3, controller.collisionMask);
        floorPosition = hit.point;

        Vector2 velocity = player.GetVelocity();
        spriteRenderer.flipX = (controller.collisions.faceDir < 0);
        Squash(velocity, lastVelocity);

        //check if stunned
        if (player.isStunned) {
            state = AnimState.STUNNED;

            PlayStunnedParticles();

            if (player.isFootStooled) {

                StopCoroutine("PlayFootStoolAnimation");
                StartCoroutine("PlayFootStoolAnimation");

                //play footstool sound
                AudioManager.instance.PlaySound("Footstool", Vector3.zero);

                player.isFootStooled = false;
            }
        }

        //check if grounded
        if (controller.collisions.below) {

            //the frame where you land
            if (!hasLanded) {
                state = AnimState.LAND;
                hasLanded = true;

                //play land sound
                AudioManager.instance.PlaySound("Land", Vector3.zero);
				//player.v.Vibrate (5f, 5f, 0.175f);

                if (player.isFastFalling == true) {
                    PlayFastFallDust();
                } else {
                    PlayLandDust();
                }
            }

            ToggleFastFallTrail(false);
            player.isFastFalling = false;

            //check if standing still
            if (Mathf.Abs(velocity.x) < 0.01f) {

                //idle vfx scripting
                //

                //play idle if its not already playing
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName(IDLE)) {
                    state = AnimState.IDLE;
                    anim.Play(IDLE);
                }

            } else {

                //run vfx scripting here
                PlayRunParticles();

                //this means the player has changed directions
                if (Mathf.Sign(velocity.x) != Mathf.Sign(lastVelocity.x)) {
                }

                //this means the player has started moving from idle, or player has changed directions
                if (Mathf.Abs(lastVelocity.x) < 0.01f || Mathf.Sign(velocity.x) != Mathf.Sign(lastVelocity.x)) {
                    state = AnimState.DASH;
                    PlayDashParticles();

                    //play dash sound
                    AudioManager.instance.PlaySound("Dash", Vector3.zero);

                }

                //play run if its not already playing
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName(RUN)) {
                    state = AnimState.RUN;
                    anim.Play(RUN);
                    particlesTimer = 5;
                }


            }

            //if not figure out what the player is doing
        } else {

			hasLanded = false;

            //check if the player is wall sliding
            if (player.isWallSliding) {
                ToggleFastFallTrail(false);

                spriteRenderer.flipX = (controller.collisions.right);

                //wallSliding vfx scripting here
                PlayWallSlideParticles();

                //play jump if its not already playing
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName(WALL_SLIDE)) {
                    state = AnimState.WALL_SLIDE;
                    anim.Play(WALL_SLIDE);
                    particlesTimer = -1;

                    //play grip wall sound
                    AudioManager.instance.PlaySound("Grip_Wall", Vector3.zero);

                }

                //if not figure out what the player is doing
            } else {

                //if ascending
                if (velocity.y > 0.0f && !player.isHovering) {

                    //play jump if its not already playing
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName(JUMP)) {
                        ToggleFastFallTrail(false);
                        state = AnimState.JUMP;

                        //grounded jump vfx scripting here

						if (anim.GetCurrentAnimatorStateInfo(0).IsName(WALL_SLIDE)) {
                            //wall jump impulse vfx scripting here
							PlayWallJumpParticles();

                        } else {


                        }

                        if (player.jumpsCompleted == 0)
                            //play jump sound
                            AudioManager.instance.PlaySound("Jump", Vector3.zero);
                        else
                            AudioManager.instance.PlaySound("Double_Jump", Vector3.zero);

                        anim.Play(JUMP);
                        hasLanded = false;

                    }


                    //if descending	
                } else {

                    if (player.isHovering) {

                        //play fall if its not already playing
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(HOVER)) {
                            state = AnimState.HOVER;
                            anim.Play(HOVER);

                            ToggleFastFallTrail(false);
                        }

                        //float vfx scripting here
                        if(player.hasHoverArmor) PlayHoverParticles();

                    } else {

                        if (player.isFastFalling) {
                            state = AnimState.FAST_FALL;
                            ToggleFastFallTrail(true);
                        } else state = AnimState.FALL;

                        //fall vfx scripting here
                        //

                        //play fall if its not already playing
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName(FALL)) {
                            anim.Play(FALL);
                        }

                    }

                }

            }

        }

        if (player.isBeingHit) {

            StopCoroutine("HitFlash");
            StartCoroutine("HitFlash");
            AbberatePlayer();

            StopCoroutine("PlayFootStoolAnimation");
            StartCoroutine("PlayFootStoolAnimation");

            player.isBeingHit = false;

            //play hit player sound
            AudioManager.instance.PlaySound("Hit_Player", Vector3.zero);

        }

        if (player.isDead && !deathAnimationPlaying) {

            StopCoroutine("HitFlash");

            StopCoroutine("DeathAnimation");
            StartCoroutine("DeathAnimation");

        }

        if (player.isInvulnerable && !invulnerableAnimationPlaying) {

            StopCoroutine("HitFlash");
            StopCoroutine("DeathAnimation");

            StopCoroutine("InvulnerabilityAnimation");
            StartCoroutine("InvulnerabilityAnimation");

        }

        if (player.isCharging && !showingCharge) {
            showingCharge = true;
            StartCoroutine("ChargeEffect");
        }

        if (!player.isCharging && showingCharge) {
            showingCharge = false;
            StopCoroutine("ChargeEffect");
            spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, 0));
        }

        if (lastVelocity != velocity)
            lastVelocity = velocity;

    }

	void LateUpdate () {

	}

    void Squash(Vector2 velocity, Vector2 lastVelocity) {

        float ax = Mathf.Abs(velocity.x - lastVelocity.x); //acc.x
        float ay = -Mathf.Abs(velocity.y - lastVelocity.y); //acc.y

        float squash = Mathf.Clamp((ax + ay) * squashFactor, -maxSquash, maxSquash);

        spriteRenderer.transform.localScale = Vector3.one + new Vector3(squash, -squash, 0);
        spriteRenderer.transform.localPosition = new Vector3(0, -0.5f, 0) + new Vector3(0, spriteRenderer.transform.localScale.y / 2, 0);

    }

    public IEnumerator ChargeEffect() {

        int anim = 0;

        while (showingCharge) {

            if (anim % 2 == 0) {

                spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, 80));

            } else {

                spriteRenderer.material.SetColor("_OverrideColor", new Color32(0, 0, 0, 100));

            }

            anim++;

            yield return new WaitForSeconds(0.1f);
        }

    }

    public IEnumerator PlayFootStoolAnimation() {

        float t = 0;
        float speed = 5f;

        while (t < 1) {

            float squash = Mathf.Lerp(0, maxSquash * 1.5f, t);
            t += Time.deltaTime * speed;

            spriteRenderer.transform.localScale = Vector3.one + new Vector3(squash, -squash, 0);
            spriteRenderer.transform.localPosition = new Vector3(0, -0.5f, 0) + new Vector3(0, spriteRenderer.transform.localScale.y / 2, 0);

            yield return null;

        }

        t = 0;
        speed = 10f;

        while (t < 1) {

            float squash = Mathf.Lerp(maxSquash * 1.5f, -maxSquash * 1.5f, t);
            t += Time.deltaTime * speed;

            spriteRenderer.transform.localScale = Vector3.one + new Vector3(squash, -squash, 0);
            spriteRenderer.transform.localPosition = new Vector3(0, -0.5f, 0) + new Vector3(0, spriteRenderer.transform.localScale.y / 2, 0);

            yield return null;

        }

        t = 0;
        speed = 20f;

        while (t < 1) {

            float squash = Mathf.Lerp(-maxSquash * 1.5f, 0, t);
            t += Time.deltaTime * speed;

            spriteRenderer.transform.localScale = Vector3.one + new Vector3(squash, -squash, 0);
            spriteRenderer.transform.localPosition = new Vector3(0, -0.5f, 0) + new Vector3(0, spriteRenderer.transform.localScale.y / 2, 0);

            yield return null;

        }

        yield return null;

    }

    public IEnumerator HitFlash() {

        spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, 0));
        yield return null;

        float t = 0;
        float speed = 15f;

        while (t < 1) {

            byte alpha = (byte)Mathf.Lerp(0, 255, t);
            spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, alpha));
            t += Time.deltaTime * speed;

            yield return null;

        }

        spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, 255));
        yield return new WaitForSeconds(0.1f);

        //spriteRenderer.material.SetColor("_OverrideColor", new Color32(0, 0, 0, 200));
        //yield return new WaitForSeconds(0.1f);

        spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, 0));

    }

    public IEnumerator AbberatePlayer() {

        spriteAbberation.Abberate();
        yield return null;

        float t = 0;
        float speed = 15f;

        while (t < 1) {

            spriteAbberation.lerpScale = t;

            t += Time.deltaTime * speed;

            yield return null;

        }

        spriteAbberation.clearAbberations();

    }

    bool deathAnimationPlaying;
    public IEnumerator DeathAnimation() {

        deathAnimationPlaying = true;

        float t = 0;
        float speed = 3f;

        while (t < 1) {

            byte alpha = (byte)Mathf.Lerp(0, 255, t);
            spriteRenderer.material.SetColor("_OverrideColor", new Color32(0, 0, 0, alpha));
            deathInsignia.color = new Color32(255, 255, 255, alpha);
            deathInsignia.flipX = spriteRenderer.flipX;
            t += Time.deltaTime * speed;

            PlaySludgeParticles();

            yield return null;

        }

        t = 0;
        speed = 1f;

        while (t < 1) {

            float cutoff = Mathf.Lerp(0, 1f, t);
            spriteRenderer.material.SetFloat("_SliceAmount", cutoff);
            deathInsignia.color = new Color32(255, 255, 255, (byte)(255 * (1 - cutoff * 2)));
            deathInsignia.flipX = spriteRenderer.flipX;

            t += Time.deltaTime * speed;

            if(t < 0.75)
                PlaySludgeParticles();

            yield return null;

        }

        player.gameObject.SetActive(false);
        Invoke("DelayedRespawn", 1);

        deathAnimationPlaying = false;
        player.isDead = false;

        spriteRenderer.material.SetColor("_OverrideColor", new Color32(0, 0, 0, 0));
        spriteRenderer.material.SetFloat("_SliceAmount", 0);
        deathInsignia.color = new Color32(255, 255, 255, 0);

        if (player.playerIndex == -1) {
            Destroy(player.gameObject);
            Destroy(player.hat.gameObject);
        }

    }

    bool invulnerableAnimationPlaying;
    public IEnumerator InvulnerabilityAnimation() {

        invulnerableAnimationPlaying = true;

        float t = 0;
        float speed = 7.5f;

        while (player.isInvulnerable) {
            
            //flash to white
            byte alpha = (byte)Mathf.Lerp(0, 255, t);
            byte greyScale = (byte)Mathf.Lerp(100, 255, t);
            spriteRenderer.material.SetColor("_OverrideColor", new Color32(greyScale, greyScale, greyScale, alpha));
            t += Time.deltaTime * speed;

            if (t >= 1f) {
                t = 1f;
                speed = -speed;
            } else if (t <= 0f) {
                t = 0f;
                speed = -speed;
            }

            yield return null;

        }

        invulnerableAnimationPlaying = false;
        spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, 0));
        yield return null;

    }

    void DelayedRespawn() {
        GameController.instance.game.RequestRespawn(player.playerIndex);
    }

    float particlesTimer = 0f;

    void PlayRunParticles() {
        if (floorPosition == Vector3.zero)
            return;

        int dir = controller.collisions.faceDir;

        if (particlesTimer % 10 == 0) {
            var runEmitParams = new ParticleSystem.EmitParams();
            runEmitParams.rotation3D = new Vector3(0, 90 + 90 * dir, 0);
            float xOffset = player.GetComponent<BoxCollider2D>().bounds.extents.x;
            VFXManager.instance.EmitAtPosition("Run_Dust", runEmitParams, 1, floorPosition + new Vector3(xOffset * -dir, 0.85f, 0), false);
            particlesTimer = 1;
        }

    }

    void PlayDashParticles() {
        if (floorPosition == Vector3.zero)
            return;

        int dir = controller.collisions.faceDir;

        var turnEmitParams = new ParticleSystem.EmitParams();
        turnEmitParams.velocity = new Vector3(2 * -dir, 0.5f, 0);

        if (dir > 0) {
            turnEmitParams.rotation3D = new Vector3(0, 0, 0);
        } else {
            turnEmitParams.rotation3D = new Vector3(0, 180, 0);
        }

        float xOffset = player.GetComponent<BoxCollider2D>().bounds.extents.x;
        VFXManager.instance.EmitAtPosition("Turn_Dust", turnEmitParams, 1, floorPosition + new Vector3(xOffset * -dir, 0.85f, 0), false);

    }

    void PlayWallSlideParticles() {

		if (particlesTimer % 10 == 0) {
			int dir = (controller.collisions.right) ? 1 : -1;
			VFXManager.instance.EmitAtPosition("Wall_Slide_Dust", 1, transform.position + (Vector3.right * 0.65f * dir), true);
		}

    }

    void PlaySludgeParticles() {
        VFXManager.instance.EmitAtPosition("Death_Sludge", 10, transform.position, true);
    }

    void PlayDeathParticles() {
        VFXManager.instance.EmitAtPosition("Death_Poof", 30, transform.position, true);
    }

	void PlayLandDust() {
		if (floorPosition == Vector3.zero)
			return;

		int dir = (controller.collisions.right) ? 1 : -1;

		var landDustEmitParams = new ParticleSystem.EmitParams();

		for(int i = -1; i < 2; i++) {
			landDustEmitParams.velocity = new Vector3(Random.Range(-1f, 1f) + player.GetVelocity().normalized.x * Random.Range(0.3f, 1f), Random.Range(0f, 0.3f), 0);
			landDustEmitParams.startSize3D = new Vector3(Random.Range(3, 5), Random.Range(3, 4), 1);
			landDustEmitParams.startLifetime = Random.Range(0.4f, 0.6f);
			landDustEmitParams.rotation = Random.Range(-20, 20);

			float xOffset = i * player.GetComponent<BoxCollider2D>().bounds.extents.x;

			VFXManager.instance.EmitAtPosition("Land_Dust", landDustEmitParams, 1, floorPosition + new Vector3(xOffset, Random.Range(1f, 1.5f), 0), false);
		}

	}

    void PlayFastFallDust() {
        if (floorPosition == Vector3.zero)
            return;

        var landDustEmitParams = new ParticleSystem.EmitParams();

        landDustEmitParams.startSize = 4;
        landDustEmitParams.velocity = new Vector3(4f + player.GetVelocity().normalized.x, 0, 0);
        float xOffset = player.GetComponent<BoxCollider2D>().bounds.extents.x;
        VFXManager.instance.EmitAtPosition("Land_Dust", landDustEmitParams, 1, floorPosition + new Vector3(xOffset, Random.Range(1.5f, 2f), 0), false);

        landDustEmitParams.startSize = 3;
        landDustEmitParams.velocity = new Vector3(Random.Range(2f, 3f) + player.GetVelocity().normalized.x, 1, 0);
        VFXManager.instance.EmitAtPosition("Land_Dust", landDustEmitParams, 1, floorPosition + new Vector3(xOffset, Random.Range(1.5f, 2f), 0), false);

        landDustEmitParams.startSize = 4;
        landDustEmitParams.velocity = new Vector3(-4f + player.GetVelocity().normalized.x, 0, 0);
        xOffset = -player.GetComponent<BoxCollider2D>().bounds.extents.x;
        VFXManager.instance.EmitAtPosition("Land_Dust", landDustEmitParams, 1, floorPosition + new Vector3(xOffset, Random.Range(1.5f, 2f), 0), false);

        landDustEmitParams.startSize = 3;
        landDustEmitParams.velocity = new Vector3(Random.Range(-2f, -3f) + player.GetVelocity().normalized.x, 1, 0);
        VFXManager.instance.EmitAtPosition("Land_Dust", landDustEmitParams, 1, floorPosition + new Vector3(xOffset, Random.Range(1.5f, 2f), 0), false);

    }

    public void ToggleFastFallTrail(bool _on) {

        //trail
		fastFallLines.GetComponent<TrailRenderer>().Clear();
        fastFallLines.GetComponent<TrailRenderer>().enabled = _on;

        //dust
        ParticleSystem.EmissionModule em = fastFallLines.GetComponent<ParticleSystem>().emission;
        em.enabled = _on;

    }

    public void PlayStunnedParticles() {

        if (particlesTimer % 4 == 0) {
            VFXManager.instance.EmitAtPosition("Stunned_Smoke", 1, transform.position, false);
            particlesTimer = 1;
        }

    }

	public void PlayWallJumpParticles() {

		var jumpEmitParams = new ParticleSystem.EmitParams();
		jumpEmitParams.rotation3D = new Vector3(0, 0, 90 * player.faceDir - player.GetVelocity().y * player.faceDir);
		jumpEmitParams.velocity = new Vector3(1 * player.faceDir, 0.5f, 0);

		VFXManager.instance.EmitAtPosition("Jump_Dust", jumpEmitParams, 1, new Vector3(transform.position.x - (controller.collider.bounds.extents.x * 0.2f) * player.faceDir, transform.position.y, 0), false);

	}

    public void PlayHoverParticles() {
        VFXManager.instance.EmitAtPosition("Hover_Smoke", 1, transform.position + Vector3.down + Vector3.right * player.faceDir * 0.5f, true);
        VFXManager.instance.EmitAtPosition("Hover_Stars", 1, transform.position + Vector3.down + Vector3.right * player.faceDir * 0.5f, true);
    }

    public void Abberate() {
        StopCoroutine("AbberatePlayer");
        StartCoroutine("AbberatePlayer");
    }

}