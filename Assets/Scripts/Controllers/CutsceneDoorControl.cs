using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CutsceneDoorControl : MonoBehaviour {

	public Tilemap doorTiles;
	public float frameRate = 4;
	void OnDisable(){
		if (doorTiles == null)
			return;
		doorTiles.animationFrameRate = 0;
	}
	void OnEnable(){
		if (doorTiles == null)
			return;
		doorTiles.animationFrameRate = frameRate;
	}
}
