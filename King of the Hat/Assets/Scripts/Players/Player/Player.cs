using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    public const float BOUNCE_TIME = 0.5f;
    public const float BOUNCE_PLATFORM_HEIGHT = 20f;
    public const float BOUNCE_OUT_OF_WELL_HEIGHT = 6f;
    public const float BOUNCE_OFF_LAVA_HEIGHT = 6f;

    public const float ICE_FRICTION_MODIFIER = 2f;

    public const float respawnInvulnerabilityTime = 2f;

    [HideInInspector]
    public int playerIndex = -1;
    [HideInInspector]
    public int teamNumber = -1;

    [Header("Hat")]
    public Hat hat;

    [Header("Basic Movement")]
    public float moveSpeed = 6;
    public float airMoveSpeed = 14;
    public bool usingDifferentMoveSpeeds = false;
    public float hatlessMoveBonus = 2;
    public float hoverMoveBonusX = 0;
    public float hoverMoveBonusY = 0;
    public float accelerationTimeRunning = .1f;
	public float weightFactor = 1f;
    float accelerationTimeStun = 2f;
	float accelerationTimeRecovery = 2f;
    [Space(5)]

    [Header("Dash Movement")]
    public float dashSpeed = 30;
    public float accelerationTimeDash = .5f;
    public float deccelerationTimeDash = .5f;
    public float dashTime = 1;
    public float dashCoolDown = 1;
    public float dashPower = 1;
    public float dashStunTime = 0.2f;
    public float dashHitFreeze = 1f;
    [Space(5)]

    [Header("Basic Jumping")]
    public int jumpsAllowed = 1;
    public float minJumpHeight = 1;
    public float maxJumpHeight = 4;
    public float hatlessJumpHeightBonus = 1;
    public float timeToJumpApex = .4f;
    public float accelerationTimeJumping = .2f;
    public float fastFallFactor = 2;
    public float terminalVelocity = 50f;
    public float terminalVelocityFastFall = 50f;
    public bool usingDifferentFallSpeeds = false;
    [Space(5)]

    [Header("Wall Jumping")]
    public float accelerationTimeAfterWallJump = .2f;
    public float wallJumpTime = 0.1f;
    public float wallSlideSpeedMax = 3;
    public float wallSlideFastFallFactor = 2;
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallStickTime = .25f;
    [Space(5)]

    [Header("Special Jumping")]
    public float bounceHeight = 2;
    public float superJumpHeight = 1;
    [Space(5)]

    [Header("Hover")]
    public float maxHoverTime = 0.5f;
    public int hoversPerJump = 1;
    public float accelerationTimeHover = .5f;
    public bool hoverTimeResets = false;
    public bool hasHoverArmor = false;
	public bool canHoverX = false;
	public bool canHoverY = false;
	public float hoverImpulseHeight = 0f;
	public float totalHoverImpulseTime = 0f;
	float hoverImpulse;
    [Space(5)]

    [Header("Teleport")]
    public bool canTeleport = false;
    public enum TeleportState { PRE_HANG, TELEPORTING, POST_HANG }
    [HideInInspector]
    public TeleportState teleportState;
    bool teleporting = false;
    public int teleportsPerJump = 2;
    int teleportsCompleted = 0;
    public float teleportMoveSpeed = 50;
    public float teleportTime = 0.2f;
    public float teleportHangTime = 0.15f;
    float teleportTimer;
    Vector2 teleportDirection;
    [Space(5)]

    [Header("Throw Stats")]
    public float maxChargeTime = 1f;
    [Space(5)]

    [Header("Status Effects")]
    public GameObject stunStars;
    [Space(5)]

    [Header("On PickUp Ability")]
    public PickUpAbility pickUpAbility = PickUpAbility.NONE;
    public enum PickUpAbility { NONE, SPEED_BOOST }
    [Space(5)]

    [Header("On Throw Ability")]
    public ThrowAbility throwAbility = ThrowAbility.NONE;
    public enum ThrowAbility { NONE, SPEED_BOOST }
    [Space(5)]


    public bool canHurtHat = true;
    public bool canKillHat = true;

    float timeToWallUnstick;

    float gravity;

    [HideInInspector]
    public int jumpsCompleted;
    float maxJumpVelocity;
    float hatlessMaxJumpVelocity;
    float minJumpVelocity;
    float bounceVelocity;
    float bouncePlatformVelocity;
    float bounceOutOfWellVelocity;
    float bounceOffLavaVelocity;
    float superJumpVelocity;
    Vector3 velocity;
    float velocityXSmoothing;
	float velocityYSmoothing;

    float accelerationTimeGrounded;
    float accelerationTimeAirborne;

    [HideInInspector]
    public Controller2D controller;

    Vector2 directionalInput;
	public Vector2 directionalThrowInput;
    bool wallSliding;
    int wallDirX;

    float currentDashSpeed;
    float currentDashSpeedSmoothing;
    float targetDashSpeed;
    Vector2 targetDashDirection;

    int hoversCompleted = 0;
    bool hovering;
    float timeToStopHovering;

    int freezeFrames = 0;

    float endStunTime;
    float chargeTime;

    float invulnerabilityTimer;

    bool dashing;
    bool wallJumped;
    bool superJumping;
    bool wallJumping;
    bool coolingDown;
    bool stunned;
	bool recovering;
    bool hitFreeze;
    float hitFreezeLength;
    bool wasHit;
    bool wasKilled;
    bool charging;
    bool fastFalling;
    bool isPressingA = false;
    bool isBouncing = false;
    bool invulnerable = false;
	bool footStooled = false;
    bool isSliding;

    //accessors
    public bool isWallSliding { get { return wallSliding; } }
    public bool isHovering { get { return hovering; } }
    public bool hasWallJumped { get { return wallJumped; } }
    public bool isSuperJumping { get { return superJumping; } }
    public bool isCoolingDown { get { return coolingDown; } }
    public bool isStunned { get { return stunned; } }
    public bool isHitFrozen { get { return hitFreeze; } }
    public bool isBeingHit { get { return wasHit; } set { wasHit = value; } }
    public int faceDir { get { return controller.collisions.faceDir; } set { controller.collisions.faceDir = value; } }
    public bool isOut { get { return gameObject.activeSelf; } }
    public bool isDead { get { return wasKilled; } set { wasKilled = value; } }
    public bool isCharging { get { return charging; } set { charging = value; } }
    public bool isFastFalling { get { return fastFalling; } set { fastFalling = value; } }
    public bool isInvulnerable { get { return invulnerable; } set { invulnerable = value; } }
	public bool isFootStooled { get { return footStooled; } set { footStooled = value; } }
    public bool isSlidingOnIce { get { return isSliding; } set { isSliding = value; } }
    public bool isTeleporting { get { return teleporting; } }

    [HideInInspector]
    public List<string> kills = new List<string>();
    [HideInInspector]
    public List<string> killedBy = new List<string>();

	[HideInInspector]
	public Vibrator v;

	[HideInInspector]
	public PlayerInput pInput;

    //public GameObject soulPrefab;

    public virtual void Start() {
        controller = GetComponent<Controller2D>();
		v = GetComponent <Vibrator> ();
		pInput = GetComponent <PlayerInput> ();

        if (!usingDifferentMoveSpeeds) {
            airMoveSpeed = moveSpeed;
        }

        if (!usingDifferentFallSpeeds) {
            terminalVelocityFastFall = terminalVelocity;
        }

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        hatlessMaxJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * (maxJumpHeight + hatlessJumpHeightBonus));
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        bounceVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * bounceHeight);
        bouncePlatformVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * BOUNCE_PLATFORM_HEIGHT);
        bounceOutOfWellVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * BOUNCE_OUT_OF_WELL_HEIGHT);
        bounceOffLavaVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * BOUNCE_OFF_LAVA_HEIGHT);

        superJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * superJumpHeight);
		hoverImpulse = Mathf.Sqrt(2 * Mathf.Abs(gravity) * superJumpHeight);

        UseNormalAirAcceleration();

    }

    public virtual void FixedUpdate() {

        if (GameController.instance.game.state == Game.State.PAUSED) return;

        GetCurrentInput();

        if (deathFreeze) {
            if (Time.time > deathFreezeTime) {
                deathFreeze = false;
                wasKilled = true;
            } else
                return;
        }

        if (hitFreeze) {

            if (freezeFrames < hitFreezeLength) {
                freezeFrames++;
            } else {
                freezeFrames = 0;
                hitFreeze = false;
            }

            return;
        }

        if (invulnerable) {
            if(Time.time > invulnerabilityTimer) {
                EndInvulnerability();
            }
        }

        CalculateVelocity();
        HandleWallSliding();

        controller.Move(velocity * Time.fixedDeltaTime, directionalInput, false, isDead);

		if ((controller.collisions.above || controller.collisions.below)) {
//			if (controller.collisions.slidingDownMaxSlope) {
//				velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.fixedDeltaTime;
//			} else {
//				velocity.y = 0;
//			}

			velocity.y = 0;
		}

		if (controller.collisions.below) {
			jumpsCompleted = 0;
			hoversCompleted = 0;            
			superJumping = false;

            if (!teleporting) teleportsCompleted = 0;
		}

        if (controller.collisions.shouldBounce) {
            velocity.y = bounceVelocity;
            controller.collisions.shouldBounce = false;
            StartBounce();

            canHurtHat = true;

        } else if (controller.collisions.shouldBounceOnBouncyPlatform) {
            velocity.y = bouncePlatformVelocity;
            controller.collisions.shouldBounceOnBouncyPlatform = false;
            StartBounce();
        } else if (controller.collisions.shouldBounceOutOfWell) {
            velocity.y = bounceOutOfWellVelocity;
            controller.collisions.shouldBounceOutOfWell = false;
            StartBounce();
        } else if(controller.collisions.shouldBounceOnLava) {
            velocity.y = bounceOffLavaVelocity;
            velocity.x = (velocity.x <= 1 && velocity.x >= -1) ? ((velocity.x >= 0) ? 10 : -10) : velocity.x;
            controller.collisions.shouldBounceOnLava = false;
            StartBounce();

            if(hat.isCurrentlyAttached)
                hat.BlowBack(new Vector2(-velocity.x, velocity.y / 2), 0.3f); //what % of player blow back is applied to the hat

        }
    }

    public virtual void GetCurrentInput() {
        if (Mathf.Abs(directionalInput.x) <= 0.2f) directionalInput.x = 0;
        if (Mathf.Abs(directionalInput.y) <= 0.2f) directionalInput.y = 0;

        #if UNITY_SWITCH
        if (controller.collisions.below) {
            if (directionalInput.x >= 0.5f) directionalInput.x = 1;
            if (directionalInput.x <= -0.5f) directionalInput.x = -1;
        }

        if (directionalInput.y >= 0.7f) directionalInput.y = 1;
        if (directionalInput.y <= -0.7f) directionalInput.y = -1;
        #endif

        if (directionalInput.x >= 0.6f) directionalInput.x = 1;
        if (directionalInput.x <= -0.6f) directionalInput.x = -1;

        if (directionalInput.y >= 0.8f) directionalInput.y = 1;
        if (directionalInput.y <= -0.8f) directionalInput.y = -1;
    }

	void LateUpdate () {
		//pInput.input.snapshot.Reset ();
	}

    public void ApplyFreezeFrames(int _numberOfFrames) {
        hitFreezeLength = _numberOfFrames;
        hitFreeze = true;
    }

    public Vector2 GetVelocity() {
        return velocity;
    }

    public void SetVelocity(float x, float y, bool useOwnX = false, bool useOwnY = false) {

        if (useOwnX) x = velocity.x;
        if (useOwnY) y = velocity.y;

        velocity = new Vector2(x, y);
    }

	public Vector2 GetDirectionalInput() {
		return directionalInput;
	}

    public void SetDirectionalInput(Vector2 input) {
        directionalInput = input;
    }

	public Vector2 GetDirectionalThrowInput() {
		return directionalThrowInput;
	}

	public void SetDirectionalThrowInput(Vector2 input) {
		directionalThrowInput = input;
	}

    //ground accelerations

    void UseDashAcceleration() {
        accelerationTimeAirborne = accelerationTimeDash;
    }

    void UseNormalGroundAcceleration() {
        accelerationTimeAirborne = accelerationTimeJumping;
    }

    //airborne accelerations

    void UseWallJumpAcceleration() {
        accelerationTimeAirborne = accelerationTimeAfterWallJump;
    }

    void UseHoverAcceleration() {
        accelerationTimeAirborne = accelerationTimeHover;
    }

    void UseNormalAirAcceleration() {
        accelerationTimeAirborne = accelerationTimeJumping;
    }

	bool canHoverJump = false;
	float hoverImpulseTimer = 0;
    public void OnJumpInputDown() {

        if (stunned) return;

        isPressingA = true;

        StopDashing();

        //if (dashing || hitFreeze) return;

        if (wallSliding && !controller.collisions.below) {
            teleportsCompleted = 0;

            if (wallDirX == Mathf.Sign(directionalInput.x) && directionalInput.x != 0) {
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;
            } else if (directionalInput.x == 0) {
                velocity.x = -wallDirX * wallJumpOff.x;
                velocity.y = wallJumpOff.y;
            } else {
                velocity.x = -wallDirX * wallLeap.x;
                velocity.y = wallLeap.y;
            }

            UseWallJumpAcceleration();
            Invoke("UseNormalAirAcceleration", wallJumpTime);

        } else if (hoversPerJump > 0 && hoversCompleted < hoversPerJump && (jumpsCompleted > 0 || !controller.collisions.below || jumpsAllowed <= 0)) {

            if (hoverTimeResets || hoversCompleted == 0) timeToStopHovering = maxHoverTime;

            if (timeToStopHovering > 0) {
                hovering = true;
                velocity.y = hoverImpulseHeight;
            }

            hoversCompleted++;

            if (controller.collisions.below)
                canHoverJump = true;

        } else if (canTeleport 
                && (!teleporting || (teleporting && teleportState == TeleportState.POST_HANG)) 
                && teleportsPerJump > 0 && teleportsCompleted < teleportsPerJump 
                && ((jumpsCompleted >= jumpsAllowed) || (jumpsCompleted == 0 && !controller.collisions.below))) {

            StartTeleport(directionalInput);

        } else if ((controller.collisions.below || jumpsCompleted < jumpsAllowed) && jumpsAllowed > 0) {
//            if (controller.collisions.slidingDownMaxSlope) {
//                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x)) { // not jumping against max slope
//                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
//                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
//                }
//            } else {
//
//                PlayJumpParticles();
//
//                velocity.y = (hat.isCurrentlyAttached ? maxJumpVelocity : hatlessMaxJumpVelocity);
//                jumpsCompleted++;
//
//            }

			PlayJumpParticles();

			velocity.y = (hat.isCurrentlyAttached ? maxJumpVelocity : hatlessMaxJumpVelocity);
			jumpsCompleted++;

        } else {
           
        }

    }

    public void OnJumpInputUp() {

        isPressingA = false;
        if (velocity.y > minJumpVelocity && !superJumping) {
            velocity.y = minJumpVelocity;
        }

        hovering = false;

		canHoverJump = false;
		hoverImpulseTimer = 0;

    }

    void StartTeleport(Vector2 _direction) {

        velocity = Vector3.zero;

        teleporting = true;
        teleportState = TeleportState.PRE_HANG;

        teleportTimer = teleportHangTime;
        teleportsCompleted++;
        teleportDirection = _direction;

        //play fx
        PlayTeleportEffect();
    }

    void EndTeleport() {

        teleporting = false;
        velocity = Vector3.zero;

        //play fx
    }

    List<Vector2> pastDirections = new List<Vector2>();
    int maxRememberedInputs = 10;
	[HideInInspector] public bool canPickUpHat = true;
    public void OnAbilityInputDown() {

        //if(hat.currentHatClick < hat.maxNumberOfClicks)
        //    hat.currentHatClick++;

        if(hat.isCurrentlyAttached || hat.isBeingAttached) {
            ReleaseHat();
        } else {
            hat.ActivateRepeatedClickAbility();

        }
        /*
        if (hat.currentHatClick < 2) {
            //if (coolingDown || dashing || hitFreeze) return;
        } else {
        }
        */

		canPickUpHat = false;

    }

    public void OnAbilityInputUp() {

		canPickUpHat = true;

    }

    void ReleaseHat() {
        if (hat.isThrowAllowed()) {
            //hat.Throw(directionalInput, 0);
			hat.Throw(new Vector2 (InputController.ConstrainAxisTo16Angles(directionalInput.x), InputController.ConstrainAxisTo16Angles(directionalInput.y)), 0);

            ActivateOnThrowAbility();

            //play throw sound
            AudioManager.instance.PlaySound ("Throw", Vector3.zero);

        } else {
            
        }

        chargeTime = 0;

        charging = false;
        //hovering = false;
    }

    public void ActivateOnPickUpAbility() {

        switch (pickUpAbility) {

            case PickUpAbility.NONE:
                break;

            case PickUpAbility.SPEED_BOOST:
                GetComponent<SpeedBoostAbility>().BoostSpeed();
                break;

            default:
                break;

        }

    }

    void RefreshPickUpAbility() {

        switch (pickUpAbility) {

            case PickUpAbility.NONE:
                break;

            case PickUpAbility.SPEED_BOOST:

                break;

            default:
                break;

        }

    }

    void ActivateOnThrowAbility() {

        switch (throwAbility) {

            case ThrowAbility.NONE:
                break;

            case ThrowAbility.SPEED_BOOST:
                GetComponent<SpeedBoostAbility>().BoostSpeed();
                break;

            default:
                break;

        }

    }

    void StartDashing() {

        StopDashing();

		velocity = new Vector2 (0, velocity.y);
        targetDashDirection = ((directionalInput != Vector2.zero) ? new Vector2(directionalInput.x, -1f).normalized : (new Vector2(faceDir, -1f).normalized));
        targetDashSpeed = dashSpeed;
        currentDashSpeed = 0;
        dashing = true;

        Invoke("StartDashDecceleration", dashTime - deccelerationTimeDash);
        Invoke("StopDashing", dashTime);

    }

    void StartDashDecceleration() {
        //targetDashSpeed = moveSpeed * -faceDir;
    }

    void StopDashing() {
        dashing = false;
    }


    public void BurnAss() {

        GetStunned(0.2f);

    }

    float totalStunTime = 0;
    public virtual void GetStunned(float timeAmount) {
       
		stunned = true;
        wasHit = true;

		totalStunTime = timeAmount;
        endStunTime = Time.time + timeAmount;

        stunStars.SetActive(false);
        stunStars.SetActive(true);


    }

    void EndStun() {
        stunned = false;

        stunStars.SetActive(false);
    }

    void StartBounce() {

        if ((!isBouncing)) {
            isBouncing = true;
            //Invoke("EndBounce", BOUNCE_TIME);
        }

    }

    void EndBounce() {
        isBouncing = false;
    }

    public void GetFootStooled(float timeAmount, bool opponentWasFastFalling) {
		velocity.x = 0f;
        velocity.y = (opponentWasFastFalling) ? -20f : 0f;
        GetStunned(timeAmount);

		isFootStooled = true;

        //squish the player here

    }

    public virtual void BlowBack(Vector3 direction, float magnitude, float coolDownTime) {

        ApplyFreezeFrames(5);

        if (hasHoverArmor && hovering)
            return;

        hovering = false;

		//targetVelocityX = (direction.normalized * magnitude * weightFactor).x;
		velocity = (direction.normalized * magnitude * weightFactor);

        
        //hat.currentThrowCount = 999;
        GetStunned(coolDownTime);

        if (hat.isCurrentlyAttached) {
			hat.BlowBack(-direction, magnitude * weightFactor * 0.0125f / 1.5f); //what % of player blow back is applied to the hat
        }

        //v.Vibrate (10f, 10f, 0.25f);

        hat.DisableRepeatedClickAbility();

    }

    bool lastWallSliding = false;
    void HandleWallSliding() {

        lastWallSliding = wallSliding;

        wallDirX = (controller.collisions.left) ? -1 : 1;
        wallSliding = false;

		if (stunned || hovering) return;

        if (controller.collisions.below) return;

        if (((controller.collisions.left) || (controller.collisions.right)) && !controller.collisions.below && velocity.y < 0 && directionalInput.y < 0.9f && !isHovering) {
            wallSliding = true;
            hovering = false;

            if (velocity.y < -wallSlideSpeedMax) {
                velocity.y = -wallSlideSpeedMax + ((Mathf.Abs(Mathf.Clamp(directionalInput.y, -1, 0)) * (wallSlideFastFallFactor - 1)) * -wallSlideSpeedMax);
            }

            if (timeToWallUnstick > 0) {
                velocityXSmoothing = 0;
                velocity.x = 0;

				if (Mathf.Sign(directionalInput.x) != wallDirX && Mathf.Abs(directionalInput.x) >= 0.3f) {
                    timeToWallUnstick -= Time.fixedDeltaTime;
                } else {
                    timeToWallUnstick = wallStickTime;
                }
            } else {
                wallSliding = false;
                timeToWallUnstick = wallStickTime;
            }

        }

    }

    bool gotBoost = false;
	float targetVelocityX;
	float targetVelocityY;
    void CalculateVelocity() {

        if (stunned) {

            velocity.x = Mathf.SmoothDamp(velocity.x, 0, ref velocityXSmoothing, accelerationTimeStun);

            if (Time.time > endStunTime - 0.3f) {

                if (recovering) {

                    if (controller.collisions.below)
                        recovering = false;

                    float stunGradient = Time.time - endStunTime;

                    //targetVelocityX = directionalInput.x * moveSpeed;
                    velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, Mathf.Lerp(accelerationTimeStun, accelerationTimeGrounded, stunGradient * 0.3f));

                    if (stunGradient > 0.3f) {

                        recovering = false;
                        EndStun();
                    }

                } else {
                    recovering = true;
                }

            }

            if (controller.collisions.left || controller.collisions.right) {
                velocity.y += Mathf.Sign(velocity.y) * Mathf.Abs(velocity.x) * 0.3f;
                velocity.x = (isStunned) ? -velocity.x * 0.3f : -velocity.x * 0.5f; //what % of your speed you retain after befoing knocked against the wall
                targetVelocityX = -targetVelocityX;

                //play rebound sound
                AudioManager.instance.PlaySound("Rebound", Vector3.zero);

            }


            velocity.y += gravity * 0.85f * Time.fixedDeltaTime;
            if (velocity.y < -terminalVelocity)
                velocity.y = -terminalVelocity;

        } else if (teleporting) {

            teleportTimer -= Time.fixedDeltaTime;

            switch (teleportState) {

                case TeleportState.PRE_HANG:

                    velocity = Vector3.zero;

                    if(teleportTimer <= 0) {
                        teleportState = TeleportState.TELEPORTING;
                        teleportTimer = teleportTime;
                        controller.SetDeadMask();
                    }

                    break;

                case TeleportState.TELEPORTING:

                    velocity = teleportDirection * teleportMoveSpeed;

                    if (teleportTimer <= 0) {
                        teleportState = TeleportState.POST_HANG;
                        teleportTimer = teleportHangTime;
                        controller.SetAliveMask();
                        PlayTeleportEffect();
                    }

                    break;

                case TeleportState.POST_HANG:

                    velocity = Vector3.zero;

                    if (teleportTimer <= 0) {
                        EndTeleport();
                    }

                    break;
                default:
                    break;
            }

            


        } else if (dashing) {

            if (controller.collisions.left || controller.collisions.right) {
                StopDashing();
            }

            currentDashSpeed = Mathf.SmoothDamp(currentDashSpeed, targetDashSpeed, ref currentDashSpeedSmoothing, (targetDashSpeed != 0) ? accelerationTimeDash : deccelerationTimeDash / 1.5f);
            velocity = new Vector2(targetDashDirection.x * currentDashSpeed, velocity.y);

            if (velocity.y < 1f)
                velocity.y = -0.5f;

            if ((faceDir > 0) ? controller.collisions.right : controller.collisions.left) {

                //StopDashing ();
                //velocity = new Vector2 (-velocity.x, 40f);
                //velocity /= 3;

            }

        } else if (hovering) {

            if (canHoverX && canHoverY) directionalInput.Normalize();


            if (canHoverX) {

                targetVelocityX = directionalInput.x * ((controller.collisions.below ? moveSpeed : airMoveSpeed) + (hat.isCurrentlyAttached ? 0 : hatlessMoveBonus) + (isHovering ? hoverMoveBonusX : 0));
                velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, accelerationTimeHover);

            } else {
                velocity.x = 0;

            }

            if (canHoverY) {

                if (directionalInput.y < 0.3 && canHoverJump) {

                    targetVelocityY = hoverImpulse;
                    hoverImpulseTimer += Time.fixedDeltaTime;

                    if (hoverImpulseTimer > totalHoverImpulseTime) {
                        canHoverJump = false;
                        hoverImpulseTimer = 0;
                    }


                } else {

                    targetVelocityY = directionalInput.y * ((controller.collisions.below ? moveSpeed : airMoveSpeed) + (hat.isCurrentlyAttached ? 0 : hatlessMoveBonus) + (isHovering ? hoverMoveBonusY : 0));

                }

                velocity.y = Mathf.SmoothDamp(velocity.y, targetVelocityY, ref velocityYSmoothing, accelerationTimeHover);


            } else {
                velocity.y = 0;
            }


            if (timeToStopHovering > 0.0f) {
                timeToStopHovering -= Time.fixedDeltaTime;
            } else {
                hovering = false;

                if (canHoverY)
                    velocity.y = Mathf.Sign(velocity.y);
            }

        } else {

            targetVelocityX = directionalInput.x * ((controller.collisions.below ? moveSpeed : airMoveSpeed) + (hat.isCurrentlyAttached ? 0 : hatlessMoveBonus) + (isHovering ? hoverMoveBonusX : 0));

            if (controller.collisions.below && isSliding) targetVelocityX *= ICE_FRICTION_MODIFIER;

            //should not dash if the player is collising 
            if (!controller.collisions.left && !controller.collisions.right) {

                if (((velocity.x == 0 && targetVelocityX != 0)) && controller.collisions.below && Mathf.Abs(directionalInput.x) > 0.6f) {
                    StartDashing();
                } else if (Mathf.Sign(directionalInput.x) != Mathf.Sign(velocity.x) && directionalInput.x != 0 && controller.collisions.below && Mathf.Abs(directionalInput.x) > 0.6f) {
                    StartDashing();
                } else if (Mathf.Sign(directionalInput.x) == Mathf.Sign(velocity.x) && Mathf.Abs(velocity.x) < Mathf.Abs(moveSpeed * 0.9f) && controller.collisions.below && Mathf.Abs(directionalInput.x) > 0.8f && !gotBoost) {
                    StartDashing();
                    gotBoost = true;
                }

            }

            if (Mathf.Sign(directionalInput.x) != Mathf.Sign(velocity.x) || Mathf.Abs(directionalInput.x) < 0.2f) gotBoost = false;

            velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne);

            if (velocity.y > 0 && (isPressingA || isBouncing)) {
                velocity.y += gravity * Time.fixedDeltaTime;
            } else {

                isBouncing = false;
                float isFastFalling = (directionalInput.y <= -0.4) ? 1 : 0;
                velocity.y += (gravity + (isFastFalling * (fastFallFactor - 1)) * gravity) * Time.fixedDeltaTime;

                if (isFastFalling != 0 && !controller.collisions.below)
                    fastFalling = true;
            }

            if (velocity.y < (fastFalling ? -terminalVelocityFastFall : -terminalVelocity)) {
                velocity.y = (fastFalling ? -terminalVelocityFastFall : -terminalVelocity);

                if (usingDifferentFallSpeeds && directionalInput.y >= -0.4) {
                    velocity.y = -terminalVelocity;
                    fastFalling = false;
                }

                PlayFastFallEffect();

            }

        }

    }

    void Kill(Player otherPlayer) {
        otherPlayer.DieAtTheHandsOf(this);
        kills.Add(otherPlayer.name);        

    }

    public virtual void DieAtTheHandsOf(Player otherPlayer) {
        PlayDeathEffect();

        //wasKilled = true;
        hat.gameObject.SetActive(false);
        controller.SetDeadMask();

        killedBy.Add(otherPlayer.name);
        
        if(otherPlayer != this)
            otherPlayer.kills.Add(name);

        GameController.instance.game.LoseLife(playerIndex);

        BeginDeathFreeze();

        if (otherPlayer.playerIndex == -1) {
            Dummy d = (Dummy) otherPlayer;
            d.AIGetsAKill();

            /*
            if(GameController.instance.game.currentGameMode == Game.Mode.SURVIVAL) {
                GameController.instance.gameModeUI.EnableUI();
            }*/

        }

        PlayFlyingSoul();

    }

    bool deathFreeze;
    float deathFreezeTime;
    protected void BeginDeathFreeze() {
        deathFreeze = true;
        deathFreezeTime = Time.time + 0.5f;
    }

    public void BeginInvulnerability() {
        invulnerable = true;

        invulnerabilityTimer = Time.time + respawnInvulnerabilityTime;
    }

    public void EndInvulnerability() {
        invulnerable = false;
    }

    public int GetNumberOfKills() {
        return kills.Count;
    }

    public int GetNumberOfDeaths() {
        return killedBy.Count;
    }

    public int GetNumberOfThrowsThisGame() {
        return hat.GetNumberOfThrowsThisGame();
    }

    public float GetAccuracy() {
        return hat.CalculateAccuracy();
    }

    public void Respawn() {

        BeginInvulnerability();

        hat.gameObject.SetActive(true);
        gameObject.SetActive(true);
        hat.transform.position = transform.position;
        Reset();

    }

    public void Reset() {

        velocity = Vector2.zero;
        hat.Reset();

        controller.SetAliveMask();
        controller.collisions.Reset();
        wasHit = false;
		canPickUpHat = true;
        canHurtHat = true;
        canKillHat = true;

        stunStars.SetActive(false);
        //GameController.instance.game.currentPlayers[playerIndex].UpdateStockUI();

    }

    public void PlayDeathEffect() {
        /*
		GameObject deathPaint = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Death_Paint"), transform.position, Quaternion.identity);
		GameObject deathPaintHat = MonoBehaviour.Instantiate(Resources.Load<GameObject>("Death_Paint"), hat.transform.position, Quaternion.identity);
		deathPaintHat.transform.localScale = Vector3.one * 5f;
        deathPaint.transform.parent = transform.root;
        deathPaintHat.transform.parent = transform.root;
        */

        //hat
        Vector3 spawnPosition;

		if (hat.isCurrentlyAttached || hat.isBeingAttached)
			spawnPosition = transform.position - (Vector3.one * 0.7f);
		else
			spawnPosition = hat.transform.position;


		var firstBlastEmitParams = new ParticleSystem.EmitParams();
		VFXManager.instance.EmitAtPosition("Hat_Explosion", firstBlastEmitParams, 1, spawnPosition + Vector3.up * 1.5f, true);
        /*
		var deathWind = new ParticleSystem.EmitParams();
		deathWind.startSize3D = new Vector3(Random.Range(80, 100), Random.Range(30, 50), 1);
		VFXManager.instance.EmitAtPosition("Death_Wind", deathWind, 1, spawnPosition, true);
        */
		var deathSkullEmitParams = new ParticleSystem.EmitParams();
		deathSkullEmitParams.velocity = new Vector3(0, 2, 0);
		VFXManager.instance.EmitAtPosition("Death_Skull", deathSkullEmitParams, 1, hat.transform.position + Vector3.up * 2.5f, false);

		//char
		VFXManager.instance.EmitAtPosition("Death_Insignia_Explosion", 1, transform.position + Vector3.up, false);

	}

    IEnumerator SecondDeathCircle() {
        float t = 0.0f;
        while (t < 0.125f) {
            t += Time.deltaTime;
            yield return null;
        }
        VFXManager.instance.EmitAtPosition("Death Circle", 1, transform.position + Vector3.down * 0.75f, false);
    }

    public void PlayFastFallEffect() {

        var waveEmitParams = new ParticleSystem.EmitParams();
        waveEmitParams.startLifetime = 0.1f;
        waveEmitParams.startSize3D = new Vector3(8, 3, 1);
        waveEmitParams.rotation3D = new Vector3(0, 0, -90);

    }

    public void PlayJumpParticles() {

        var jumpEmitParams = new ParticleSystem.EmitParams();
        jumpEmitParams.rotation3D = new Vector3(0, 0, Mathf.Abs(velocity.x * 2f) * faceDir);

        if (jumpsCompleted > 0) {
            var circleEmitParams = new ParticleSystem.EmitParams();
            circleEmitParams.rotation3D = new Vector3(0, 0, Mathf.Abs(velocity.x * 2f) * faceDir);
            //circleEmitParams.velocity = new Vector3(0, -1, 0);
            VFXManager.instance.EmitAtPosition("Double_Jump_Circle", circleEmitParams, 1, transform.position + Vector3.down * 0.5f, false);

            //jumpEmitParams.velocity = new Vector3(0, 2, 0);
            VFXManager.instance.EmitAtPosition("Double_Jump_Dust", jumpEmitParams, 1, new Vector3(transform.position.x, transform.position.y - 0.5f, 0), false);
        } else { //transform.position + Vector3.up * 0.5f
            VFXManager.instance.EmitAtPosition("Jump_Dust", jumpEmitParams, 5, new Vector3(transform.position.x, controller.raycastOrigins.bottomLeft.y - 0.1f, 0) + (Vector3.up), false);
        }

    }

    void PlayFlyingSoul() {

        //GameObject newSoul = Instantiate(soulPrefab, transform.position, Quaternion.identity) as GameObject;

    }

    void PlayTeleportEffect() {

        VFXManager.instance.EmitAtPosition("Teleport", 1, transform.position + Vector3.up * 0.5f, false);

    }

}