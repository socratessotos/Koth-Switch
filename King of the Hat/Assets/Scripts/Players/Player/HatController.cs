using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HatController : RaycastController, PlatformController.IMoveable {

	public CollisionInfo collisions;
	Hat hat;
	
	public override void Start () {
		base.Start ();
		collisions.faceDir = 1;

		//remove the the current layer from collisions
		collisionMask = collisionMask & ~(1 << gameObject.layer);
		hat = GetComponent <Hat> ();

	}

	public void Move(Vector2 moveAmount, bool standingOnPlatform = false) {

		UpdateRaycastOrigins ();

		collisions.Reset ();
		collisions.moveAmountOld = moveAmount;

		if (moveAmount.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		HorizontalCollisions (ref moveAmount);
		if (moveAmount.y != 0) {
			VerticalCollisions (ref moveAmount);
		}

		transform.Translate (moveAmount, Space.World);

		if (standingOnPlatform) {
			collisions.below = true;
		}

		//hat.collisionSkip = false;

	}

	void HorizontalCollisions(ref Vector2 moveAmount) {

        //if (hat.collisionSkip) return;
        Vector2 oldVelocity = hat.velocity;

        float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth) {
			rayLength = 2*skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX,Color.red);

			if (hit) {

				if (hit.distance == 0) {
					//continue;
				}

				if (hit.collider.CompareTag("Ignore")) {
					continue;
				}

				if (hit.collider.CompareTag ("Target")) {

                    if (hat.isCurrentlyAttached || hat.isBeingAttached) continue;
                    
					if (hat.isBeingThrown) { 
					
						hat.ApplyFreezeFrames(5);

						VFXManager.instance.EmitAtPosition ("Target_Explosion", 30, hit.transform.position, true);

                        PlayHitEffect(hit, Vector3.zero, hat.velocity, hat.lastVelocity);

                        Destroy(hit.collider.gameObject);

						collisions.shouldReflect = true;
						hat.collisionSkip = false;
					
					} else {

                        continue;

                    }

                }

                if (hit.transform.CompareTag("Balloon")) {

                    if (hat.isCurrentlyAttached || hat.isBeingAttached) continue;

                    if (hat.isBeingThrown) {

                        hat.ApplyFreezeFrames(5);
                        hat.owner.ApplyFreezeFrames(5);

                        VFXManager.instance.EmitAtPosition("Balloon_Explosion", 30, hit.transform.position, true);
                        VFXManager.instance.EmitAtPosition("Balloon_Lines", 1, hit.transform.position, true);

                        PlayHitEffect(hit, Vector3.zero, hat.velocity, hat.lastVelocity);

                        Destroy(hit.transform.gameObject);

                        collisions.shouldReflect = true;
                        hat.collisionSkip = false;

                    } else {

                        hat.collisionSkip = true;
                        continue;

                    }

                }

                if (hit.transform.CompareTag("Icicle") && hat.collisionSkip) {

                    Icicle icicle = hit.transform.GetComponent<Icicle>();

                    if(hat.isBeingThrown) {

                        if (icicle.isCurrentlyAttached) icicle.BreakOff();

                        icicle.LoseHp();

                        collisions.shouldReflect = true;


                    } else if(!hat.isCurrentlyAttached) {

                        collisions.shouldReflect = true;                       

                    }

                    hat.collisionSkip = false;

                }

                if (hit.transform.CompareTag("TransferDanger") && hat.collisionSkip) {

                    if(hat.isBeingThrown) {

                        hit.transform.GetComponent<TransferDanger>().Transfer(hat.velocity * 1.5f);

                    }

                    collisions.shouldReflect = true;
                    hat.collisionSkip = false;

                }

                if (hit.transform.CompareTag("MiningCart") && hat.collisionSkip) {

                    if (hat.isBeingThrown) {

                        int _direction = collisions.faceDir;

                        hit.transform.GetComponent<MiningCart>().MoveCart(_direction);
                        
                        hat.AllowToComeBack();

                        if(hit.transform.GetComponent<MiningCart>().isDangerous)
                            PlayHitEffect(hit, hat.velocity, hat.velocity, oldVelocity);

                    }

                    collisions.shouldReflect = true;
                    hat.collisionSkip = false;

                }

                if (hat.isBeingAttached && (hit.collider.CompareTag("Player") || hit.collider.CompareTag("Hat") || hit.collider.CompareTag("Lava"))) continue;
				if (hat.isBeingAttached && i == 0) continue;

                if (hit.collider.CompareTag("Lava")) {

                    collisions.shouldBurn = true;
                    
                    continue;
                }

                if(hit.collider.CompareTag("Smash Ball")) {

                    if (hat.isCurrentlyAttached || hat.isBeingAttached) continue;

                    if (hat.isBeingThrown) {

                        hat.ApplyFreezeFrames(5);
                        hat.owner.ApplyFreezeFrames(5);
                      
                        PlayHitEffect(hit, Vector3.zero, hat.velocity, hat.lastVelocity);
                       
                        collisions.shouldReflect = true;
                        hat.collisionSkip = false;

                    } else {

                        hat.collisionSkip = true;
                        continue;

                    }

                }

                if (hit.collider.CompareTag ("Player")) {

                    Player otherPlayer = hit.collider.GetComponent<Player>();

					if (hat.isBeingThrown) {

                        if(!otherPlayer.isInvulnerable && !hat.isHitFrozen) {

                            float stunTimeModifier = (hat.hasIncreasingStunOverTime) ? hat.currentThrowTimer : 0;

                            otherPlayer.BlowBack((new Vector3(hit.point.x, hit.point.y, transform.position.z) - transform.position), hat.throwBlowBack, hat.throwStunTime + stunTimeModifier);

                            hat.ApplyFreezeFrames(5);
                            hat.owner.ApplyFreezeFrames(5);
                            otherPlayer.hat.ApplyFreezeFrames(5);
                            otherPlayer.ApplyFreezeFrames(5);

                            PlayHitEffect(hit, hit.collider.GetComponent<Player>().GetVelocity(), hat.velocity, oldVelocity);
                            collisions.isHittingPlayer = true;
                            hat.collisionSkip = false;
                            hat.AllowToComeBack();

                            CalculateAccuracy();
                        }

					} else {

						continue;

					}

				}

				if (hit.collider.CompareTag ("Hat")) {
					
					Hat other = hit.collider.GetComponent<Hat>();

					if (hat.isFallingOff || other.isFallingOff || hat.isDying) {
						continue;
					}

					if (hat.isBeingThrown) {

						if (other.isCurrentlyAttached) {

                            if (!other.owner.isInvulnerable) {

                                float stunTimeModifier = (hat.hasIncreasingStunOverTime) ? hat.currentThrowTimer : 0;

                                other.owner.BlowBack((new Vector3(hit.point.x, hit.point.y, transform.position.z) - transform.position), hat.throwBlowBack, hat.throwStunTime + stunTimeModifier);

                                hat.ApplyFreezeFrames(5);
                                hat.owner.ApplyFreezeFrames(5);
                                other.ApplyFreezeFrames(5);
                                other.owner.ApplyFreezeFrames(5);

                                hit.collider.GetComponent<HatController>().collisions.hitHat = true;
                                PlayHitEffect(hit, other.velocity, hat.velocity, oldVelocity);
                                collisions.isHittingPlayer = true;
                                hat.collisionSkip = false;
                                hat.AllowToComeBack();

                                CalculateAccuracy();
                            }
                            
						} else if (other.isBeingThrown) {
							hit.collider.GetComponent<HatController>().collisions.hitHat = true;
                            
                            hat.ApplyFreezeFrames(3);
                            hat.owner.ApplyFreezeFrames(3);
                            other.ApplyFreezeFrames(3);
                            other.owner.ApplyFreezeFrames(3);

                            //PlayHatHitHatEffect(hit);
                            //PlayHitEffect(hit, other.velocity, hat.velocity, hat.velocity);
                            PlayHitHatEffect(hit, hat.velocity);

                            CalculateAccuracy();
                        }

						if (!other.isCurrentlyAttached && !other.isFallingOff && !other.isBeingThrown && !hat.isCurrentlyAttached) {
							other.KnockBack(hat.velocity, hat.velocity.magnitude * 0.8f);
						}

					} else {
                        //PlayHatHitHatEffect(hit);
                    }

					if((hat.isCurrentlyAttached) || (other.isCurrentlyAttached)) {
						continue;
					}

					if (!other.isCurrentlyAttached && !other.isFallingOff && !other.isBeingThrown) {
						//other.KnockBack (hat.velocity, hat.velocity.magnitude);
					}

				}

				moveAmount.x = (hit.distance - skinWidth) * directionX;
				rayLength = hit.distance;

				collisions.left = directionX == -1;
				collisions.right = directionX == 1;

                if ((collisions.right || collisions.left) && !hat.isBeingAttached) {
                    collisions.shouldReflect = true;
                }

                if ((collisions.right || collisions.left) && hat.isBeingThrown && !hat.owner.isStunned && hit.transform.gameObject != hat.owner.gameObject) {
                    hat.AllowToComeBack();
                }

            }

		}

	}

	void VerticalCollisions(ref Vector2 moveAmount) {

        //if (hat.collisionSkip) return;

        Vector2 oldVelocity = hat.velocity;

        float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) {

			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D[] hits = Physics2D.RaycastAll (rayOrigin, Vector2.up * directionY, rayLength, collisionMask);
			Debug.DrawRay(rayOrigin, Vector2.up * directionY,Color.red);

			for (int j = 0; j < hits.Length; j++) {

				if (hits[j]) {

					if (hits[j].collider.CompareTag("Ignore")) {
						continue;
					}

					if (hits[j].collider.CompareTag ("Target")) {

                        if (hat.isCurrentlyAttached || hat.isBeingAttached) continue;

                        if (hat.isBeingThrown) { 

							hat.ApplyFreezeFrames(5);
                            hat.owner.ApplyFreezeFrames(5);
                            collisions.shouldReflectY = true;

							VFXManager.instance.EmitAtPosition ("Target_Explosion", 30, hits[j].transform.position, true);

                            PlayHitEffect(hits[j], Vector3.zero, hat.velocity, hat.lastVelocity);

							Destroy (hits[j].collider.gameObject);

						} else {

							continue;

						}

					}

                    if (hits[j].transform.CompareTag("Balloon")) {

                        if (hat.isCurrentlyAttached || hat.isBeingAttached) continue;

                        if (hat.isBeingThrown) {

                            hat.ApplyFreezeFrames(5);
                            hat.owner.ApplyFreezeFrames(5);

                            collisions.shouldReflectY = true;

                            VFXManager.instance.EmitAtPosition("Balloon_Explosion", 30, hits[j].transform.position, true);
                            VFXManager.instance.EmitAtPosition("Balloon_Lines", 1, hits[j].transform.position, true);
                            PlayHitEffect(hits[j], Vector3.zero, hat.velocity, hat.lastVelocity);

                            Destroy(hits[j].transform.gameObject);

                        } else {

                            hat.collisionSkip = true;
                            continue;

                        }

                    }

                    if (hits[j].transform.CompareTag("Icicle") && hat.collisionSkip) {

                        Icicle icicle = hits[j].transform.GetComponent<Icicle>();

                        if (hat.isBeingThrown) {

                            if (icicle.isCurrentlyAttached) icicle.BreakOff();

                            icicle.LoseHp();

                            collisions.shouldReflectY = true;


                        } else if (!hat.isCurrentlyAttached) {

                            collisions.shouldReflectY = true;

                        }

                        hat.collisionSkip = false;

                    }

                    if (hat.isBeingAttached && directionY < 0 && (hits[j].collider.CompareTag("Player") || hits[j].collider.CompareTag("Hat") || hits[j].collider.CompareTag("Lava"))) continue;
					if (hat.isBeingAttached && (hits[j].collider.CompareTag("Player") || hits[j].collider.CompareTag("Hat") || hits[j].collider.CompareTag("Lava"))) continue;

                    if (hits[j].collider.CompareTag ("Lava")) {
                        collisions.shouldBurn = true;
                        continue;
                    }

                    if (hits[j].collider.CompareTag ("Through")) {
//						if (directionY == 1 || hits[j].distance == 0) {
//							continue;
//						}

						if (directionY == 1) {
							continue;
						}
					}

                    if (hits[j].transform.CompareTag("TransferDanger") && hat.collisionSkip) {

                        if (hat.isBeingThrown) {

                            hits[j].transform.GetComponent<TransferDanger>().Transfer(hat.velocity);

                        }

                        collisions.shouldReflectY = true;
                        hat.collisionSkip = false;

                    }

                    if (hits[j].collider.CompareTag("Smash Ball")) {

                        if (hat.isCurrentlyAttached || hat.isBeingAttached) continue;

                        if (hat.isBeingThrown) {

                            hat.ApplyFreezeFrames(5);
                            hat.owner.ApplyFreezeFrames(5);

                            collisions.shouldReflectY = true;

                            //destroy
                            //gain effect

                        } else {

                            hat.collisionSkip = true;
                            continue;

                        }

                    }
                    

                    if (hits[j].collider.CompareTag ("Player")) {

                        Player otherPlayer = hits[j].collider.GetComponent<Player>();

						if (hat.isBeingThrown) {

                            if(!otherPlayer.isInvulnerable && !hat.isHitFrozen) {

                                float stunTimeModifier = (hat.hasIncreasingStunOverTime) ? hat.currentThrowTimer : 0;

                                otherPlayer.BlowBack((new Vector3(hits[j].point.x, hits[j].point.y, transform.position.z) - transform.position), hat.throwBlowBack, hat.throwStunTime + stunTimeModifier);

                                hat.ApplyFreezeFrames(5);
                                hat.owner.ApplyFreezeFrames(5);
                                otherPlayer.hat.ApplyFreezeFrames(5);
                                otherPlayer.ApplyFreezeFrames(5);

                                PlayHitEffect(hits[j], hits[j].collider.GetComponent<Player>().GetVelocity(), hat.velocity, oldVelocity);

								hat.velocity.y = -hat.velocity.y;


                                CalculateAccuracy();
                            }

						} else {

							continue;

						}

					}

					if (hits[j].collider.CompareTag ("Hat")) {
                    
						Hat otherHat = hits[j].collider.GetComponent<Hat>();
                        Player otherPlayer = hits[j].collider.GetComponent<Player>();

                        if (otherHat == null || otherPlayer == null) return;

						if (hat.isFallingOff || otherHat.isFallingOff || hat.isDying) {
							continue;
						}

						if (hat.isBeingThrown) {

                            collisions.shouldReflectY = true;

                            if (otherHat.isCurrentlyAttached) {

                                if(!otherHat.owner.isInvulnerable) {

                                    float stunTimeModifier = (hat.hasIncreasingStunOverTime) ? hat.currentThrowTimer : 0;

                                    otherHat.owner.BlowBack((new Vector3(hits[j].point.x, hits[j].point.y, transform.position.z) - transform.position), hat.throwBlowBack, hat.throwStunTime + stunTimeModifier);
                                    hits[j].collider.GetComponent<HatController>().collisions.hitHat = true;                                 

                                    PlayHitEffect(hits[j], otherPlayer.GetVelocity(), hat.velocity, oldVelocity);

                                    CalculateAccuracy();
                                } else {

                                }

							} else if (otherHat.isBeingThrown) {
								hits[j].collider.GetComponent<HatController>().collisions.hitHat = true;
								//PlayHatHitHatEffect (hits[j]);
                                //PlayHitEffect(hits[j], otherHat.velocity, hat.velocity, hat.velocity);
                                PlayHitHatEffect(hits[j], hat.velocity);
                            }

                            if (!otherHat.isCurrentlyAttached && !otherHat.isFallingOff && !otherHat.isBeingThrown && !hat.isCurrentlyAttached) {
								otherHat.KnockBack(hat.velocity, hat.velocity.magnitude *0.5f);
							}

                            hat.ApplyFreezeFrames(3);
                            otherHat.ApplyFreezeFrames(3);
                            hat.owner.ApplyFreezeFrames(3);
                            otherHat.owner.ApplyFreezeFrames(3);

                        } else {
                            //PlayHatHitHatEffect(hits[j]);
                        }

						if (!otherHat.isCurrentlyAttached && !otherHat.isFallingOff && !otherHat.isBeingThrown && !hat.isCurrentlyAttached) {
                            //other.KnockBack(hat.velocity, hat.velocity.magnitude);
                        }

                        continue;

//						if (!other.isCurrentlyAttached && !other.isBeingThrown && !hat.isCurrentlyAttached && !hat.isBeingThrown) {
//							//other.BlowBack (hat.velocity, hat.velocity.magnitude/10);
//							//hat.velocity.y = -hat.velocity.y / 2;
//							continue;
//						}

					}
                    
					moveAmount.y = (hits[j].distance - skinWidth) * directionY;
					rayLength = hits[j].distance;

					if (!hits[j].collider.CompareTag ("Hat") && !hits[j].collider.CompareTag ("Player")) {
						collisions.below = directionY == -1;
						collisions.above = directionY == 1;
					} else {
						hat.velocity.y = -hat.velocity.y / 2;
					}

                    if ((collisions.above) && hat.isBeingThrown && !hat.owner.isStunned) {
                        hat.AllowToComeBack();
                    }

                }

			}

		}

        if (collisions.shouldReflect) collisions.shouldReflectY = false;

	}

    void CalculateAccuracy() {
		
        if (!hat.hasHitAlready) {
            hat.hasHitAlready = true;
            hat.ThrowHasHit();
        }

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

        public bool shouldBurn;

		public void Reset() {
			
			above = below = false;
			left = right = false;
			shouldReflect = false;
			isHittingPlayer = false;

			shouldReflectY = false;
			isHittingPlayerY = false;

            shouldBurn = false;

		}

	}

    int lastSlashRotation = 0;
    Vector3 hitPlayerSizeMin = new Vector3(3, 13, 1);
    Vector3 hitPlayerSizeMax = new Vector3(6, 16, 1);
	public void PlayHitEffect (RaycastHit2D hit, Vector3 otherVelocity, Vector3 thisVelocity, Vector3 thisOldVelocity) {

        CameraFX.instance.PlayHitPlayerAnimation();
        
        VFXManager.instance.EmitAtPosition("Screen_Crack", 1, hit.transform.position, false);

        //var smackEmitParams = new ParticleSystem.EmitParams();
        //VFXManager.instance.EmitAtPosition("Smack_Additive", smackEmitParams, 1, hit.transform.position - Vector3.forward, hat.owner.playerIndex, 0, false);

        var spikeEmitParams = new ParticleSystem.EmitParams();

        //get the angle of the throw
        float otherAngle = Mathf.Atan2(-otherVelocity.normalized.y, otherVelocity.normalized.x) * Mathf.Rad2Deg;

        spikeEmitParams.rotation3D = new Vector3(0, 0, otherAngle);
        spikeEmitParams.startSize3D = new Vector3(5, 1.3f, 1);
        spikeEmitParams.velocity = otherVelocity.normalized * 3;
        VFXManager.instance.EmitAtPosition("Single_Spike", spikeEmitParams, 1, (hit.transform.position - Vector3.forward) + otherVelocity.normalized, hat.owner.teamNumber, 0, false);

        float thisAngle = Mathf.Atan2(-hat.velocity.normalized.y, -hat.velocity.normalized.x) * Mathf.Rad2Deg;

        spikeEmitParams.rotation3D = new Vector3(0, 0, thisAngle);
        spikeEmitParams.startSize3D = new Vector3(4, 1f, 1);
        spikeEmitParams.velocity = hat.velocity.normalized * 2;
        VFXManager.instance.EmitAtPosition("Single_Spike", spikeEmitParams, 1, (hit.transform.position - Vector3.forward) - (Vector3) hat.velocity.normalized, hat.owner.teamNumber, 0, false);

        float thisOldAngle = Mathf.Atan2(-thisOldVelocity.normalized.y, thisOldVelocity.normalized.x) * Mathf.Rad2Deg;

        spikeEmitParams.rotation3D = new Vector3(0, 0, thisOldAngle);
        spikeEmitParams.startSize3D = new Vector3(2, 0.75f, 1);
        spikeEmitParams.velocity = thisOldVelocity.normalized;
        VFXManager.instance.EmitAtPosition("Single_Spike", spikeEmitParams, 1, (hit.transform.position - Vector3.forward) + thisOldVelocity.normalized, false);



        float hitCircleAngle = Mathf.Atan2(-otherVelocity.normalized.y, otherVelocity.normalized.x) * Mathf.Rad2Deg;

        var hitCircleEmitParams = new ParticleSystem.EmitParams();
        hitCircleEmitParams.rotation3D = new Vector3(0, 0, hitCircleAngle);
        VFXManager.instance.EmitAtPosition("Hit_Circle", hitCircleEmitParams, 1, (hit.transform.position - Vector3.forward), hat.owner.teamNumber, 0, false);


        //NEW SLASH LOGIC
        //get the angle of the throw
        float slashAngle = Mathf.Atan2(-otherVelocity.normalized.y, otherVelocity.normalized.x) * Mathf.Rad2Deg;
        slashAngle -= 90;

        var slashEmitParams = new ParticleSystem.EmitParams();
        slashEmitParams.velocity = otherVelocity.normalized;
        slashEmitParams.rotation3D = new Vector3(0, 0, slashAngle);
        slashEmitParams.startSize3D = new Vector3(Random.Range(3, 6), Random.Range(13, 16), 1);
        VFXManager.instance.EmitAtPosition("Slash", slashEmitParams, 1, hit.transform.position - Vector3.forward, hat.owner.teamNumber, 0, false);

        /*
        var slashEmitParams = new ParticleSystem.EmitParams();
        int newSlashRotation = lastSlashRotation + Random.Range(90, 270);
        lastSlashRotation = newSlashRotation;
        slashEmitParams.rotation = newSlashRotation;
        slashEmitParams.startSize3D = new Vector3(Random.Range(3, 6), Random.Range(13, 16), 1);
        
        VFXManager.instance.EmitAtPosition("Slash", slashEmitParams, 1, hit.transform.position - Vector3.forward, hat.owner.playerIndex, 0, false);
        */
    }

    public void PlayHitHatEffect(RaycastHit2D hit, Vector3 thisVelocity) {

        //NEW SLASH LOGIC
        //get the angle of the throw
        float slashAngle = Mathf.Atan2(-thisVelocity.normalized.y, thisVelocity.normalized.x) * Mathf.Rad2Deg;
        slashAngle -= 90;

        var slashEmitParams = new ParticleSystem.EmitParams();
        slashEmitParams.velocity = thisVelocity.normalized;
        slashEmitParams.rotation3D = new Vector3(0, 0, slashAngle);
        slashEmitParams.startSize3D = new Vector3(Random.Range(3, 6), Random.Range(13, 16), 1);
        VFXManager.instance.EmitAtPosition("Slash", slashEmitParams, 1, hit.transform.position - Vector3.forward, hat.owner.teamNumber, 0, false);

        /*
        var slashEmitParams = new ParticleSystem.EmitParams();
        int newSlashRotation = lastSlashRotation + Random.Range(90, 270);
        lastSlashRotation = newSlashRotation;
        slashEmitParams.rotation = newSlashRotation;
        slashEmitParams.startSize3D = new Vector3(Random.Range(3, 6), Random.Range(13, 16), 1);
        
        VFXManager.instance.EmitAtPosition("Slash", slashEmitParams, 1, hit.transform.position - Vector3.forward, hat.owner.playerIndex, 0, false);
        */
    }

    public void PlayHatHitHatEffect(RaycastHit2D hit) {
        var emitParams = new ParticleSystem.EmitParams();
        // [NEW] VFXManager.instance.EmitAtPosition("Hat_Hit_Hat", emitParams, 20, hit.point, hat.owner.playerIndex, 0, true);
        VFXManager.instance.EmitAtPosition("Collision_Stars_Additive", emitParams, 3, hit.point, hat.owner.teamNumber, 0, true);
        VFXManager.instance.EmitAtPosition("Collision_Stars", emitParams, 6, hit.point, hat.owner.teamNumber, 0, true);

        //VFXManager.instance.EmitAtPosition("Hat_Hit_Hat", emitParams, 1, hit.point, hat.owner.playerIndex, 0, true);

    }

}
