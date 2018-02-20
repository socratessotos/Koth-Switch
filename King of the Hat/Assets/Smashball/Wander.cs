using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wander : MonoBehaviour {

	private Vector3 position;
	private Vector3 velocity;
	private Vector3 acceleration;

	private Vector3 steer;

	private Vector3 desired;

	private Rect region;
	public float MAX_SPEED = .05f;

	public float MAX_SEEKFORCE = .1f;

	public Vector3 target;

	[Header("Wanderer Settings")]
	public float timeBetweenChanges = 1f;
	private float lastChange = 0f;

	public float APPROACH_RADIUS = 1;

	private float distance;


	// Use this for initialization
	void Start () {

		region.height = Camera.main.orthographicSize * 2;
		region.width = Camera.main.orthographicSize * Camera.main.aspect * 2; // aspect ratio
		region.min = new Vector2(Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect, Camera.main.transform.position.y - Camera.main.orthographicSize);

		velocity = new Vector3(2, 1, 1);
		acceleration = new Vector3(0,0,0);
		target = new Vector3(Random.Range(region.xMin, region.xMax), Random.Range(region.yMin, region.yMax), 0);
	}
	
	// Update is called once per frame
	void Update () {

		acceleration = wander();

		velocity+=acceleration;

			if (velocity.magnitude > MAX_SPEED){

				scaleToLength(velocity, MAX_SPEED);
			}

		position+=velocity;

		velocity *= Time.deltaTime;

		this.transform.position = position;
		
		moveRegion();

		worldWrap();

		this.position.z = 1f;
	}

	public Vector3 follow(Vector3 target){
		return (Vector3.Normalize(target - this.position) * 10);
	}

	public Vector3 wander(){

		if (Time.time - lastChange > timeBetweenChanges){
			
			lastChange = Time.time;

			target = new Vector3(Random.Range(region.xMin, region.xMax), Random.Range(region.yMin, 
			region.yMax), 0);

		}

		return seekWithApproach(target);

	}

	public Vector3 seekWithApproach(Vector3 target){
		
		this.desired = target - this.position;
		distance = this.desired.magnitude;

		this.desired = Vector3.Normalize(desired);

		if (distance < APPROACH_RADIUS){
			
			this.desired *= (distance / APPROACH_RADIUS);
		}
		else
			this.desired *= MAX_SPEED;

		steer = this.desired - this.velocity;

		if (steer.magnitude > MAX_SEEKFORCE)
			
			scaleToLength(steer, MAX_SEEKFORCE);
		
		steer.z = 1f;

		return steer;

	}

	void worldWrap(){
		if (this.position.x > region.xMax){
			this.position.x = region.xMin;
		}

		if (this.position.x < region.xMin){
			this.position.x = region.xMax;
		}


		if (this.position.y > region.yMax){
			this.position.y = region.yMin;
		}

		if (this.position.y < region.yMin){
			this.position.y = region.yMax;
		}
	}

	public Vector3 scaleToLength(Vector3 vector, float length){
		return (vector.normalized * length);
	}


	public void OnDrawGizmos(){
		Gizmos.color = Color.green;
		Gizmos.DrawRay(new Vector3(position.x, position.y, 1), new Vector3(velocity.x, velocity.y, 1));

		Gizmos.color = Color.red;
		Gizmos.DrawRay(new Vector3(position.x, position.y, 1), new Vector3(desired.x, desired.y, 1));	

		Gizmos.color = Color.yellow;
		Gizmos.DrawWireCube(region.center, (new Vector3(region.width, region.height, 1)));
	}

	private void moveRegion(){
		region.height = Camera.main.orthographicSize * 2;
		region.width = Camera.main.orthographicSize * Camera.main.aspect * 2; // aspect ratio
		region.min = new Vector2(Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect, Camera.main.transform.position.y - Camera.main.orthographicSize);
	}
}
