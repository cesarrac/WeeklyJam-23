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
	
	void Awake(){
		instance = this;
		/* if (ship_TileMap.HasTile(positionToCheck)){
			Debug.Log(ship_TileMap.name + " has tile at " + positionToCheck);
		} */
		startingX = ship_TileMap.origin.x;
		startingY = ship_TileMap.origin.y;
		map_width = ship_TileMap.size.x;
		map_height = ship_TileMap.size.y;
		//Debug.Log(ship_TileMap.name + " starts at " + startingX + ", " + startingY);
		//Debug.Log(ship_TileMap.name + " ends at " + (startingX+ map_width) + ", " + (startingY + map_height));
		//GenerateTileData();
	}
	void Start(){
		
		
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
	
}
