using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Liquid : MonoBehaviour {

	//arrays for holding the surface points
	public float[] xPositions;
	public float[] yPositions;
	public float[] velocities;
	public float[] accelerations;
	public LineRenderer body;

	//mesh objects for rendering the surface
	public GameObject meshObject;
	public Mesh mesh;
	public string tag = "Liquid";

	//colliders for measuring where objects hit
	public GameObject col;

	public float baseHeight;
	public float left;
	public float bottom;

	[Header ("Visuals")]
	//particle system for splashing
	public GameObject splash;
	//material for the surface
	public Material surfaceMat;
	//mesh used for the main body of water
	public Material bodyMat;
	//Texture used for building the tileset
	public Texture2D tileMap;
	public Vector2 numberOfFrames = Vector2.one;
	public float frameLength = 0.5f;

	[Header ("Behaviour")]
	//constants values for determining interactions
	//default for water
	public string layerName;
	public string sortingLayer = "Default";
	public int sortingOrder = 0;
	public float springConstant = 0.02f;
	public float damping = 0.04f;
	public float spread = 0.05f;
	public float z = 0f;
	public bool topOnly = true;
    public bool verticalMesh = false;

	[Range (1, 10)]
	public int quality = 5;
	public int blockSize = 1;
	public int textureSizeX = 3;

	[Header("Render Order")]
	public RenderLayer renderLayer = RenderLayer.FROM_SHADER;
	public int renderLayerOffset = 4;

	void Awake () {

		InstantiateLiquid ();

	}

	// Update is called once per frame
	void FixedUpdate () {

		//UpdateMeshes ();
		UpdateMesh ();
		UpdateSurface ();

	}

	public void InstantiateLiquid () {

		DestroyLiquid ();

		float _width = Mathf.Floor(transform.localScale.x);
		float _left = transform.position.x - _width/2;

		float height = Mathf.Floor(transform.localScale.y);
		float _top = transform.position.y + height/2;
		float _bottom = transform.position.y - height/2;

		int edgeCount = Mathf.RoundToInt (_width) * quality;
		int nodeCount = edgeCount + 1;

		body = gameObject.AddComponent<LineRenderer> ();
		body.sharedMaterial = surfaceMat;
		body.sharedMaterial.renderQueue = 1000;
		body.positionCount = nodeCount;

		body.startWidth = 0f;
		body.endWidth = 0f;

		xPositions = new float[nodeCount];
		yPositions = new float[nodeCount];
		velocities = new float[nodeCount];
		accelerations = new float[nodeCount];

		baseHeight = _top;
		bottom = _bottom;
		left = _left;

		for (int i = 0; i < nodeCount; i++) {

			yPositions[i] = _top;
			xPositions[i] = _left + _width * i /edgeCount;
			accelerations[i] = 0;
			velocities[i] = 0;
			body.SetPosition (i, new Vector3 (xPositions[i], yPositions[i], z));
		}

		CreateMesh ();

		col = new GameObject ();
		col.name = "Trigger";
		col.AddComponent <BoxCollider2D> ();
		col.transform.parent = transform;
		col.transform.localPosition = new Vector3 (0, 0, 0);
		col.transform.localScale = new Vector3 (1, 0.8f, 1);
		col.GetComponent <BoxCollider2D> ().isTrigger = true;
		col.AddComponent <LiquidDetector> ();
		col.tag = tag;
		col.layer = LayerMask.NameToLayer (layerName);

		meshObject.GetComponent<Renderer> ().material.renderQueue = (int)renderLayer + renderLayerOffset;
		meshObject.GetComponent<Renderer> ().sortingLayerName = sortingLayer;
		meshObject.GetComponent<Renderer> ().sortingOrder = sortingOrder;

        if (verticalMesh)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));

	}

	Vector3[] vertices;
	void CreateMesh () {

		mesh = new Mesh ();

		//Determine the vertices
		vertices = new Vector3 [(xPositions.Length - 1) * 4];
		Vector2[] uv = new Vector2 [(xPositions.Length - 1) * 4];

		for (int i = 0, x = 0; x < xPositions.Length - 1; x++) {

			for (int c = 0; c < 4; c++, i++) {

				vertices [i] = new Vector3 (
					xPositions [x + (c % 2)], 
					(c < 2) ? yPositions [x + (c % 2)]: bottom, 
					z);

				int quadsPerBlock = (quality * blockSize);
				
				uv [i] = new Vector2 (
					((float)((x + (c % 2)) % quadsPerBlock) / quadsPerBlock), 
					1-(c / 2));

				if (uv [i].x == 0)
					uv [i].x = (c % 2 == 1) ? 1 : 0;


				//01 01 01 | 01 01 01

				if (textureSizeX > 1) {


					uv [i].x /= 3;

					float xTile = 0f;

					if (x >= quality * blockSize)
						xTile = (x >= xPositions.Length - 1 - quality  * blockSize) ? 2f : 1f;


					uv [i].x += (1f/3f * xTile);
					
				}

			}

		}

		int[] triangles = new int [(xPositions.Length - 1) * 6];

		for (int ti = 0, vi = 0, x = 0; x < xPositions.Length - 1; x++, ti += 6, vi += 4) {

			triangles [ti] = triangles [ti + 3] = vi;
			triangles [ti + 1] = vi + 1;
			triangles [ti + 2] = triangles [ti + 4] = vi + 3;
			triangles [ti + 5] = vi + 2;

		}
			
		//Determine the UV
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateNormals();

		meshObject = new GameObject ();
		meshObject.name = "mesh";
		meshObject.AddComponent <MeshFilter> ();
		meshObject.AddComponent <MeshRenderer> ();
		meshObject.transform.parent = transform;

		meshObject.GetComponent<MeshFilter> ().mesh = mesh;
		meshObject.GetComponent <MeshRenderer> ().material = bodyMat;
		meshObject.GetComponent <MeshRenderer> ().material.mainTexture = tileMap;

		if (numberOfFrames.x > 1 || numberOfFrames.y > 1) {
			
			AnimatedSpriteSheet sheet = meshObject.AddComponent <AnimatedSpriteSheet> ();
			sheet.numberOfFrames = this.numberOfFrames;
			sheet.frameLength = this.frameLength;
			sheet.StartAnimating ();

		}

		//CreateTexture ();

	}

	void UpdateMesh () {

		for (int i = 0, x = 0; x < xPositions.Length - 1; x++) {

			for (int c = 0; c < 4; c++, i++) {

				vertices [i].x = xPositions [x + (c % 2)];
				vertices [i].y = (c < 2) ? yPositions [x + (c % 2)]: bottom;
				vertices [i].z = z;

				if (!topOnly && c > 1) {
					vertices [i].y = yPositions [x + (c % 2)] - transform.localScale.y;
				}

			}

		}

		mesh.vertices = vertices;

	}

	public void UpdateSurface () {

		for (int i = 0; i < xPositions.Length; i++) {

			float force = springConstant * (yPositions[i] - baseHeight) + velocities[i] * damping;
			accelerations[i] = -force;
			yPositions[i] += velocities [i];
			velocities[i] += accelerations [i];
			body.SetPosition (i, new Vector3 (xPositions[i], yPositions[i], z));

		}

		float[] leftDeltas = new float[xPositions.Length];
		float[] rightDeltas = new float[xPositions.Length];

		for (int j = 0; j < 8; j++) {

			for (int i = 0; i < xPositions.Length; i++) {

				if (i > 0) {

					leftDeltas[i] = spread * (yPositions [i] - yPositions[i-1]);
					velocities[i - 1] += leftDeltas [i];

				}

				if (i <xPositions.Length - 1) {

					rightDeltas[i] = spread * (yPositions[i] - yPositions [i +1]);
					velocities[i + 1] += rightDeltas [i];

				}

			}

		}

		for (int i = 0; i < xPositions.Length; i++) {

			if (i > 0) {

				yPositions [i - 1] += leftDeltas [i];

			}

			if (i < xPositions.Length - 1) {

				yPositions [i + 1] += rightDeltas [i];

			}

		}
		
	}

	public void Splash (float xPos, float velocity) {

		if (xPos >= xPositions [0] && xPos <= xPositions[xPositions.Length -1]) {

			xPos -= xPositions [0];
			int index = Mathf.RoundToInt ((xPositions.Length - 1) * (xPos / (xPositions [xPositions.Length - 1] - xPositions [0])));

			velocities[index] = velocity;

		}

	}

	public void DestroyLiquid () {

		for (int i = transform.childCount - 1; i >= 0; i--) {
			DestroyImmediate (transform.GetChild (i).gameObject);
		}

		LineRenderer body = null;
		LineRenderer surface = GetComponent <LineRenderer> ();
	
		if (surface != null)
			DestroyImmediate (surface);

		//arrays for holding the surface points
		xPositions = new float[0];
		yPositions = new float[0];
		velocities = new float[0];
		accelerations = new float[0];

		//mesh objects for rendering the surface
		meshObject = null;
		mesh = null;
		
	}

	void OnDrawGizmos () {

		Gizmos.color = Color.green; 
		Gizmos.DrawCube (transform.position + Vector3.forward * - 1, transform.localScale);

	}

}

