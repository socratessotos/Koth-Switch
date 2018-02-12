using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Tiles {

	GRASS = 0,
	ICE = 1,

}

public class TileMap {

	private int width;
	private int height;

	public int Width {
		get {
			return width;
		}

		set {
			width = value;
		}
	}

	public int Height {
		get {
			return height;
		}

		set {
			height = value;
		}
	}

	int[] tiles;

	public TileMap (int _width, int _height) {
		width = _width;
		height = _height;

		InitializeTileMap ();
	}

	public void InitializeTileMap () {

		tiles = new int [width * height];

		for (int y = 0; y < height; y++) {
			for (int x = 0; x < width; x++) {
				if (IsOnBorder (x, y)) {
					tiles [x + y * width] = 0;
				} else {
					tiles [x + y * width] = -1;
				}
			}
		}

	}

	public bool IsInBounds (int x, int y) {
		return (x >= 0 && y >= 0 && x < width && y < height);
	}

	public bool IsOnBorder (int x, int y) {
		return (x == 0 || y == 0 || x == width-1 || y == height-1);
	}

	public int GetTile (int x, int y) {

		if (IsInBounds (x, y)) {
			return tiles [x + y * width];
		}

		return -100;

	}

	public void SetTile (int x, int y, int value) {

		if (IsInBounds (x, y)) {
			tiles [x + y * width] = value;
		}
	
	}

}
