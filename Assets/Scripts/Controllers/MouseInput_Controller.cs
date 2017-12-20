using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseInput_Controller : MonoBehaviour {
	public static MouseInput_Controller instance {get; protected set;}
	TileManager tileManager;
	public delegate void OnInput();
	public event OnInput onUse;
	public event OnInput onInteract;
	void Awake(){
		instance = this;
	}
	void Start(){
		tileManager = TileManager.instance;
	}
	void Update(){
		if (Input.GetMouseButtonDown(0)){
			CheckTileUnderMouse();
		}
		if (Input.GetMouseButtonDown(1)){
			if (onInteract != null)
				onInteract();
		}
	}
	void CheckTileUnderMouse(){
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0;
		Tile_Data tile = tileManager.GetTile(mousePosition);
		if (tile != null){
			Debug.Log("Tile at: " + tile.worldPos + " is type: " + tile.tileType);
			if (tile.machine != null){
				Debug.Log("The tile has a machine: " + tile.machine.machineName);
				tile.machine.TryRepair();
			}
		}
		else{
			Debug.Log("tile on " + mousePosition + " is NULL!");
		}
	}
	

}
