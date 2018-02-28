using UnityEngine;
using System.Collections;

public class Controller2D : RaycastController, PlatformController.IMoveable {

	public float maxSlopeAngle = 80;

	public CollisionInfo collisions;
	[HideInInspector]
	public Vector2 playerInput;

	LayerMask aliveMask;
	LayerMask deadMask;

	LayerMask selfAliveMask;
	LayerMask selfDeadMask;

	public void SetAliveMask () {
		collisionMask = aliveMask;
		gameObject.layer = selfAliveMask;
	}

	public void SetDeadMask () {
		collisionMask = deadMask;
		gameObject.layer = selfDeadMask;
	}

	public override void Start() {
		base.Start ();
		collisions.faceDir = 1;

		//remove the the current layer from collisions
		collisionMask = collisionMask & ~(1 << gameObject.layer);

		aliveMask = collisionMask;
		deadMask = 1 << LayerMask.NameToLayer("Solid");

		selfAliveMask = gameObject.layer;
		selfDeadMask = LayerMask.NameToLayer("NoCol");

	}

	public void Move(Vector2 moveAmount, bool standingOnPlatform) {
		Move (moveAmount, Vector2.zero, standingOnPlatform);
	}

	public void Move(Vector2 moveAmount, Vector2 input, bool standingOnPlatform = false, bool isDead = false) {

		moveAmount.x = RoundToDecimal (moveAmount.x, 3);
		moveAmount.y = RoundToDecimal (moveAmount.y, 3);

		UpdateRaycastOrigins ();

		collisions.Reset ();
		collisions.moveAmountOld = moveAmount;
		playerInput = input;

		if (moveAmount.y < 0) {
			DescendSlope(ref moveAmount);
		}

		if (moveAmount.x != 0) {
			collisions.faceDir = (int)Mathf.Sign(moveAmount.x);
		}

		HorizontalCollisions (ref moveAmount, isDead);

		if (moveAmount.y != 0) {
			VerticalCollisions (ref moveAmount, isDead);
		}

		transform.Translate (moveAmount);

		if (standingOnPlatform) {
			collisions.below = true;
		}

	}

	void HorizontalCollisions(ref Vector2 moveAmount, bool isDead) {
		float directionX = collisions.faceDir;
		float rayLength = Mathf.Abs (moveAmount.x) + skinWidth;

		if (Mathf.Abs(moveAmount.x) < skinWidth) {
			rayLength = 2*skinWidth;
		}

		for (int i = 0; i < horizontalRayCount; i ++) {
			Vector2 rayOrigin = (directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight;
			rayOrigin += Vector2.up * (horizontalRaySpacing * i);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.right * directionX,Color.red);

			if (hit) {

				if (hit.transform.CompareTag("Balloon")) {
					continue;
				}

                if (hit.transform.CompareTag("Smash Ball")) {
                    continue;
                }

                if (hit.transform.CompareTag("Target")) {
                    continue;
                }

                if (hit.distance == 0) {
					//continue;
				}

				if (hit.collider.CompareTag ("Hat")) {
					continue;
				} 

                if (hit.collider.CompareTag("Lava")) {
                    continue;
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

				if (i == 0 && slopeAngle <= maxSlopeAngle) {
					if (collisions.descendingSlope) {
						collisions.descendingSlope = false;
						moveAmount = collisions.moveAmountOld;
					}
					float distanceToSlopeStart = 0;
					if (slopeAngle != collisions.slopeAngleOld) {
						distanceToSlopeStart = hit.distance-skinWidth;
						moveAmount.x -= distanceToSlopeStart * directionX;
					}
					ClimbSlope(ref moveAmount, slopeAngle, hit.normal);
					moveAmount.x += distanceToSlopeStart * directionX;
				}

				if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle) {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					rayLength = hit.distance;

					if (collisions.climbingSlope) {
						moveAmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmount.x);
					}

					collisions.left = directionX == -1;
					collisions.right = directionX == 1;
				}
			}
		}
	}

	void VerticalCollisions(ref Vector2 moveAmount, bool isDead) {
		float directionY = Mathf.Sign (moveAmount.y);
		float rayLength = Mathf.Abs (moveAmount.y) + skinWidth;

		for (int i = 0; i < verticalRayCount; i ++) {

			Vector2 rayOrigin = (directionY == -1)?raycastOrigins.bottomLeft:raycastOrigins.topLeft;
			rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmount.x);
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

			Debug.DrawRay(rayOrigin, Vector2.up * directionY,Color.red);

			if (hit) {

                if(hit.transform.CompareTag("Ice") && moveAmount.y <= 0) {
                    Player player = GetComponent<Player>();

                    player.isSlidingOnIce = true;

                } else {
                    Player player = GetComponent<Player>();

                    player.isSlidingOnIce = false;
                }

                if (hit.transform.CompareTag("Balloon") && moveAmount.y > 0) {
					continue;
				}

                if (hit.transform.CompareTag("Smash Ball")) {
                    continue;
                }

                if (hit.transform.CompareTag("Target")) {
                    continue;
                }

                if (hit.collider.CompareTag("Lava")) {
                    Player player = GetComponent<Player>();

                    player.BurnAss();

                    collisions.shouldBounceOnLava = true;

                }

                if (hit.collider.tag == "Through") {
//					if (directionY == 1 || hit.distance == 0) {
//						continue;
//					}

					if (directionY == 1) {
						continue;
					}
					if (collisions.fallingThroughPlatform) {
						continue;
					}
					if (playerInput.y == -1) {
						collisions.fallingThroughPlatform = true;
						Invoke("ResetFallingThroughPlatform", .01f);
						continue;
					}
				}

				if (collisions.below && hit.collider.tag == "Bounce") {
					collisions.shouldBounceOnBouncyPlatform = true;


                    DieOnBounce d = hit.transform.GetComponent<DieOnBounce>();
                    if(d != null) {
                        hit.transform.gameObject.SetActive(false);
                    }

				}

				if (hit.collider.CompareTag ("Hat")) {

					if (moveAmount.y > 0) {
						continue;
					}

                    if (hit.collider.GetComponent<Hat>().isBeingAttached) continue;

                    if (!hit.collider.GetComponent <Hat> ().isCurrentlyAttached) {

                        if (!hit.collider.GetComponent<Hat>().isBeingThrown && GetComponent<Player>().isStunned)
                            continue;

						RaycastHit2D groundCheckBottomLeft = Physics2D.Raycast (raycastOrigins.bottomLeft, Vector2.down, hit.distance + skinWidth * 2, deadMask);
						RaycastHit2D groundCheckBottomRight = Physics2D.Raycast (raycastOrigins.bottomRight, Vector2.down, hit.distance + skinWidth * 2, deadMask);

						if ((groundCheckBottomLeft || groundCheckBottomRight)) {
							moveAmount.y = (hit.distance) * directionY;
							collisions.below = true;
							continue;
						}

					} else {
                        hit.collider.GetComponent<Hat>().owner.BlowBack(new Vector3(collisions.faceDir * 10, 30f, 0), 10, 0.025f);
                        hit.collider.GetComponent<Hat>().KnockBack(new Vector3(-collisions.faceDir * 2, 2f, 0), 0.1f);
                    }

                }

				moveAmount.y = (hit.distance - skinWidth) * directionY;
				rayLength = hit.distance;

				if (collisions.climbingSlope) {
					moveAmount.x = moveAmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmount.x);
				}

				collisions.below = directionY == -1;
				collisions.above = directionY == 1;

				if (collisions.below && hit.collider.tag == "Player") {

                    Player other = hit.collider.GetComponent<Player>();

                    if (other.isInvulnerable) return;
                    if (other.hat.isBeingAttached) return;
                    if (GetComponent<Player>().isStunned) return;

                    if ((other.hat.isCurrentlyAttached && !other.hat.isBeingAttached) && GetComponent<Player>().GetVelocity().y < -1f && !other.isInvulnerable && GetComponent<Player>().canHurtHat) {

                        //other.hat.LoseHatLife();
                        //GetComponent<Player>().canHurtHat = false;
                        //collisions.shouldBounce = true;
                        //GameController.instance.game.currentPlayers[other.GetComponent<Player>().playerIndex].UpdateStockUI();

                        //if (other.hat.currentHp <= 0) {
                            other.hat.KillOwner(GetComponent<Player>());
                        //} else {
                        //    other.hat.StartHitFlash();
                        //}

                    } else {
						other.GetFootStooled(0.05f, GetComponent<Player>().isFastFalling);
					}

					collisions.shouldBounce = true;
					collisions.below = false;

				} else if (collisions.above && hit.collider.tag == "Player" && !hit.collider.GetComponent<Player>().isStunned && hit.collider.GetComponent<Player>().GetVelocity().y < -1f) {

					hit.collider.GetComponent <Controller2D> ().collisions.shouldBounce = true;
					GetComponent<Player>().GetFootStooled (0.05f, GetComponent<Player>().isFastFalling);

				}

                if (collisions.below && hit.collider.tag == "Hat") {

                    if (GetComponent<Player>().isStunned)
                        continue;
                    
					Hat other = hit.collider.GetComponent <Hat> ();

					if (other.isDying || other.isBeingAttached) return;

                    if (hit.collider.GetComponent<Hat>().owner.isInvulnerable) {
                        collisions.shouldBounce = true;
                        continue;
                    }

                    if (!other.isBeingThrown && !other.isBeingAttached && GetComponent<Player>().GetVelocity().y < -1f) {
						//collisions.shouldBounce = true;
						collisions.below = false;

                        if (!other.owner.isInvulnerable && GetComponent<Player>().GetVelocity().y < -1 && GetComponent<Player>().canHurtHat) {

                            //hit.collider.GetComponent<Hat>().LoseHatLife();    
                            //GetComponent<Player>().canHurtHat = false;
                            //collisions.shouldBounce = true;
                            //GameController.instance.game.currentPlayers[hit.collider.GetComponent<Hat>().owner.playerIndex].UpdateStockUI();
                            
                            //if (hit.collider.GetComponent<Hat>().currentHp <= 0)
                                hit.collider.GetComponent<Hat>().KillOwner(GetComponent<Player>());
                            //else {
                            //    hit.collider.GetComponent<Hat>().StartHitFlash();
                            //}

                        }

                    }

				}
					
				if (hit.transform.gameObject.name == "Balloon") {

					collisions.shouldBounce = true;
					Destroy (hit.transform.gameObject);
                    VFXManager.instance.EmitAtPosition("Balloon_Explosion", 30, hit.transform.position, true);
                    VFXManager.instance.EmitAtPosition("Balloon_Lines", 1, hit.transform.position, true);

                }

            }

		}

		if (collisions.climbingSlope) {
			float directionX = Mathf.Sign(moveAmount.x);
			rayLength = Mathf.Abs(moveAmount.x) + skinWidth;
			Vector2 rayOrigin = ((directionX == -1)?raycastOrigins.bottomLeft:raycastOrigins.bottomRight) + Vector2.up * moveAmount.y;
			RaycastHit2D hit = Physics2D.Raycast(rayOrigin,Vector2.right * directionX,rayLength,collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle(hit.normal,Vector2.up);
				if (slopeAngle != collisions.slopeAngle) {
					moveAmount.x = (hit.distance - skinWidth) * directionX;
					collisions.slopeAngle = slopeAngle;
					collisions.slopeNormal = hit.normal;
				}
			}
		}
	}

	void ClimbSlope(ref Vector2 moveAmount, float slopeAngle, Vector2 slopeNormal) {
		float moveDistance = Mathf.Abs (moveAmount.x);
		float climbmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;

		if (moveAmount.y <= climbmoveAmountY) {
			moveAmount.y = climbmoveAmountY;
			moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
			collisions.below = true;
			collisions.climbingSlope = true;
			collisions.slopeAngle = slopeAngle;
			collisions.slopeNormal = slopeNormal;
		}
	}

	void DescendSlope(ref Vector2 moveAmount) {

		RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast (raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, collisionMask);
		RaycastHit2D maxSlopeHitRight = Physics2D.Raycast (raycastOrigins.bottomRight, Vector2.down, Mathf.Abs (moveAmount.y) + skinWidth, collisionMask);
		if (maxSlopeHitLeft ^ maxSlopeHitRight) {
			SlideDownMaxSlope (maxSlopeHitLeft, ref moveAmount);
			SlideDownMaxSlope (maxSlopeHitRight, ref moveAmount);
		}

		if (!collisions.slidingDownMaxSlope) {
			float directionX = Mathf.Sign (moveAmount.x);
			Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
			RaycastHit2D hit = Physics2D.Raycast (rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

			if (hit) {
				float slopeAngle = Vector2.Angle (hit.normal, Vector2.up);
				if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle) {
					if (Mathf.Sign (hit.normal.x) == directionX) {
						if (hit.distance - skinWidth <= Mathf.Tan (slopeAngle * Mathf.Deg2Rad) * Mathf.Abs (moveAmount.x)) {
							float moveDistance = Mathf.Abs (moveAmount.x);
							float descendmoveAmountY = Mathf.Sin (slopeAngle * Mathf.Deg2Rad) * moveDistance;
							moveAmount.x = Mathf.Cos (slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign (moveAmount.x);
							moveAmount.y -= descendmoveAmountY;

							collisions.slopeAngle = slopeAngle;
							collisions.descendingSlope = true;
							collisions.below = true;
							collisions.slopeNormal = hit.normal;
						}
					}
				}
			}
		}
	}

	void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount) {

		if (hit) {
			float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
			if (slopeAngle > maxSlopeAngle) {
				moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs (moveAmount.y) - hit.distance) / Mathf.Tan (slopeAngle * Mathf.Deg2Rad);

				collisions.slopeAngle = slopeAngle;
				collisions.slidingDownMaxSlope = true;
				collisions.slopeNormal = hit.normal;
			}
		}

	}

	void ResetFallingThroughPlatform() {
		collisions.fallingThroughPlatform = false;
	}

	float RoundToDecimal (float value, int numberOfPlaces) {

		value = Mathf.Floor (value * Mathf.Pow (10, numberOfPlaces)) / Mathf.Pow (10, numberOfPlaces);
		return value;

	}

	public struct CollisionInfo {
		public bool above, below;
		public bool left, right;

		public bool climbingSlope;
		public bool descendingSlope;
		public bool slidingDownMaxSlope;

		public float slopeAngle, slopeAngleOld;
		public Vector2 slopeNormal;
		public Vector2 moveAmountOld;
		public int faceDir;
		public bool fallingThroughPlatform;
		public bool shouldBounce;
		public bool shouldBounceOnBouncyPlatform;
        public bool shouldBounceOutOfWell;
        public bool shouldBounceOnLava;

        public void Reset() {
			above = below = false;
			left = right = false;
			climbingSlope = false;
			descendingSlope = false;
			slidingDownMaxSlope = false;
			slopeNormal = Vector2.zero;

			slopeAngleOld = slopeAngle;
			slopeAngle = 0;
		}

		public void Kill () {

			Reset ();
			shouldBounce = false;
            shouldBounceOnBouncyPlatform = false;
            shouldBounceOutOfWell = false;
            shouldBounceOnBouncyPlatform = false;
            fallingThroughPlatform = false;
			
		}
	}

}
