using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput_Controller : MonoBehaviour {

	TileManager tileManager;
	void Start(){
		tileManager = TileManager.instance;
	}
	void Update(){
		if (Input.GetMouseButtonDown(0)){
			CheckTileUnderMouse();
		}
	}
	void CheckTileUnderMouse(){
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0;
		TileData tile = tileManager.GetTile(mousePosition);
		if (tile != null){
			Debug.Log("Tile at: " + tile.worldPos + " is type: " + tile.tileType);
		}
		else{
			Debug.Log("tile on " + mousePosition + " is NULL!");
		}
	}
}
