using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (Collider2D))]
public class MeshJigglifier : MonoBehaviour {

	public Vector3[] defaultVertexPositions;

	public float springConstant = 0.1f;
	public float damping = 0.1f;

	public float jiggleRadius = 2f;

	private Vector3[] vertexPositions;
	private Vector2[] vertexVelocities;
	private Vector2[] vertexAccelerations;

	private MeshFilter filter;

	private Stopwatch sw;

	// Use this for initialization
	void Start () {

		filter = GetComponent <MeshFilter> ();
		defaultVertexPositions = filter.mesh.vertices;

		vertexPositions = new Vector3[filter.mesh.vertices.Length];
		vertexVelocities = new Vector2[filter.mesh.vertices.Length];
		vertexAccelerations = new Vector2[filter.mesh.vertices.Length];

		for (int i = 0; i < defaultVertexPositions.Length; i++) {

			vertexPositions [i].x = defaultVertexPositions [i].x;
			vertexPositions [i].y = defaultVertexPositions [i].y;
			vertexPositions [i].z = defaultVertexPositions [i].z;


			//vertexVelocities [i] = new Vector2 (0, -1);

		}

		sw = new Stopwatch ();
			
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		for (int i = 0, l = defaultVertexPositions.Length; i < l; i++) {

			if (vertexVelocities [i].x == 0 && vertexVelocities [i].y == 0)
				continue;

			float forceX = springConstant * (vertexPositions [i].x - defaultVertexPositions[i].x) + vertexVelocities[i].x * damping;
			float forceY = springConstant * (vertexPositions [i].y - defaultVertexPositions[i].y) + vertexVelocities[i].y * damping;

			vertexAccelerations [i].x = -forceX;
			vertexAccelerations [i].y = -forceY;

			vertexVelocities[i].x += vertexAccelerations [i].x;
			vertexVelocities[i].y += vertexAccelerations [i].y;

			vertexPositions[i].x += vertexVelocities [i].x;
			vertexPositions[i].y += vertexVelocities [i].y;

		}


		filter.mesh.vertices = vertexPositions;
	
	}

	public void Splash (Vector3 pos, Vector3 force, float sqrRadius) {

		for (int i = 0, l = defaultVertexPositions.Length; i < l; i++) {

			float sqrDist = (pos - defaultVertexPositions[i]).sqrMagnitude;

			if (sqrDist <= sqrRadius) {

				vertexVelocities [i].x = force.x * (1 - (sqrDist / sqrRadius));
				vertexVelocities [i].y = force.y * (1 - (sqrDist / sqrRadius));

			}
				
		}

	}

	public Vector3 GetRandomVertex () {
		return defaultVertexPositions [Random.Range (0, defaultVertexPositions.Length)];
	}


	void OnTriggerEnter2D (Collider2D other) {

		if (other.CompareTag("Player")) {

			Controller2D c = other.GetComponent<Controller2D> ();
			Splash (other.transform.position, c.collisions.moveAmountOld / 4, 5);

		}

		if (other.CompareTag("Hat")) {

			Hat h = other.GetComponent<Hat> ();
			if (!h.isBeingThrown) return;
			Splash (other.transform.position, h.velocity / 2, 10);

		}



	}

	void OnTriggerExit2D (Collider2D other) {

		if (other.CompareTag("Player")) {

			Controller2D c = other.GetComponent<Controller2D> ();
			Splash (other.transform.position, c.collisions.moveAmountOld / 2, 5);

		}

		if (other.CompareTag("Hat")) {
			
			Hat h = other.GetComponent<Hat> ();
			if (!h.isBeingThrown) return;
			//Splash (other.transform.position, h.velocity, 10);

		}

	}

}
