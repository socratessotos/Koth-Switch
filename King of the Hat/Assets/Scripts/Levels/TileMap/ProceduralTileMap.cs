using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (BoxCollider2D))]
public class ProceduralTileMap : MonoBehaviour {

	[Header("Generation")]
	public Texture2D tilemap;
	public int tileSizeX = 2;
	public int tileSizeY = 2;
		
	[Header("Render Order")]
	public RenderLayer renderLayer = RenderLayer.FROM_SHADER;
	public int renderLayerOffset = 1;

	Mesh mesh;
	MeshFilter filter;
	MeshRenderer rend;
	BoxCollider2D col;

	Vector2[] tileCoords;

	public void Awake () {

		mesh = new Mesh ();
		filter = GetComponent <MeshFilter> ();
		rend = GetComponent <MeshRenderer> ();
		col = GetComponent <BoxCollider2D> ();

		CreateMesh ();
		rend.material.renderQueue = (int)renderLayer + renderLayerOffset;

	}

	Vector3[] vertices;
	void CreateMesh () {

		float _width = (int)(transform.localScale.x) / tileSizeX;
		float _left = -_width/2;

		float _height = (int)(transform.localScale.y) / tileSizeY;
		float _bottom = -_height/2;

		Vector2 numberOfQuads = new Vector2 (
			_width, 
			_height);

		SetTiles ((int)_width, (int)_height);

		vertices = new Vector3[(int)(numberOfQuads.x * numberOfQuads.y * 4)];
		Vector2[] uv = new Vector2 [vertices.Length];

		for (int i= 0, y = 0; y < numberOfQuads.y; y++) {

			for (int x = 0; x < numberOfQuads.x; x++) {

				for (int c = 0; c < 4; c++, i++) {

					vertices [i] = new Vector3 (

						_left + (x + (c % 2)),
						_bottom + (y + (c / 2)),
						transform.position.z

					);

					uv [i] = new Vector2 ((c % 2), (c / 2)) / 3;
					uv [i] += new Vector2 (tileCoords[(int)(x + y * _width)].x, tileCoords[(int)(x + y * _width)].y) / 3;

				}

			}

		}

		int[] triangles = new int [(int)(numberOfQuads.x * numberOfQuads.y * 6)];

		for (int ti = 0, vi = 0, y = 0; y < numberOfQuads.y ; y++) {
			
			for (int  x = 0; x < numberOfQuads.x; x++, ti += 6, vi += 4) {

				triangles [ti] = triangles [ti + 3] = vi;
				triangles [ti + 2] = vi + 1;
				triangles [ti + 1] = triangles [ti + 5] = vi + 3;
				triangles [ti + 4] = vi + 2;

			}
			
		}

		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.triangles = triangles;
		mesh.RecalculateNormals ();

		filter.mesh = mesh;
		rend.material.mainTexture = tilemap;

		transform.localScale = new Vector3 (tileSizeX, tileSizeY);
		col.size = new Vector2 (_width, _height - 0.1f);

	}


	public void SetTiles (int width, int height) {

		tileCoords = new Vector2[width * height];

		for (int y = 0; y < height; y++) {

			for (int x = 0; x < width; x++) {

				int tileX = 1;

				if (x == 0) tileX = 0;
				if (x == width -1) tileX = 2;


				int tileY = 1;

				if (y == 0) tileY = 0;
				if (y == height -1) tileY = 2;

				tileCoords [x + y * width] = new Vector2 (tileX , tileY);

			}

		}

	}


	void OnDrawGizmos () {

		Gizmos.color = Color.red; 
		Gizmos.DrawCube (transform.position + Vector3.forward * - 1, transform.localScale);

	}

}
