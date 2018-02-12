using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Camera))]
public class SmashCam : MonoBehaviour {
    
    public enum STATE { SMASH_CAM, ENTRANCE_CAM }

	//The amount that the camera will be moved up from the centroid
	//this will allow for more space above the characters
	[Header ("Smash Cam")]
    public float verticalOffset;
	public float borderWidth = 3f;
	public float minSize;
	public float maxSize;
	public float moveTime = 0.1f;
	public float zoomTime = 0.2f;

	public bool isFixed = false;

	public float minX = -25, maxX = 25, minY = -15, maxY = 25;

	//The transforms that the camera will try to keep on screen
	public List <Transform> targets;

    [Header("Theatric Entrance Cam")]
    public float zoom_Size;
    public float zoom_Time;
    public float move_Time;
    public float stare_Time;

    Transform[] playerSpawnPoints;
    Transform currentTarget;
    int currentTargetIndex;

    Vector3 desiredPosition;
	float desiredZValue;

	Vector3 desiredPositionSmooth; 
	float desiredZValueSmooth;

	//A reference to the attached camera
	Camera cam;

    STATE currentState = STATE.SMASH_CAM;

	// Use this for initialization
	void Awake () {
		cam = GetComponent <Camera> ();
	}

	// Update is called once per frame
	void FixedUpdate () {
        
		if (!isFixed) {
            
            switch(currentState) {

                case STATE.SMASH_CAM:
                    
                    FindCentroid();
                    FindFurthestTarget();

                    desiredPosition.z = Mathf.SmoothDamp(desiredZValue, desiredZValue, ref desiredZValueSmooth, zoomTime);
                    transform.position = Vector3.SmoothDamp(transform.position, ClampCentroid(desiredPosition), ref desiredPositionSmooth, moveTime);
                    
                    break;

                case STATE.ENTRANCE_CAM:

                    MoveTheatricEntranceCam();

                    break;

                default:
                    break;

            }

			
		}

	}

	private void FindCentroid () {

		Vector3 centroid = new Vector3 ();
		int numberOfTargets = 0;

		for (int i = 0; i < targets.Count; i++) {

            if (targets[i] == null) targets.RemoveAt(i);    

			if (targets[i].gameObject.activeSelf) {

				centroid += targets[i].position;
				numberOfTargets++;

			}

		}

		if (numberOfTargets > 0) 
			centroid /= numberOfTargets;

		centroid.y += verticalOffset;
		centroid.z = transform.position.z;
		desiredPosition = centroid;

	}

	private Vector3 ClampCentroid (Vector3 centroid) {

		Vector3 topLeftCorner = cam.ViewportToWorldPoint (new Vector3 (0,0, centroid.z));
		Vector3 bottomRightCorner = cam.ViewportToWorldPoint (new Vector3 (1,1, centroid.z));

		float camWidth = Mathf.Abs(topLeftCorner.x -bottomRightCorner.x);
		float camHeight = Mathf.Abs(topLeftCorner.y -bottomRightCorner.y);

		float topBound = centroid.y + camHeight/2;
		float bottomBound = centroid.y - camHeight/2;

		float leftBound = centroid.x - camWidth/2;
		float rightBound = centroid.x + camWidth/2;

		if (bottomBound <= minY) {
			centroid.y = minY + camHeight/2;
		} else if (topBound >= maxY) {
			centroid.y = maxY - camHeight/2;
		}

		if (leftBound <= minX) {
			centroid.x = minX + camWidth/2;
		} else if (rightBound >= maxX) {
			centroid.x = maxX - camWidth/2;
		}

		topBound = centroid.y + camHeight/2;
		bottomBound = centroid.y - camHeight/2;

		leftBound = centroid.x - camWidth/2;
		rightBound = centroid.x + camWidth/2;

		if (bottomBound <= minY && topBound >= maxY) {
			centroid.y = 0;
		}

		if (leftBound <= minX && rightBound >= maxX) {
			centroid.x = 0;
		}

		return centroid;

	}

	private void FindFurthestTarget () {

		Vector3 desiredLocalPosition = cam.transform.InverseTransformPoint(desiredPosition);
		float distance = 0;

		for (int i = 0; i < targets.Count; i++) {

			if (targets[i].gameObject.activeSelf) {

				Vector3 localTargetPosition = cam.transform.InverseTransformPoint (targets[i].position);
				Vector3 distanceToTarget = (desiredLocalPosition - localTargetPosition);

				distance = Mathf.Max (distance, Mathf.Abs (distanceToTarget.y) * cam.aspect);
				distance = Mathf.Max (distance, Mathf.Abs (distanceToTarget.x));

			}

		}

		distance = Mathf.Max (distance, minSize);
		distance = Mathf.Min (distance, maxSize);
		desiredZValue = -distance - borderWidth;



	}

	public void AddTarget (params Transform[] newTargets) {

		for (int i = 0; i < newTargets.Length; i++) {
			targets.Add (newTargets [i]);
		}

	}

	public void RemoveAllTargets () {

		targets.Clear ();

	}

    void OnDrawGizmos() {

        Gizmos.color = Color.magenta;

        //top line
        Gizmos.DrawLine(new Vector3(minX, maxY), new Vector3(maxX, maxY));
        //bottom line
        Gizmos.DrawLine(new Vector3(minX, minY), new Vector3(maxX, minY));
        //right line
        Gizmos.DrawLine(new Vector3(maxX, maxY), new Vector3(maxX, minY));
        //left line
        Gizmos.DrawLine(new Vector3(minX, maxY), new Vector3(minX, minY));

    }

    public void AdjustBorderWidth(float value) {
        borderWidth += value;
    }

    public void AdjustMinSize(float value) {
        minSize += value;
    }

    public void AdjustMoveTime(float value) {
        moveTime += value;
    }

    public void AdjustZoomTime(float value) {
        zoomTime += value;
    }

    void HandleManualZoom() {
        
        //border width
        if (Input.GetKeyDown(KeyCode.O)) {
            borderWidth -= 2f;
        } else if (Input.GetKeyDown(KeyCode.P)) {
            borderWidth += 2f;
        }
        
        //minSize of camera
        if (Input.GetKeyDown(KeyCode.K)) {
            minSize -= 2f;
        } else if (Input.GetKeyDown(KeyCode.L)) {
            minSize += 2f;
        }


        //move time (how long it takes)
        if (Input.GetKeyDown(KeyCode.Q)) {
            moveTime -= 0.1f;
        } else if (Input.GetKeyDown(KeyCode.W)) {
            moveTime += 0.1f;
        }

        //zoom time (how long it takes)
        if (Input.GetKeyDown(KeyCode.A)) {
            zoomTime -= 0.1f;
        } else if (Input.GetKeyDown(KeyCode.S)) {
            zoomTime += 0.1f;
        }
			
    }

    float entranceCamTimer;
    public void InitiateTheatricEntranceCam(Transform[] _spawnPoints) {

        entranceCamTimer = Time.time + 1f;
        currentState = STATE.ENTRANCE_CAM;

        playerSpawnPoints = new Transform[_spawnPoints.Length];

        for (int i = 0; i < _spawnPoints.Length; i++) {
            playerSpawnPoints[i] = _spawnPoints[i];
        }

        currentTargetIndex = 0;
        currentTarget = playerSpawnPoints[currentTargetIndex];
        switchingTargets = true;
    }

    bool switchingTargets;
    void MoveTheatricEntranceCam() {
        
        if(switchingTargets) {

            desiredZValue = -10;
            desiredPosition = currentTarget.position;

            desiredPosition.z = Mathf.SmoothDamp(desiredZValue, desiredZValue, ref desiredZValueSmooth, zoom_Time);
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref desiredPositionSmooth, move_Time);

            if(Time.time > entranceCamTimer) {
                switchingTargets = false;
                entranceCamTimer = Time.time + stare_Time;
            }

        } else {
            
            if(Time.time > entranceCamTimer) {
                switchingTargets = true;
                currentTargetIndex++;

                if (currentTargetIndex < playerSpawnPoints.Length) {
                    currentTarget = playerSpawnPoints[currentTargetIndex];
                    entranceCamTimer = Time.time + 1f;
                } else
                    currentState = STATE.SMASH_CAM;
            }

        }
        
        

    }

    public void CleanUp() {

        for(int i = 0; i < targets.Count - 1; i++) {
        
            if(targets[i] == null) {

                targets.RemoveAt(i);

            }    

        }

    }

}



