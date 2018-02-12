using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BirdController2D))]
public class Bird : MonoBehaviour {

	public enum Action {
		STAY,
		MOVE,
		PECK,
		SCARED,
		DEAD,
		SPAWN,
		FLY_AWAY
	}
	public static readonly string[] BIRD_ANIMATIONS = {"01_Stay", "02_Hop", "03_Peck", "04_Fly", "05_Dead"};

	[HideInInspector]
	public BirdManager manager;
	[HideInInspector]
	public SpriteRenderer render;
	[HideInInspector]
	public Animator aniController;


	[Header("Movement")]
	//the strength of the impulse of the hop
	public Vector2 hopImpulse;
	public float maxSpeed;
	[Space (10)]

	[Header("Physics")]
	//how quickly it slows down on the air
	public float drag;
	//how quickly it slows down in the ground
	public float friction;
	//speed at which the bird falls
	public float gravity;
	[Space (10)]

	public GameObject deathParticles;

	private Action currentActionHidden;
	private Action currentAction {
		get { return currentActionHidden; }

		set {
			currentActionHidden = value;
			SetAnimation (value);
		}
	}

	private float waitTime;

	private int direction = 1;
	private bool isMovingRight {get {return direction > 0;}}

	// Fleeing
	private BirdManager fleeTarget;
	private float fleeTime;

	private BirdController2D controller;

	private Vector2 velocity, acceleration;
		
	void Awake () {
		
		controller = GetComponent<BirdController2D> ();
		render = GetComponent<SpriteRenderer> ();
		aniController = GetComponent<Animator> ();

		currentAction = Action.SPAWN;

	}

	void FixedUpdate () {

		PerformCurrentAction ();

		switch (currentAction) {
		case Action.DEAD:
		case Action.FLY_AWAY:
			return;
		case Action.SPAWN:
			RandomizeWaitTime ();
			currentAction = Action.STAY;
			break;
		}

		if (waitTime <= 0) {
			
			ChooseNewAction ();

		} else {
			
			waitTime -= Time.deltaTime;

		}

		Move ();

		if (currentAction == Action.SCARED) return;

		// Player detection
		foreach (GameObject p in GameObject.FindGameObjectsWithTag("Player")) {
			Vector3 pos = p.transform.position;

			float distance = Mathf.Sqrt (sqr (pos.x - transform.position.x) + sqr (pos.y - transform.position.y));
			if (distance <= manager.birdFrightDistance) {
				Vector2 newDirection = pos - transform.position;
				RaycastHit2D ray = Physics2D.Raycast (transform.position, newDirection.normalized, manager.birdFrightDistance, manager.birdFrightMask);

				Debug.DrawRay (transform.position, newDirection.normalized * manager.birdFrightDistance, Color.yellow);

				if (!ray || ray.collider.tag != "Player")
					continue;

				direction = (int) Mathf.Sign (pos.x - transform.position.x);

				currentAction = Action.SCARED;
				//render.color = Color.green;

				FlyToNewZone ();
				return;
			}
		}

	}
		
	void ChooseNewAction () {

		RandomizeWaitTime ();
		//render.color = Color.white;

		if (currentAction == Action.STAY)
			currentAction = ((Random.Range (0, 2) == 1) ? Action.MOVE : Action.PECK);
		else if (currentAction == Action.PECK)
			currentAction = ((Random.Range (0, 2) == 1) ? Action.MOVE : Action.STAY);
		else if (currentAction == Action.MOVE) {
			if (Random.value < 0.5)
				currentAction = Action.MOVE;
			else
				currentAction = ((Random.Range (0, 2) == 1) ? Action.STAY : Action.PECK);
		}

		PrepareForAction ();

	}

	void PrepareForAction () {

		switch (currentAction) {

		case Action.MOVE:

			RandomizeDirection ();
			waitTime += 1.0f;


			float newPos = transform.position.x + (BirdManager.MOVE_RATIO * hopImpulse.x * direction);

			if (manager.isBirdThere (this, newPos))
				direction = -direction;

			if (manager.bounds.max.x - newPos <= 0 || newPos - manager.bounds.min.x <= 0)
				direction = -direction;

			//render.color = Color.blue;
			velocity = new Vector2 (hopImpulse.x * direction, hopImpulse.y);

			break;

		case Action.STAY:

			//render.color = Color.white;
			velocity = Vector2.zero;

			break;

		case Action.PECK:

			//Set animaition and such
			//TODO: Add peck animation time to wait time
			waitTime += 0.5f;
			//render.color = Color.red;

			velocity = Vector2.zero;

			break;

		case Action.SCARED:
			currentAction = Action.FLY_AWAY;
			break;
		}

	}

	void PerformCurrentAction () {

		switch (currentAction) {

		default:
			break;

		case Action.FLY_AWAY:

			acceleration = new Vector2 (direction * 1, 1);

			if (velocity.magnitude >= maxSpeed)
				velocity = velocity.normalized * maxSpeed;

			break;

		case Action.SCARED:
			
			Vector2 directionVector = (fleeTarget.getRandomPos (transform.localScale/2) - transform.position);

			if (directionVector.magnitude <= manager.birdFleeRadius) {
				manager.removeBird (this);
				manager = fleeTarget;
				manager.addBird (this);

				currentAction = Action.STAY;
				RandomizeWaitTime ();

				velocity = Vector2.zero;
				fleeTime = Time.fixedTime;
				return;
			}


			acceleration = (directionVector.normalized * maxSpeed) - velocity;
			acceleration = acceleration.normalized * 0.3f;

			Debug.DrawRay (transform.position, velocity.normalized * 10f, Color.green);
			Debug.DrawRay (transform.position, acceleration.normalized * 10f, Color.red);
			Debug.DrawRay (transform.position, directionVector.normalized * 10f, Color.yellow);

			if (velocity.magnitude >= maxSpeed)
				velocity = velocity.normalized * maxSpeed;

			break;

		}

	}

	void Move () {

		velocity += acceleration;

		if (Mathf.Abs(velocity.x) <= friction && controller.collisions.below)
			velocity.x = 0;
		
		controller.Move (velocity * Time.fixedDeltaTime, currentAction == Action.SCARED);
		//controller.Move (velocity);

		if (controller.collisions.left || controller.collisions.right)
			velocity.x = 0;

		if (controller.collisions.above || controller.collisions.below)
			velocity.y = 0;

		acceleration = new Vector2 (
			-direction * ((controller.collisions.below) ? friction : drag), 
			-gravity);


		if (velocity.x != 0) {
			render.flipX = velocity.x < 0;
		} else {
			render.flipX = !isMovingRight;
		}


	}

	public void FlyToNewZone () {

		BirdManager zone = manager.getNewZone ();

		if (zone == null) {
			currentAction = Action.FLY_AWAY;
			return;
		}

		fleeTarget = zone;
		velocity = Vector2.up * maxSpeed;
		waitTime = manager.birdMaxFleeTime;

	}
	private void RandomizeWaitTime () {
		waitTime = manager.birdWaitTimeMin + Random.value * manager.birdWaitTimeAdd;
	}

	private void RandomizeDirection () {
		if (Random.value <= manager.birdDirectionChangeChance)
			direction = -direction;
	}

	private float sqr(float f) {
		return f * f;
	}

	private float min (float a, float b) {
		if (a < b)
			return a;
		return b;
	}

	void SetAnimation (Action action) {
		if ((int) action >= BIRD_ANIMATIONS.Length && action != Action.FLY_AWAY)
			aniController.Play (BIRD_ANIMATIONS [0]);
		else if (!aniController.GetCurrentAnimatorStateInfo (0).IsName (BIRD_ANIMATIONS [(int) action]))
			aniController.Play (BIRD_ANIMATIONS [(int) action]);
	}


	//keeping trigger functions on top or on bottom is easier to manage imo
	void OnTriggerEnter2D (Collider2D other) {
		
		if (other.tag == "Hat") {

			Hat h = other.GetComponent <Hat> ();

			if (h.isBeingThrown) {

				currentAction = Action.DEAD;

				manager.removeBird (this);
				Instantiate (deathParticles, transform.position, Quaternion.identity);
				Destroy (gameObject);

			}
			
		}

	}

}
