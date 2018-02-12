using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowManager : MonoBehaviour {
    
	public const float MAX_SIZE_DISTANCE = 10f;
	public const float MAX_SIZE_FACTOR = 2f;

	public Vector2 checkOffset;

	public LayerMask solidMask;
	public Sprite shadow;

	Vector3 initialScale;
	Vector3 intialPosition;

	GameObject leftShadow;
	GameObject rightShadow;

	Material leftMat;
	Material rightMat;

	// Use this for initialization
	void Start () {

		initialScale = transform.localScale;
		transform.localScale = Vector3.one;

		leftShadow = new GameObject ("Left Shadow");
		leftShadow.transform.parent = this.transform;
		SpriteRenderer left = leftShadow.AddComponent<SpriteRenderer> ();
		left.sprite = shadow;
		left.sortingLayerName = "Players";
		left.sortingOrder = -1;
		left.material = new Material (Shader.Find("Sprites/Shadow"));
		leftMat = left.material;

		left.color = new Color(0, 0, 0, 0.4f);


        rightShadow = new GameObject ("Right Shadow");
		rightShadow.transform.parent = this.transform;
		SpriteRenderer right = rightShadow.AddComponent<SpriteRenderer> ();
		right.sprite = shadow;
		right.sortingLayerName = "Players";
		right.sortingOrder = -1;
		right.material = new Material (Shader.Find("Sprites/Shadow"));
		rightMat = right.material;

		right.color = new Color(0, 0, 0, 0.4f);
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {

		CastShadow ();

	}

	void CastShadow () {

		RaycastHit2D hitLeft = Physics2D.Raycast (transform.parent.position - new Vector3 (transform.parent.localScale.x/2 * 0.6f, -checkOffset.y, 0f), Vector3.down, 10000, solidMask);
		RaycastHit2D hitRight = Physics2D.Raycast (transform.parent.position + new Vector3 (transform.parent.localScale.x/2 * 0.6f, checkOffset.y, 0f), Vector3.down, 10000, solidMask);

		Debug.DrawRay (transform.parent.position - new Vector3 (transform.parent.localScale.x/2 * 0.6f, 0f, 0f), Vector3.down * 100);
		Debug.DrawRay (transform.parent.position + new Vector3 (transform.parent.localScale.x/2 * 0.6f, 0f, 0f), Vector3.down * 100);

		leftShadow.transform.localScale = initialScale + Mathf.Clamp01 (hitLeft.distance / MAX_SIZE_DISTANCE) * initialScale * (MAX_SIZE_FACTOR - 1);
		leftShadow.transform.position = new Vector2 (transform.parent.position.x, hitLeft.point.y) - new Vector2 (0.0f, leftShadow.transform.localScale.y - 0.05f);

		rightShadow.transform.localScale = initialScale + Mathf.Clamp01 (hitRight.distance / MAX_SIZE_DISTANCE) * initialScale * (MAX_SIZE_FACTOR - 1);
		rightShadow.transform.position = new Vector2 (transform.parent.position.x, hitRight.point.y)- new Vector2 (0.0f, rightShadow.transform.localScale.y - 0.05f);

		if (!Mathf.Approximately(hitLeft.distance, hitRight.distance)) {
			
			RaycastHit2D higherHit = (hitLeft.distance < hitRight.distance) ? hitLeft : hitRight;
			RaycastHit2D lowerHit = (hitLeft.distance > hitRight.distance) ? hitLeft : hitRight;
			Vector2 checkPoint = new Vector2 (lowerHit.point.x, higherHit.point.y - 0.5f);

			RaycastHit2D splitLength = Physics2D.Raycast (checkPoint,  (hitLeft.distance > hitRight.distance) ? Vector3.right : Vector3.left, 10000, solidMask);
			//Debug.DrawRay (checkPoint, Vector3.left * 100, Color.black);

			float totalDistance = (transform.parent.localScale.x/2 * 0.6f) * 2;
			float splitDistance = splitLength.point.x - (transform.parent.position.x - transform.parent.localScale.x/2  * 0.6f);
			float splitValue = (splitDistance /totalDistance);

			leftMat.SetFloat ("_SliceAmountLeft", 0);
			leftMat.SetFloat ("_SliceAmountRight", splitValue);

			rightMat.SetFloat ("_SliceAmountLeft", splitValue);
			rightMat.SetFloat ("_SliceAmountRight", 1);

			rightShadow.SetActive(true);

			Debug.DrawLine (checkPoint, splitLength.point, Color.blue);

		} else {

			leftMat.SetFloat ("_SliceAmountLeft", 0);
			leftMat.SetFloat ("_SliceAmountRight", 1);

			rightMat.SetFloat ("_SliceAmountLeft", 0);
			rightMat.SetFloat ("_SliceAmountRight", 1);

			rightShadow.SetActive(false);
			
		}

		leftShadow.transform.rotation = Quaternion.identity;
		rightShadow.transform.rotation = Quaternion.identity;

	}

}
