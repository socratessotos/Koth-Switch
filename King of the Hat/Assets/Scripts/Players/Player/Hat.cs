
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (HatController))]
public class Hat : MonoBehaviour {

	public const float THROW_SPEED_CUTOFF_X = 0.35f;
	public const float THROW_SPEED_CUTOFF_Y = 0.1f;
    public const float GRAVITY_HEIGHT_MODIFIER = 0.5f;
    public const float MAX_SPEED_X = 1.5f;
    public const float MAX_SPEED_Y = 1.5f;

    public LayerMask solidMask;

	[Header("Connected Objects")]
	public Player owner;
	public SpriteRenderer spriteRenderer;
	public Transform target;

    [Header("Hat VFX")]
    public GameObject firePrefab;
    [HideInInspector]
    public ParticleSystem dangerFire;
    [HideInInspector]
    public ParticleSystem dangerFireFront;
    float initialFireSize;

    TrailRenderer trail;

    [Header("Basic Movement")]
	public float followSpeed;
	public float maxRotation;
    [Space(10)]
	public float gravity;
    [Space(10)]
	public bool splitFriction;
	public float dangerousFriction;
	public float normalFriction;
    [Space(10)]
    public bool splitDrag;
	public float dangerousDrag;
    public float normalDrag;
    [Space(10)]
	public float maxDistance;
    public float bounceDampening = 0.6f;

    [Header ("Hat Throwing")]
	public float throwPower = 1f;
	public float throwTime = 0.3f;
	public float throwBlowBack = 30f;
	public float throwStunTime = 0.2f;
	public int minThrowHits = 1;
	public int maxThrowHits = 10;
    public bool increasesStunOverTime = false;

    [Header("Properties")]
    public int maxHp = 3;
    public int currentHp;

    [Header("Size Throws")]
    public bool throwsChangeSize = false;
    public float throwSizeScale = 1f;
    Vector3 initialHatSize;
    Vector3 finalHatSize;
    bool isChangingSize = false;
    float changeSizeTimer = 0;
    float changeSizeTime = 10;

    [Header("Dance Properties")]
    public float yImpulse = 0.1f;
    public float xImpulse = 0f;
    int impulseDir = 1;

    [Header("Lava Impulse")]
    public float yLavaImpulse = 0.2f;
    public float xLavaImpulse = 0.2f;

    [Header("Repeated Ability")]
    //public int maxNumberOfClicks = 1;
    //int currentClick = 0;
    public RepeatedClickAbility repeatedClickAbility = RepeatedClickAbility.NONE;
    public enum RepeatedClickAbility { NONE, SECOND_THROW, SHOCKWAVE, VERTICALLINK, POOPJELLY }

    [Header("On Danger Finish Ability")]
    public DangerFinishAbility dangerFinishAbility = DangerFinishAbility.NONE;
    public enum DangerFinishAbility { NONE, SHOCKWAVE }


    float throwY;
	float throwHeight;

	[HideInInspector]
	public Vector2 lastVelocity;
	[HideInInspector]
	public Vector2 lastPlayerVelocity;
	[HideInInspector]
	public Vector2 velocity;

	HatController controller;
	bool isAttached = true;
	Transform sprite;
	BoxCollider2D col;

	int freezeFrames = 0;
	int numberOfHits = 1;
	int currentHit = 0;

    bool hitAlready = false;
    int numberOfThrowsThisGame = 0;
    int numberOfHitsThisGame = 0;

	float endAttachingTime = 0;

	Vector3 initialSpriteSize;
	Vector3 initialSpritePosition;
	bool shouldStretch = true;
	bool isThrown = false;
	bool fellOff = false;
    bool isComingBack = false;
    bool canComeBack = true;
	bool hitFreeze;
    float hitFreezeLength;
	bool isAttaching = false;
	bool canStickThrow = true;

	public bool isCurrentlyAttached { get { return isAttached; } set { isAttached = value; }}
	public bool isBeingThrown {get {return isThrown;}}
    public bool isCurrentlyComingBack { get { return isComingBack; } set { isComingBack = value; } }
    public bool isAbleToComeBack { get { return canComeBack; } set { isComingBack = value; } }
	public bool isFallingOff { get { return fellOff; } }
	public bool isHitFrozen { get { return hitFreeze; } set { hitFreeze = value; } }
	public bool isBeingAttached {get {return isAttaching; } set { isBeingAttached = value; } }
    public bool hasHitAlready { get { return hitAlready; } set { hitAlready = value; } }
	public bool isDying { get { return dying; } set { hitAlready = dying; } }
    public float initialThrowY { get { return throwY; } set { throwY = value; } }
    //public int currentHatClick { get { return currentClick; } set { currentClick = value; } }
    public bool hasIncreasingStunOverTime { get { return increasesStunOverTime; } }
    public float currentThrowTimer { get { return throwTimer; } set { throwTimer = value; } }

	bool dying = false;

    public enum FireSize { SMALL, MEDIUM, LARGE};

	void OnEnable () {

		StopCoroutine ("Expand");
        StopCoroutine("HitFlash");
        StopCoroutine("VulnerableFlash");
        trail.enabled = false;

		sprite.localPosition = initialSpritePosition;
		sprite.localScale = initialSpriteSize;
		
	}

    void Awake () {

		gameObject.layer = transform.parent.gameObject.layer;

		sprite = transform.GetChild (0);
        trail = sprite.GetComponent<TrailRenderer>();
		initialSpriteSize = sprite.localScale;
		initialSpritePosition = sprite.localPosition;
		controller = GetComponent <HatController> ();
		col = GetComponent <BoxCollider2D> ();

    }

    void Start () {
		
		gameObject.layer = owner.gameObject.layer;
		transform.SetParent (owner.transform.parent, true);
		ResetPosition ();

        initialHatSize = transform.localScale;
        finalHatSize = initialHatSize * throwSizeScale;
        currentHp = maxHp;

        CreateFirePrefab();

    }

    void FixedUpdate () {

		if (GameController.instance.game.state == Game.State.PAUSED) return;

		if (dying) return;
        
        //hit freeze
		if (hitFreeze) {
            
			if (freezeFrames < hitFreezeLength) {
                freezeFrames++;
			} else {
				freezeFrames = 0;
				hitFreeze = false;
			}

			return;
		}

        if(controller.collisions.shouldBurn) {

            velocity.y = yLavaImpulse;
            velocity.x = (velocity.x <= 1 && velocity.x >= -1) ? (controller.collisions.faceDir * xLavaImpulse) : velocity.x * 1.5f;    

            controller.collisions.shouldBurn = false;

        }

        //changing size
        if(isChangingSize) {
            
            transform.localScale = Vector3.Lerp(startSize, endSize, changeSizeTimer / changeSizeTime);
            
            var fireEmitParams = new ParticleSystem.EmitParams();
            var fireFrontEmitParams = new ParticleSystem.EmitParams();

            fireEmitParams.startSize = Mathf.Lerp(4, 4 * throwSizeScale, changeSizeTimer / changeSizeTime);
            fireFrontEmitParams.startSize = Mathf.Lerp(3, 3 * throwSizeScale, changeSizeTimer / changeSizeTime);

            if (changeSizeTimer < changeSizeTime) {
                changeSizeTimer++;
            } else {
                changeSizeTimer = 0;
                isChangingSize = false;
            }

        }

        if(!isAttached && !isAttaching && !isThrown && controller.collisions.below && velocity.y == 0 && velocity.x <= 0.05f) {

            velocity.y += yImpulse;

            velocity.x += xImpulse * impulseDir;
            impulseDir = -impulseDir;

        } else {

            impulseDir = (int) velocity.normalized.x;
        
        }

		if (isThrowAllowed () && canStickThrow && (Mathf.Abs(owner.GetDirectionalThrowInput().x) >= 0.5f || Mathf.Abs(owner.GetDirectionalThrowInput().y) >= 0.5f)) {
			Throw (new Vector2 (
				InputController.ConstrainAxisTo16Angles (owner.GetDirectionalThrowInput().x),
				InputController.ConstrainAxisTo16Angles (owner.GetDirectionalThrowInput().y)
			), 0);

			canStickThrow = false;
		}

		if (!canStickThrow && (Mathf.Abs(owner.GetDirectionalThrowInput().x) <= 0.2f && Mathf.Abs(owner.GetDirectionalThrowInput().y) <= 0.2f)) {
			canStickThrow = true;
		}

		CalculatePosition ();
		CalculateRotation ();

		//CountDownThrowTimer ();

	}

	void LateUpdate () {
		
	}

	public void CalculateRotation () {

		if (isAttached) {

			//transform.rotation = Quaternion.Slerp (Quaternion.identity, Quaternion.Euler (0, 0, Mathf.Sign (velocity.x) * maxRotation), Mathf.Abs ((transform.position - target.position).x) / 1);
			float angle = Mathf.Lerp (0, Mathf.Sign ((target.position.x - transform.position.x)) * maxRotation, Mathf.Abs ((target.position.x - transform.position.x)));
			if (Mathf.Abs (angle) >= maxRotation && isAttaching)
				angle =  -Mathf.Sign ((target.position.x - transform.position.x)) * maxRotation;

			if (!isAttaching) {
				
				transform.localRotation = Quaternion.Euler(0, 0, angle);
				transform.GetChild(0).localRotation = Quaternion.identity;

			} else {
				transform.GetChild(0).localRotation = Quaternion.Euler(0, 0, angle);
				transform.localRotation = Quaternion.identity;

			}
			
		} else {

			transform.localRotation = Quaternion.identity;

			if (controller.collisions.below) {
				
				transform.GetChild(0).localRotation = Quaternion.identity;

			} else {
				
				transform.GetChild(0).localRotation =  Quaternion.Slerp (transform.rotation, Quaternion.identity, Time.deltaTime);

			}

		}
		
	}

	//variables for checking if there are solids between the hat and its owner
	Vector3 startPoint;
	Vector3 endPoint;
	float length;

	public void CalculatePosition () {

		startPoint = transform.position + new Vector3 (0f, transform.localScale.y/2, 0f);
		endPoint = ((target.position) - startPoint);
		length = (startPoint - target.position).magnitude;

		Debug.DrawRay(startPoint, endPoint, Color.black);

		if (isAttached) {

			//velocity = (Vector3.Lerp (transform.position, (target.position), Time.fixedDeltaTime * followSpeed) - transform.position);	
            //if (isAttaching) velocity = (target.position - transform.position) * Time.fixedDeltaTime * followSpeed / 2;
            /*else*/ velocity = (target.position - transform.position) * Time.fixedDeltaTime * followSpeed;
            
            spriteRenderer.flipX = owner.faceDir < 0;

			if (Physics2D.Raycast(startPoint - new Vector3 (0f, transform.localScale.y/2, 0f), endPoint, length, solidMask) && !isAttaching) {
					isAttached = false;
			}

			if (Mathf.Abs(target.position.x - transform.position.x) <= velocity.x) {
				velocity.x = 0;
			}

			if (Mathf.Abs(target.position.y - transform.position.y) <= velocity.y) {
				velocity.y = 0;
			}

			Vector3 rightCheck = startPoint + new Vector3 (col.size.x /2 * 1.25f, 0, 0);
			Vector3 rightEnd = ((target.position) - rightCheck);
			float rightLength = rightEnd.magnitude;

			if (Physics2D.Raycast (rightCheck + Vector3.up * 1.6f/4, rightEnd, rightLength, solidMask)
				|| Physics2D.Raycast (rightCheck - Vector3.up * 1.6f/2, rightEnd, rightLength, solidMask)) {

				velocity += new Vector2 (-2, -1) * Time.fixedDeltaTime * followSpeed/2;
			}

			Vector3 leftCheck = startPoint + new Vector3 (-col.size.x /2 * 1.25f, 0, 0);
			Vector3 leftEnd = ((target.position) - leftCheck);
			float leftLength = leftEnd.magnitude;

			if (Physics2D.Raycast (leftCheck + Vector3.up * 1.6f/4, leftEnd, leftLength, solidMask)
				|| Physics2D.Raycast (leftCheck - Vector3.up * 1.6f/2, leftEnd, leftLength, solidMask)){

				velocity += new Vector2 (2, -1) * Time.fixedDeltaTime * followSpeed/2;
			}

			Debug.DrawRay (rightCheck, rightEnd, Color.blue);
			Debug.DrawRay (leftCheck, leftEnd, Color.blue);




		} else {
            
            if (controller.collisions.shouldReflectY) {
                velocity.y = -velocity.y * bounceDampening;
            }

            if (controller.collisions.shouldReflect) {
				
//				if (controller.collisions.isHittingPlayer && currentHit < numberOfHits) {
//					currentHit++;
//				
//				} else {
//
//				}

				if (controller.collisions.hitHat) {
					velocity.x = -velocity.x * bounceDampening;
					
				} else if (!collisionSkip){
					velocity.x = -velocity.x * bounceDampening;	
				} else {
					collisionSkip = false;
				}

				currentHit = 1000;
				numberOfHits = 999;
					
				transform.rotation = Quaternion.identity;
				if (Mathf.Abs (velocity.x) > 0.001f) {

                    PlayCollisionParticles(velocity);

                }

            }

            if (isThrown) {

                // [NEW] PlayHatFire(velocity);
                UpdateTrailAndFire(velocity);

                throwTimer += Time.deltaTime;

                if (ThrowIsOver ()) {
                    EndThrow();
                    ActivateOnDangerFinishAbility();
                    PlayThrowTimeDoneParticles(velocity);
                }


            }

			if (Mathf.Abs (velocity.x) > dangerousFriction * Time.deltaTime) {
				velocity.x -= Mathf.Sign (velocity.x) * ((controller.collisions.below) ? ((splitFriction && isBeingThrown) ? dangerousFriction : normalFriction) : ((splitDrag && isBeingThrown) ? dangerousDrag : normalDrag)) * Time.fixedDeltaTime;

                if(controller.collisions.below && Mathf.Abs(velocity.x) > 0.2f)
                    VFXManager.instance.EmitAtPosition("Collision_Stars", 2, transform.position + Vector3.right * 0.5f * controller.collisions.faceDir, true);

            } else {
				velocity.x = 0.0f;
			}

			velocity.y -= (gravity * Time.fixedDeltaTime) + ((transform.position.y > throwY + throwHeight && velocity.y > -5) ? (gravity * ((Mathf.Abs(transform.position.y - (throwY + throwHeight))) * GRAVITY_HEIGHT_MODIFIER) * Time.fixedDeltaTime) : 0);

			if (canComeBack && !owner.isStunned && GetDistanceToOwner () <= maxDistance && !isHitFrozen) {

				if ((Physics2D.Raycast(startPoint, endPoint, length, solidMask))) {
					isAttached = false;
                    
				} else {

					if (owner.canPickUpHat && !owner.isStunned) {

                        GrabHatBack();

                        //GetComponent<HatPersonality>().StopDancing();

					}

				}

            }

            if (controller.collisions.hitHat) {

                StopCoroutine("HitFlash");
                StartCoroutine("HitFlash");

                controller.collisions.hitHat = false;

            }

        }

		if (isAttaching) {
            
			if (Time.time > endAttachingTime) {
				isAttaching = false;
            }

        }

        //max speed check
        if (Mathf.Abs(velocity.x) > MAX_SPEED_X) velocity.x = MAX_SPEED_X * Mathf.Sign(velocity.x);
        if (Mathf.Abs(velocity.y) > MAX_SPEED_Y) velocity.y = MAX_SPEED_Y * Mathf.Sign(velocity.y);

        //move
        controller.Move (velocity);

		if (controller.collisions.below) {
			velocity.y = 0f;
		}

		if (controller.collisions.above) {
			velocity.y = -0.01f;
		}

		Squash (velocity, lastVelocity);

		if ((int)(velocity.x * 100) != (int)(lastVelocity.x * 100))
			lastVelocity.x = velocity.x;

		if ((int)(velocity.y * 100) != (int)(lastVelocity.y * 100))
			lastVelocity.y = velocity.y;

	}

    public void GrabHatBack() {

        owner.ActivateOnPickUpAbility();

        LandOnHead();
        isAttached = true;
        EndThrow();

        RefreshRepeatedClickAbility();
        //currentClick = 0;

    }

    public bool isThrowAllowed() {
		return (!owner.isStunned && !hitFreeze && isCurrentlyAttached 
        && (owner.teleportState != Player.TeleportState.TELEPORTING));
        
    }

    float initialXVelocity;
    float initialYVelocity;

	[HideInInspector]
	public bool collisionSkip = false;
    public virtual void Throw (Vector2 directionalInput, float chargeTime) {
        if (isComingBack) return;

        if (throwsChangeSize)
            TriggerSizeChange(1);

        //PlayThrowImpulseParticles(directionalInput);
        ToggleHatVFX(true);
        trail.enabled = true;

		collisionSkip = true;

        //currentThrowCount++;
        numberOfThrowsThisGame++;
        hitAlready = false;

        numberOfHits = (int) Mathf.Lerp (minThrowHits, maxThrowHits, chargeTime / 2f);

		transform.rotation = Quaternion.identity;
            
        float extraPower = 0f;
        if(owner.GetVelocity().y < 0 && directionalInput.y < 0) {
            extraPower = owner.GetVelocity().magnitude / 200;
        }

		bool isWeak = Mathf.Abs(owner.GetDirectionalInput().x) < 0.7f && Mathf.Abs(owner.GetDirectionalInput().x) > 0.0f
			&& Mathf.Abs(owner.GetDirectionalInput().y) < 0.7f && Mathf.Abs(owner.GetDirectionalInput().y) > 0.0f;

        canComeBack = false;
		isThrown = true;
		isAttached = false;
		isAttaching = false;

		velocity = ((directionalInput != Vector2.zero) ? (directionalInput.normalized) : (new Vector2 (owner.faceDir, 0)));
		//if (owner.isWallSliding) velocity.x = -velocity.x;
        velocity *= (throwPower / 10) + extraPower;
		
        if (isWeak)
			velocity /= 2;
		throwY = owner.transform.position.y;

        initialXVelocity = velocity.x;
        initialYVelocity = velocity.y;

        owner.EndInvulnerability();

        //if (currentThrowCount == 1 || resetsDangerTimer)
        //    ResetThrowTimer ();
        throwTimer = 0;

		//play throw sound
		AudioManager.instance.PlaySound ("Throw", Vector3.zero);
        Invoke("AllowToComeBack", 0.25f);

	}

    public void ActivateRepeatedClickAbility() {
        
        switch(repeatedClickAbility) {

            case RepeatedClickAbility.NONE:
                break;

            default:
                GetComponent<RepeatedClickAbilities>().NextClick();
                break;

        }

    }

    void RefreshRepeatedClickAbility() {

        switch (repeatedClickAbility) {
    
            case RepeatedClickAbility.NONE:
                break;
          
            default:
                GetComponent<RepeatedClickAbilities>().RefreshAbility();
                break;

        }

    }

    public void DisableRepeatedClickAbility() {

        switch (repeatedClickAbility) {

            case RepeatedClickAbility.NONE:
                break;

            default:
                GetComponent<RepeatedClickAbilities>().DisableAbility();
                break;

        }

    }

    void ActivateOnDangerFinishAbility() {

        switch (dangerFinishAbility) {

            case DangerFinishAbility.NONE:
                break;

            case DangerFinishAbility.SHOCKWAVE:

                break;

            default:
                break;

        }

    }

    Vector3 startSize;
    Vector3 endSize;
    void TriggerSizeChange(int _direction) {

        isChangingSize = true;
        changeSizeTimer = 0;

        startSize = (_direction > 0) ? initialHatSize : finalHatSize;
        endSize = (_direction > 0) ? finalHatSize : initialHatSize;

    }

    void PlayHatFire(Vector2 direction) {

		var waveEmitParams = new ParticleSystem.EmitParams();

		dangerFire.Emit(waveEmitParams, 1);
		dangerFireFront.Emit(waveEmitParams, 1);

    }

    void ToggleHatVFX(bool _isDangerous) {

        ToggleFire(_isDangerous);
        ToggleHatGlow(_isDangerous);

    }

    public virtual void ToggleHatGlow(bool _isDangerous) {

        Color32 _hatGlowColor = PlayerColorManager.instance.players[owner.playerIndex].hatGlowColor;
        Color32 _hatColor = (_isDangerous) ? _hatGlowColor : new Color32(0, 0, 0, 0);
        
        spriteRenderer.material.SetColor("_OverrideColor", _hatColor);

    }

    public void ToggleFire(bool _isDangerous) {
        if (dangerFire == null) return;
        ParticleSystem.EmissionModule em = dangerFire.emission;
        em.enabled = _isDangerous;
    }


    int fireCounter = 0;
    void UpdateTrailAndFire(Vector2 direction) {

        if (fireCounter++ > 1000)
            fireCounter = 0;

        float throwTimeRatio = throwTimer / throwTime;
        float xSpeedRatio = (Mathf.Abs(velocity.x) - THROW_SPEED_CUTOFF_X) / (Mathf.Abs(initialXVelocity) - THROW_SPEED_CUTOFF_X);
        float ySpeedRatio = (Mathf.Abs(velocity.y) - THROW_SPEED_CUTOFF_Y) / (Mathf.Abs(initialYVelocity) - THROW_SPEED_CUTOFF_Y);

        //get the angle of the throw
        float angle = Mathf.Atan2(-direction.normalized.y, direction.normalized.x) * Mathf.Rad2Deg;

        //add 180 to compensate for the initial offset
        angle += 180;

        var waveEmitParams = new ParticleSystem.EmitParams();
        waveEmitParams.rotation3D = new Vector3(0, 0, angle);


        //if (fireCounter % 2 == 0)
        dangerFire.Emit(waveEmitParams, 1);
        dangerFireFront.Emit(waveEmitParams, 1);

    }

    public void AllowToComeBack() {
        canComeBack = true;
    }
		
	Vector2 initialDirection;
	public bool ThrowIsOver () {
        if (Mathf.Abs(velocity.x) < THROW_SPEED_CUTOFF_X && velocity.y < 0 && Mathf.Abs(velocity.y) < THROW_SPEED_CUTOFF_Y * 3) return true;
		return (Mathf.Abs(velocity.x) < THROW_SPEED_CUTOFF_X && Mathf.Abs(velocity.y) < THROW_SPEED_CUTOFF_Y);
    }

	public void EndThrow () {
        throwTimer = 0;
		isThrown = false;

        if (throwsChangeSize)
            TriggerSizeChange(-1);

        ToggleHatVFX(false);

        if (!isAttached) {
            StopCoroutine("VulnerableFlash");
            StartCoroutine("VulnerableFlash");

        }

    }

	public void EndFallOff () {
		fellOff = false;
	}

	public void BlowBack (Vector3 direction, float magnitude) {

        fellOff = true;
		isAttached = false;
		isAttaching = false;
		isThrown = false;

        DisableRepeatedClickAbility();

		velocity = direction.normalized * magnitude;

		Invoke ("EndFallOff", 0.25f);

        StopCoroutine("VulnerableFlash");
        StartCoroutine("VulnerableFlash");

        trail.enabled = true;

    }

	public void KnockBack (Vector3 direction, float magnitude) {

		velocity = direction.normalized * magnitude;
		
	}

    public void ApplyFreezeFrames(int _numberOfFrames) {
        hitFreezeLength = _numberOfFrames;
        hitFreeze = true;
    }

	public float GetDistanceToOwner () {
		return (target.position - transform.position).magnitude;
	}

	void Squash (Vector2 _velocity, Vector2 _lastVelocity) {

		if (!shouldStretch) return;

		float ax = Mathf.Abs (_velocity.x - _lastVelocity.x); //acc.x
		float ay = -Mathf.Abs (_velocity.y - _lastVelocity.y); //acc.y

		float squash = Mathf.Clamp ((ax + ay) * 3, -0.2f, 0.2f);
		sprite.localScale = initialSpriteSize + new Vector3(squash, -squash, 0);

	}

	public virtual void LandOnHead () {

        if (owner.isStunned) return;    

        StopCoroutine("VulnerableFlash");
        StartCoroutine("Expand");
        trail.enabled = false;
		EndThrow ();

		isAttaching = true;
        endAttachingTime = Time.time + 0.15f;

        //currentThrowCount = 0;

        RefreshRepeatedClickAbility();

        PlayLandOnHeadEffect();

		//play attach sound
		AudioManager.instance.PlaySound ("Attach", Vector3.zero);

    }

    void PlayLandOnHeadEffect() {

        float colliderHeight = owner.GetComponent<BoxCollider2D>().size.y/10f - owner.GetComponent<BoxCollider2D>().offset.y;
        float colliderHeightBack = owner.GetComponent<BoxCollider2D>().size.y / 4f - owner.GetComponent<BoxCollider2D>().offset.y;

        var energyEmitParams = new ParticleSystem.EmitParams();
        VFXManager.instance.EmitAtPosition("Pick_Up_Energy", energyEmitParams, 1, Vector3.down * colliderHeight, owner.teamNumber, 1, false, owner.transform);
        VFXManager.instance.EmitAtPosition("Pick_Up_Energy_White", 1, Vector3.down * colliderHeight, false, owner.transform);
        VFXManager.instance.EmitAtPosition("Pick_Up_Energy_Back", energyEmitParams, 1, Vector3.up * colliderHeightBack + Vector3.forward * 0.1f, owner.teamNumber, 1, false, owner.transform);

        /*
        VFXManager.instance.EmitAtPosition("Pick_Up_Effect", 1, target.position, false, target);


        StopCoroutine("SecondPickUpCircle");
        StartCoroutine("SecondPickUpCircle");
        */

    }

    IEnumerator SecondPickUpCircle() {
        float t = 0.0f;
        while (t < 0.1f) {
            t += Time.deltaTime;
            yield return null;
        }
        VFXManager.instance.EmitAtPosition("Pick_Up_Circle", 1, target.position, false, target);
    }

    IEnumerator Expand () {
        if (throwsChangeSize) yield break;    

		shouldStretch = false;

		Vector3 bigScale = initialSpriteSize * 1.75f;
		sprite.localScale = bigScale;

		float t = 0.0f;
		while (t < 1.0f) {
			
			sprite.localScale = Vector3.Lerp (bigScale, initialSpriteSize, t * 3);
			sprite.localPosition = new Vector3 (0, -0.5f, 0) + new Vector3 (0, sprite.localScale.y/2 + 0.5f, 0);
			t += Time.deltaTime;
			yield return null;
		
		}

		shouldStretch = true;
		
	}

    IEnumerator VulnerableFlash() {
        if (!shouldStretch) yield break;  

        //color variables
        float t = 0;
        bool goingRed = true;
        float colorSpeed = 2.5f;

        //size variables
        Vector3 bigScale = initialSpriteSize * 1.3f;

		while (!isAttached && !dying) {

            //color change
            byte alpha = (byte)Mathf.Lerp(0, 255, t);
            spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, alpha));

            //size change
            sprite.localScale = Vector3.Lerp(initialSpriteSize, bigScale, t);
            sprite.localPosition = new Vector3(0, -0.5f, 0) + new Vector3(0, sprite.localScale.y / 2 + 0.5f, 0);

            //color + size timer
            if (goingRed) {
                t += Time.deltaTime * colorSpeed;

                if (t >= 0.5f)
                    goingRed = false;

            } else {
                t -= Time.deltaTime * colorSpeed;

                if (t <= 0)
                    goingRed = true;

            }

            yield return null;

        }

    }

	float throwTimer = 0;

	void CountDownThrowTimer () {

		if (throwTimer >= 0) {
			throwTimer -= Time.fixedDeltaTime;
		} else {
			if (isThrown) {
				EndThrow ();
                PlayThrowTimeDoneParticles(velocity);
            }
        }

	}

	void ResetThrowTimer () {
		throwTimer = throwTime;
	}

    public float CalculateAccuracy() {
        if (numberOfThrowsThisGame == 0) return 0;
        return ((float) numberOfHitsThisGame / (float) numberOfThrowsThisGame) * 100;
    }

    public void ThrowHasHit() {
        numberOfHitsThisGame++;
    }

    public int GetNumberOfThrowsThisGame() {
        return numberOfThrowsThisGame;
    }

    public void LoseHatLife() {
        currentHp--;
        PlayHatLoseLifeParticles();
    }

    public void StartHitFlash() {
        StopCoroutine("HitFlash");
        StartCoroutine("HitFlash");
    }

    public IEnumerator HitFlash() {

        spriteRenderer.material.SetColor("_OverrideColor", new Color32(0, 0, 0, 225));
        yield return null;

        float t = 0;
        float speed = 25f;

        while (t < 1) {

            byte alpha = (byte)Mathf.Lerp(0, 255, t);
            spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, alpha));
            t += Time.deltaTime * speed;

            yield return null;

        }

        spriteRenderer.material.SetColor("_OverrideColor", new Color32(0, 0, 0, 200));
        yield return new WaitForSeconds(0.1f);

        spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, 225));
        yield return new WaitForSeconds(0.15f);

        spriteRenderer.material.SetColor("_OverrideColor", new Color32(255, 255, 255, 0));

    }

    public void KillOwner (Player otherPlayer) {
        if (!otherPlayer.canKillHat) return;

        owner.canKillHat = false;
		StartCoroutine (SquashOnDeath (otherPlayer));

	}

    public IEnumerator SquashOnDeath (Player otherPlayer) {

		float t = 0;
		float speed = 10f;

		shouldStretch = false;
		dying = true;

		Vector3 startPos = sprite.transform.position;

		sprite.GetComponent<SpriteRenderer> ().sortingLayerName = "Back_VFX";

		while (t < 1f) {

			float squash = 0 + t;
			sprite.localScale = initialSpriteSize + new Vector3(squash, -squash, 0);
			sprite.transform.position = startPos - new Vector3 (0, squash, 0);
			
			t += Time.deltaTime * speed;
			yield return null;

		}

        if(otherPlayer != this.owner)
		    otherPlayer.controller.collisions.shouldBounce = true;
			
		owner.DieAtTheHandsOf (otherPlayer);
		CameraFX.instance.PlayKIllPlayerAnimation ();
		ResetPosition();

		shouldStretch = true;
		dying = false;
		sprite.GetComponent<SpriteRenderer> ().sortingLayerName = "Players";

	}

    void ResetPosition () {
		transform.position = (target.position);
	}

	public void Reset () {

        velocity = new Vector2 ();
		ResetPosition ();
		isThrown = false;
		fellOff = false;
        currentHp = maxHp;
        ToggleHatVFX(false);

        controller.collisions.Reset ();

	}

    public void PlayThrowTimeDoneParticles(Vector2 direction) {

        //get the angle of the throw
        float angle = Mathf.Atan2(-direction.normalized.y, direction.normalized.x) * Mathf.Rad2Deg;

        var poofEmitParams = new ParticleSystem.EmitParams();
        
        //add 180 to compensate for the initial offset
        if (direction != Vector2.zero)
            angle -= 90;
        else
            angle = 0;

        poofEmitParams.rotation3D = new Vector3(0, 0, angle);
        poofEmitParams.velocity = direction.normalized;

        // [NEW] VFXManager.instance.EmitAtPosition("Fire_Done_Smoke", poofEmitParams, 20, transform.position + Vector3.up * 0.5f, owner.playerIndex, 0, true);
        // [NEW] VFXManager.instance.EmitAtPosition("Fire_Done_Smoke_Additive", poofEmitParams, 20, transform.position + Vector3.up * 0.5f, true);
        VFXManager.instance.EmitAtPosition("Fire_Done_Smoke", poofEmitParams, 1, transform.position + Vector3.up * 0.5f, false);

    }

    public void PlayHatLoseLifeParticles() {
        var hatLoseLifeEmitParams = new ParticleSystem.EmitParams();
     
        VFXManager.instance.EmitAtPosition("Hat_Lose_Life", hatLoseLifeEmitParams, 100, transform.position + Vector3.up * 0.5f, owner.teamNumber, 1, true);
        VFXManager.instance.EmitAtPosition("Hat_Lose_Life_Circle", hatLoseLifeEmitParams, 1, transform.position + Vector3.up * 0.5f, true);

    }

    public virtual void CreateFirePrefab() {
        GameObject f = Instantiate(firePrefab, transform.position, Quaternion.identity) as GameObject;
        f.transform.parent = transform;
        f.transform.position = transform.position + Vector3.up * 0.5f;
        f.transform.localScale = Vector3.one;

        //f.layer = LayerMask.NameToLayer("Player" + (owner.playerIndex + 1) + "Fire");

       //f.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("_OutlineColor", PlayerColorManager.instance.players[owner.playerIndex].playerColor);

        dangerFire = f.transform.GetChild(0).GetComponent<ParticleSystem>();
        dangerFireFront = f.transform.GetChild(1).GetComponent<ParticleSystem>();

        var col = dangerFire.colorOverLifetime;
        //var colFront = dangerFireFront.colorOverLifetime;

        Gradient grad = PlayerColorManager.instance.players[owner.teamNumber - 1].hatFireColor;
        //Gradient gradFront = GameController.instance.playerInputs[owner.playerIndex].playerColorFront;

        col.color = grad;
        //colFront.color = gradFront;

        trail.colorGradient = PlayerColorManager.instance.players[owner.teamNumber - 1].hatTrailColor;
    }

    public GameObject CreateFireForAnotherObject() {
        GameObject f = Instantiate(firePrefab, transform.position, Quaternion.identity) as GameObject;
        //f.transform.parent = obj.transform;
        //f.transform.position = obj.transform.position + Vector3.up * 0.5f;
        //f.transform.localScale = Vector3.one;

        //f.layer = LayerMask.NameToLayer("Player" + (owner.playerIndex + 1) + "Fire");

        //f.transform.GetChild(0).GetComponent<ParticleSystemRenderer>().material.SetColor("_OutlineColor", PlayerColorManager.instance.players[owner.playerIndex].playerColor);

        var col = dangerFire.colorOverLifetime;
        //var colFront = dangerFireFront.colorOverLifetime;
        
        Gradient grad = PlayerColorManager.instance.players[owner.teamNumber - 1].hatFireColor;
        //Gradient gradFront = GameController.instance.playerInputs[owner.playerIndex].playerColorFront;

        col.color = grad;
        //colFront.color = gradFront;

        return f;
    }

    void PlayCollisionParticles(Vector2 direction) {

        var hatCollisionParticlesEmitParams = new ParticleSystem.EmitParams();
        hatCollisionParticlesEmitParams.velocity = direction * Random.Range(0.1f, 5f);

        VFXManager.instance.EmitAtPosition("Collision_Stars_Additive", hatCollisionParticlesEmitParams, 10, transform.position + Vector3.right * 0.7f * controller.collisions.faceDir + new Vector3(0, 0, -0.2f), owner.teamNumber, 0, true);

        hatCollisionParticlesEmitParams.velocity = direction * Random.Range(0.1f, 2.5f);
        VFXManager.instance.EmitAtPosition("Collision_Stars", hatCollisionParticlesEmitParams, 15, transform.position + Vector3.right * 0.7f * controller.collisions.faceDir + new Vector3(0, 0, -0.2f), owner.teamNumber, 0, true);

    }

    void PlayThrowImpulseParticles(Vector2 direction) {

        float angle = Mathf.Atan2(-direction.normalized.y, direction.normalized.x) * Mathf.Rad2Deg;

        var throwEmitParams = new ParticleSystem.EmitParams();

        //add 180 to compensate for the initial offset
        if (direction != Vector2.zero)
            angle -= 60;
        else
            angle = 0;

        throwEmitParams.rotation3D = new Vector3(0, 0, angle);
        
        VFXManager.instance.EmitAtPosition("Throw_Impulse", 1, target.position, false, target);
    
    }

}