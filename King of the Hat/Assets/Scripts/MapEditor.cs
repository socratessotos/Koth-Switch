using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class MapEditor : MonoBehaviour {

	[Header("Texture Params")]
	public Texture2D[] tileMaps;
	public Material material;
	//rename this
	public int pixelsPerTile = 16;

	//rename this
	int tileMapTilesX, tileMapTilesY;

	[Header("World Params")]
	public Vector2 mapSize = new Vector2 (10, 10);		//the number of tiles in the map
	public Vector2 tileSize = new Vector2 (1, 1);		//the number of unity units for the tiles width and height

	//these offsets are used to center the map to (0,0)
	float mapOffsetX {get {return (map.Width * tileSize.x)/2 - tileSize.x/2;}}
	float mapOffsetY {get {return (map.Height * tileSize.y)/2  - tileSize.y/2;}}

	//the actual map of tiles
	//is represented by a grid of ints
	TileMap map;
	Vector2[] tileCoords;

	//a reference to the camera
	Camera cam;

	//variables for the mouse controls of the program
	float mouseMovementBorderSize = 100f;
	float maxCamBorder = 2f;

	float minScrollSpeed = 1f;
	float maxScrollSpeed = 10f;

	//min and max for zooming in and out
	float minOrthographicSize = 5f;
	float maxOrthographicSize {get {return ((mapOffsetX > mapOffsetY) ? mapOffsetX : mapOffsetY) + maxCamBorder;}}

	//the current type of tile that the user is drawing with
	int currentBrushValue = 0;

	//offset used for determining max scroll distance
	float camOffset {get {return cam.orthographicSize - maxCamBorder;}}

	//dynamic mesh variables
	Mesh mesh;
	Mesh collisionMesh;

	//the array of dynamic meshes associated with this level
	DynamicMesh[] dynamicMeshes;

	// Use this for initialization
	void Awake () {

		//initialize the map
		map = new TileMap ((int)mapSize.x, (int)mapSize.y);
		tileCoords = new Vector2[map.Width * map.Height];

		tileMapTilesX = tileMaps[0].width / pixelsPerTile;
		tileMapTilesY = tileMaps[0].height / pixelsPerTile;

		cam = Camera.main;

		//get references to the dynamic mesh components
		mesh = new Mesh ();
		dynamicMeshes = new DynamicMesh[tileMaps.Length];

		GenerateMeshFor ((int) Tiles.GRASS);

		cam.orthographicSize = maxOrthographicSize;

	}
	
	// Update is called once per frame
	void Update () {

		HandleMovement ();
		HandleScrolling ();
		HandleInput ();

		if (Input.GetKeyDown (KeyCode.S)) {
			SaveLevel ();
		}


		if (Input.GetKeyDown (KeyCode.Q)) {
			currentBrushValue = 0;
		}		

		if (Input.GetKeyDown (KeyCode.W)) {
			currentBrushValue = 1;
		}

	}

	void HandleMovement () {

		Vector3 mousePosition = Input.mousePosition;
		Vector2 camMovement = new Vector2 (0, 0);

		//check for cam movement left and right
		if (mousePosition.x > 0 && mousePosition.x < mouseMovementBorderSize && cam.transform.position.x - camOffset > -mapOffsetX) {

			camMovement.x = -Mathf.Lerp (maxScrollSpeed, minScrollSpeed, (mousePosition.x / mouseMovementBorderSize));

		} else if (mousePosition.x < Screen.width && mousePosition.x > Screen.width - mouseMovementBorderSize && cam.transform.position.x + camOffset < mapOffsetX) {
			
			camMovement.x = Mathf.Lerp (maxScrollSpeed, minScrollSpeed, ((Screen.width - mousePosition.x) / mouseMovementBorderSize));

		}

		//check for cam movement up and down
		if (mousePosition.y > 0 && mousePosition.y < mouseMovementBorderSize && cam.transform.position.y - camOffset > -mapOffsetY) {
			
			camMovement.y = -Mathf.Lerp (maxScrollSpeed, minScrollSpeed, (mousePosition.y / mouseMovementBorderSize));

		} else if (mousePosition.y < Screen.height && mousePosition.y > Screen.height - mouseMovementBorderSize && cam.transform.position.y + camOffset < mapOffsetY) {

			camMovement.y = Mathf.Lerp (maxScrollSpeed, minScrollSpeed, ((Screen.height - mousePosition.y) / mouseMovementBorderSize));;

		}

		camMovement *= Time.deltaTime;
		cam.transform.Translate (camMovement.x, camMovement.y, 0);

	}

	void HandleScrolling () {

		float scroll = Input.GetAxis("Mouse ScrollWheel");

		if (scroll < 0 && cam.orthographicSize < maxOrthographicSize) {
			cam.orthographicSize -= scroll;
		}

		if (scroll > 0 && cam.orthographicSize > minOrthographicSize) {
			cam.orthographicSize -= scroll;
		}
		
	}

	Vector2 lastCoordinate;
	int lastBrushValue;
	void HandleInput () {

		if (Input.GetMouseButtonDown (0)) {

			//get the mouse position in world coordinates
			Vector2 tilePosition = WorldToTilePosition (cam.ScreenToWorldPoint(Input.mousePosition));

			//give the current tile a new value
			SetTile (tilePosition, currentBrushValue, true);

		} else if (Input.GetMouseButton(0)) {

			//get the mouse position in world coordinates
			Vector2 tilePosition = WorldToTilePosition (cam.ScreenToWorldPoint(Input.mousePosition));

			//check if we are on the same tile as last time
			if (lastCoordinate == new Vector2((int)tilePosition.x, (int)tilePosition.y)) return;

			//give the current tile a new value
			SetTile (tilePosition, lastBrushValue);

		}
		
	}

	//rename this
	void SetTile (Vector2 tilePosition, int brushValue, bool onDown = false) {

		//the last tile coordinate clicked on by the player is updated
		lastCoordinate = new Vector2((int)tilePosition.x, (int)tilePosition.y);

		//check if the current tile is not equal to the current brush value
		if (map.GetTile ((int)tilePosition.x, (int)tilePosition.y) != brushValue) {

			//set it equal to the current brush value
			map.SetTile ((int)tilePosition.x, (int)tilePosition.y, brushValue);
			if (onDown) lastBrushValue = currentBrushValue;

		}

		else {

			if (onDown) {
				
				//if it is equal to the current brush value then make it an empty tile
				map.SetTile ((int)tilePosition.x, (int)tilePosition.y, -1);
				lastBrushValue = -1;

			}

		}

		GenerateMeshFor ((int) Tiles.GRASS);

	}

	Vector2 WorldToTilePosition (Vector3 position) {

		float tilePositionX = (position.x + mapOffsetX + tileSize.x/2) / tileSize.x;
		float tilePositionY = (position.y + mapOffsetY + tileSize.y/2) / tileSize.y;

		return new Vector2 ((int) tilePositionX, (int) tilePositionY);

	}

	int GetTotalMapTiles (int tileMapIndex) {

		int numTiles = 0;

		//iterate through the y values
		for (int i = 0, y = 0; y < map.Height; y++) {

			//iterate through the x values
			for (int x = 0; x < map.Width; x++) {

				//check if the tile is "lit"
				if (map.GetTile (x, y) == tileMapIndex) {

					numTiles += 1;

				}

			}

		}

		return numTiles;
		
	}

	Vector3[] vertices;
	Vector2[] uv;
	void GenerateMeshFor (int tileMapIndex) {

		mesh = new Mesh ();
		DetermineTileCoords (tileMapIndex);

		vertices = new Vector3[(int)(map.Width * map.Height * 4)];
		Vector2[] uv = new Vector2 [vertices.Length];

		List <Mesh> cornerMeshes = new List <Mesh> ();

		//iterate through the y values
		for (int i = 0, y = 0; y < map.Height; y++) {

			//iterate through the x values
			for (int x = 0; x < map.Width; x++) {

				//check if the tile is "lit"
				if (map.GetTile (x, y) == tileMapIndex) {

					float worldTileCoordX = x * tileSize.x - mapOffsetX; 
					float worldTileCoordY = y * tileSize.y - mapOffsetY;

					//add the four corners of the tilemap to the vertices array
					for (int c = 0; c < 4; c++, i++) {

						vertices [i] = new Vector3 (
	
							-tileSize.x/2 + (worldTileCoordX + (c % 2) * tileSize.x),
							-tileSize.y/2 + (worldTileCoordY + (c / 2) * tileSize.y),
							transform.position.z
	
						);

						uv [i] = new Vector2 ((tileCoords [x + y * map.Width].x + (c % 2)) / tileMapTilesX, ((tileMapTilesY-1) - tileCoords [x + y * map.Width].y + (c / 2)) / tileMapTilesY);
							
					}

					if (map.GetTile (x + 1, y) == tileMapIndex && map.GetTile (x, y - 1) == tileMapIndex && map.GetTile (x + 1, y - 1) != tileMapIndex)
						cornerMeshes.Add (CreateCornerMesh (x, y, 5, 2));

					if (map.GetTile (x + 1, y) == tileMapIndex && map.GetTile (x, y + 1) == tileMapIndex && map.GetTile (x + 1, y + 1) != tileMapIndex)
						cornerMeshes.Add (CreateCornerMesh (x, y, 5, 3));

					if (map.GetTile (x - 1, y) == tileMapIndex && map.GetTile (x, y - 1) == tileMapIndex && map.GetTile (x - 1, y - 1) != tileMapIndex)
						cornerMeshes.Add (CreateCornerMesh (x, y, 5, 0));

					if (map.GetTile (x - 1, y) == tileMapIndex && map.GetTile (x, y + 1) == tileMapIndex && map.GetTile (x - 1, y + 1) != tileMapIndex)
						cornerMeshes.Add (CreateCornerMesh (x, y, 5, 1));
						
				}

			}

		}

		int[] triangles = new int [(int)(map.Width * map.Height * 6)];

		for (int ti = 0, vi = 0, y = 0; y < map.Height ; y++) {

			for (int  x = 0; x < map.Width; x++, ti += 6, vi += 4) {

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

		DynamicMesh dynamicMesh = null;

		if (dynamicMeshes [tileMapIndex] == null) {
			
			GameObject go = new GameObject (tileMaps [tileMapIndex].name);
			go.transform.parent = this.transform;
			dynamicMesh = go.AddComponent<DynamicMesh> ();
			dynamicMeshes [tileMapIndex] = dynamicMesh;

		} else {
			dynamicMesh = dynamicMeshes [tileMapIndex];
		}
			

		dynamicMesh.CreateCollider (CreateCollisionMesh ());

		mesh = GenereateCombinedMesh (cornerMeshes);
		dynamicMesh.CreateMesh (mesh, tileMaps [tileMapIndex], material);

	}

	Mesh CreateCollisionMesh () {

		Vector3[] v = new Vector3[(int)(map.Width * map.Height * 4)];

		//iterate through the y values
		for (int i = 0, y = 0; y < map.Height; y++) {

			//iterate through the x values
			for (int x = 0; x < map.Width; x++) {

				//check if the tile is "lit"
				if (map.GetTile (x, y) >= 0) {

					//add the four corners of the tilemap to the vertices array
					for (int c = 0; c < 4; c++, i++) {

						float worldTileCoordX = x * tileSize.x - mapOffsetX; 
						float worldTileCoordY = y * tileSize.y - mapOffsetY;

						v [i] = new Vector3 (

							-tileSize.x/2 + (worldTileCoordX + (c % 2) * tileSize.x),
							-tileSize.y/2 + (worldTileCoordY + (c / 2) * tileSize.y) + ((map.GetTile (x, y + 1) < 0) ? (-0.3f * ((c /2))) : 0),
							transform.position.z

						);

					}

				}

			}

		}

		Mesh collisionMesh = new Mesh ();

		collisionMesh.vertices = v;
		collisionMesh.triangles = mesh.triangles;
		collisionMesh.RecalculateNormals ();

		return collisionMesh;

	}

	Mesh CreateCornerMesh (int x, int y, int tileCoordX, int tileCoordY) {

		Mesh cornerMesh = new Mesh ();
		Vector3[] v = new Vector3[4];
		Vector2[] uvs = new Vector2 [v.Length];

		float worldTileCoordX = x * tileSize.x - mapOffsetX; 
		float worldTileCoordY = y * tileSize.y - mapOffsetY;

		//add the four corners of the tilemap to the vertices array
		for (int c = 0, i = 0; c < 4; c++, i++) {

			v [i] = new Vector3 (

				-tileSize.x/2 + (worldTileCoordX + (c % 2) * tileSize.x),
				-tileSize.y/2 + (worldTileCoordY + (c / 2) * tileSize.y),
				transform.position.z - 0.0001f

			);

			uvs [i] = new Vector2 (((float)tileCoordX + (c % 2)) / tileMapTilesX, ((tileMapTilesY-1) - (float)tileCoordY + (c / 2)) / tileMapTilesY);

			//uvs[i].x = ((int)(uvs[i].x * 100) + 1.0f) / 100.0f;
			//uvs[i].y = (int)(uvs[i].y * 1000) / 1000.0f;

		}

		int[] t = new int [6];

		t [0] = t [0 + 3] = 0;
		t [0 + 2] = 0 + 1;
		t [0 + 1] = t [0 + 5] = 0 + 3;
		t [0 + 4] = 0 + 2;

		cornerMesh.vertices = v;
		cornerMesh.uv = uvs;
		cornerMesh.triangles = t;
		cornerMesh.RecalculateNormals ();

		//GameObject go = new GameObject ("cornerMesh");
		//go.AddComponent <MeshFilter> ().mesh = cornerMesh;
		//go.AddComponent <MeshRenderer> ().material = material;

		return cornerMesh;

	}

	Mesh GenereateCombinedMesh (List <Mesh> meshes) {

		Mesh combinedMesh = new Mesh ();

		GameObject go = new GameObject ("cornersMesh");
		MeshFilter filter = go.AddComponent <MeshFilter> ();

		CombineInstance[] combine = new CombineInstance[meshes.Count + 1];
		combine[combine.Length - 1].mesh = mesh;
		combine[combine.Length - 1].transform = filter.transform.localToWorldMatrix;

		int i = 0;
		while (i < meshes.Count) {
			
			filter.mesh = meshes[i];
			combine[i].mesh = meshes[i];
			combine[i].transform = filter.transform.localToWorldMatrix;
			i++;

		}

		combinedMesh.CombineMeshes (combine);

		Destroy (go);

		return combinedMesh;

	}

	void DetermineTileCoords (int tileMapIndex) {

		//iterate through the y values
		for (int y = 0; y < map.Height; y++) {

			//iterate through the x values
			for (int x = 0; x < map.Width; x++) {

				tileCoords [x + y * map.Width] = CheckSurroundingConnections (x, y, tileMapIndex);

			}

		}

	}

	//rename this function
	Vector2 CheckSurroundingConnections (int x, int y, int tileMapIndex) {

		Vector2 coord = Vector2.zero;
	
		bool up = map.GetTile (x, y + 1) == tileMapIndex;
		bool down = map.GetTile (x, y - 1) == tileMapIndex;
		bool left = map.GetTile (x - 1, y) == tileMapIndex;
		bool right = map.GetTile (x + 1, y) == tileMapIndex;

		if (left && right)
			coord.x = 1;
		else if (left)
			coord.x = 2;
		else if (right)
			coord.x = 0;
		else
			coord.x = 3;

		if (up && down)
			coord.y = 1;
		else if (up)
			coord.y = 2;
		else if (down)
			coord.y = 0;	
		else 
			coord.y = 3;

		return coord;

	}

	void SaveLevel () {

		//check if the outer file exists
		//if not create it
		if ( !Directory.Exists("Assets/CustomLevels") ) {
			AssetDatabase.CreateFolder("Assets", "CustomLevels");
		}

		//the number of levels the player has created up till now
		int levelNumber = 1;

		//while the directory exists, increment the level number
		//to ensure levels are not overwritten
		while (Directory.Exists("Assets/CustomLevels/Level_" + levelNumber)) levelNumber++;

		//create a folder for the new level
		AssetDatabase.CreateFolder("Assets/CustomLevels", "Level_" + levelNumber);

		//create a folder for the new Mesh
		AssetDatabase.CreateFolder("Assets/CustomLevels/Level_" + levelNumber, "Mesh");	


		//save the mesh
		AssetDatabase.CreateAsset(mesh, "Assets/CustomLevels/Level_" + levelNumber + "/Mesh/Mesh.asset");
		AssetDatabase.SaveAssets();

		//create and save the prefab
		var emptyPrefab = PrefabUtility.CreateEmptyPrefab("Assets/CustomLevels/Level_" + levelNumber + "/Level_" + levelNumber + "_Prefab.prefab");
		PrefabUtility.ReplacePrefab(transform.GetChild (0).gameObject, emptyPrefab);

	}

	void OnDrawGizmos () {

		return;

		if (map == null) return;

		Gizmos.color = Color.black;

		for (int y = 0; y < map.Height; y++) {
			for (int x = 0; x < map.Width; x++) {

				if (map.GetTile (x, y) == 0) {

					Gizmos.DrawCube (new Vector3 (x * tileSize.x - mapOffsetX, y * tileSize.y - mapOffsetY, 0), new Vector3 (tileSize.x, tileSize.y, 1));
					
				}
					
			}

		}

	}

}
