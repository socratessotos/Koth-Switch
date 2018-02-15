using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (MeshFilter))]
[RequireComponent (typeof (MeshRenderer))]
[RequireComponent (typeof (Mesh2DColliderMaker))]
public class DynamicMesh : MonoBehaviour {

	//dynamic mesh variables
	Mesh mesh;
	MeshFilter filter;
	MeshRenderer rend;
	PolygonCollider2D col;
	Mesh2DColliderMaker meshToCol;

	void Awake () {
		
		filter = GetComponent <MeshFilter> ();
		rend = GetComponent <MeshRenderer> ();
		col = GetComponent <PolygonCollider2D> ();
		meshToCol = GetComponent <Mesh2DColliderMaker> ();

	}

	Vector3[] vertices;
	Vector2[] uv;
	public void CreateMesh (Mesh mesh, Texture2D tileMap, Material mat) {

		if (filter == null) filter = GetComponent <MeshFilter> ();
		if (rend == null) rend = GetComponent <MeshRenderer> ();
		if (col == null) col = GetComponent <PolygonCollider2D> ();
		if (meshToCol == null) meshToCol = GetComponent <Mesh2DColliderMaker> ();

		//
		vertices = mesh.vertices;
		uv = mesh.uv;

		filter.mesh = mesh;
		rend.material = mat;
		rend.material.mainTexture = tileMap;
		
	}

	public void CreateCollider (Mesh mesh) {
		
		col.isTrigger = false;
		meshToCol.CreatePolygon2DColliderPoints(mesh);

	}
		
}
