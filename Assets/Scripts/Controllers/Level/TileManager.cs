using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour {
	public static TileManager instance {get; protected set;}
	public Tilemap ship_TileMap;
	Vector3Int positionToCheck = Vector3Int.zero;
	int startingX;
	int startingY;
	int map_width;
	int map_height;
	Tile_Data[,] tileData_Grid;
	Dictionary<Tile_Data, GameObject> dirtGObjMap;
	
	void Awake(){
		instance = this;
		/* if (ship_TileMap.HasTile(positionToCheck)){
			Debug.Log(ship_TileMap.name + " has tile at " + positionToCheck);
		} */
		startingX = ship_TileMap.origin.x;
		startingY = ship_TileMap.origin.y;
		map_width = ship_TileMap.size.x;
		map_height = ship_TileMap.size.y;

		dirtGObjMap = new Dictionary<Tile_Data, GameObject>();
		//Debug.Log(ship_TileMap.name + " starts at " + startingX + ", " + startingY);
		//Debug.Log(ship_TileMap.name + " ends at " + (startingX+ map_width) + ", " + (startingY + map_height));
		//GenerateTileData();
	}

	public void GenerateTileData(){
		tileData_Grid = new Tile_Data[map_width, map_height];
		for(int x = 0; x <map_width; x++){
			for(int y = 0; y <map_height; y++){
				Vector3Int nextTilePosition = new Vector3Int(startingX + x, startingY + y, 0);
				if (ship_TileMap.HasTile(nextTilePosition) == false){
					continue;
				}
				if (ship_TileMap.GetSprite(nextTilePosition).name == "Floor"){
					tileData_Grid[x, y] = new Tile_Data(x, y, nextTilePosition, TileType.Floor);
					tileData_Grid[x, y].RegisterOnDirtCB(OnTileDirty);
					//Debug.Log("Tile at " + nextTilePosition + " set to FLOOR!");
					continue;
				}
				tileData_Grid[x, y] = new Tile_Data(x, y, nextTilePosition, TileType.Wall);
				
			}
		}
	}
	public Tile_Data GetTile(Vector3 worldPosition){
		int worldX = Mathf.FloorToInt(worldPosition.x);
		int worldY = Mathf.FloorToInt(worldPosition.y);
		int gridX = Mathf.Abs(worldX - startingX);
		int gridY = Mathf.Abs(worldY - startingY);
		//Debug.Log("World position: " + worldPosition + " is grid position: " + gridX + ", " + gridY);
		if (IsInGridBounds(gridX, gridY) == true){
			return tileData_Grid[gridX, gridY];
		}
		return null;
	}
	public Tile_Data GetTile(int x, int y){
		if (IsInGridBounds(x, y) == true){
			return tileData_Grid[x, y];
		}
		return null;
	}
	public bool IsInGridBounds(int x, int y){
		if (x < 0 || y < 0)
			return false;
		if (x >= map_width || y >= map_height)
			return false;
		return true;
	}
	
	public void OnTileDirty(Tile_Data tile){
		Debug.Log("OnTileDirty");
		if (dirtGObjMap.ContainsKey(tile)){
			if (tile.Dirtiness <= 0){
				// Pool the dirty tile
				GameObject gobj = dirtGObjMap[tile];
				gobj.transform.SetParent(null);
				ObjectPool.instance.PoolObject(gobj);
				dirtGObjMap.Remove(tile);
				return;
			}
			// Increase or Decrease dirt by changing sprite
			Sprite newDirt = Sprite_Manager.instance.GetDirt(Mathf.RoundToInt(tile.Dirtiness * 10));
			if (newDirt == null)
				return;
			dirtGObjMap[tile].GetComponentInChildren<SpriteRenderer>().sprite = newDirt;
			return;
		}

		if (tile.Dirtiness <= 0){
			Debug.Log("TileManager-- not changing dirty tile because its dirtiness is less than 0");
			return;
		}
			
		// Create new one:
		GameObject dirt = ObjectPool.instance.GetObjectForType("Dirty Tile", true, tile.worldPos);
		Sprite dirtSprite = Sprite_Manager.instance.GetDirt(Mathf.RoundToInt(tile.Dirtiness * 10));
		if (dirtSprite == null)
			 dirtSprite = Sprite_Manager.instance.GetDirt(1);
		dirt.GetComponentInChildren<SpriteRenderer>().sprite = dirtSprite;
		dirt.transform.SetParent(ship_TileMap.transform);
		dirtGObjMap.Add(tile, dirt);

	}
}
