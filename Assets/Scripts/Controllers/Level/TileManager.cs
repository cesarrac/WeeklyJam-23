using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileManager : MonoBehaviour {
	public static TileManager instance {get; protected set;}
	Vector3Int positionToCheck = Vector3Int.zero;
	int startingX;
	int startingY;
	int map_width;
	int map_height;
	Tile_Data[,] tileData_Grid;
	Dictionary<AreaID, Tile_Data[,]> TileDataMap;
	Dictionary<Tile_Data, GameObject> dirtyTilesGObjMap;
	
	public Area[] tile_areas;
	public Area currentArea {get; protected set;}
	Buildable_Manager buildable_Manager;
	void Awake(){
		instance = this;

		dirtyTilesGObjMap = new Dictionary<Tile_Data, GameObject>();
		TileDataMap = new Dictionary<AreaID, Tile_Data[,]>();
	}
	void Start(){
		buildable_Manager = Buildable_Manager.instance;
	}
	public void LoadArea(AreaID areaID){
		if (tileData_Grid != null && currentArea.tilemap != null){
			// Save the current data in map, keyed to area id
			if (TileDataMap.ContainsKey(currentArea.id) == true){
				// replace old data
				TileDataMap[currentArea.id] = tileData_Grid;
			}else{
				// new data
				TileDataMap.Add(currentArea.id, tileData_Grid);
			}
		}
		foreach (Area area in tile_areas)
		{
			if (area.id == areaID){
				currentArea = area;
				break;
			}
		}
		if (currentArea.tilemap == null)
			return;
		startingX = currentArea.tilemap.origin.x;
		startingY = currentArea.tilemap.origin.y;
		map_width = currentArea.tilemap.size.x;
		map_height = currentArea.tilemap.size.y;
		GenerateTileData();

		// Load background, if any
		BGVisuals_Manager.instance.LoadBgForArea(currentArea.id);

		
	}

	void GenerateTileData(){
		// Verify that we don't alreay have the tile data
		if (TileDataMap.ContainsKey(currentArea.id) == true){
			// Load it if we got it
			tileData_Grid = TileDataMap[currentArea.id];
			return;
		}
		tileData_Grid = new Tile_Data[map_width, map_height];
		for(int x = 0; x <map_width; x++){
			for(int y = 0; y <map_height; y++){

				Vector3Int nextTilePosition = new Vector3Int(startingX + x, startingY + y, 0);

				if (currentArea.tilemap.HasTile(nextTilePosition) == false){
					continue;
				}
				if (currentArea.tilemap.GetSprite(nextTilePosition).name == "Floor"){
					tileData_Grid[x, y] = new Tile_Data(x, y, nextTilePosition, TileType.Floor);

					// If this is a ship, process dirty tiles
					if (currentArea.id == AreaID.Player_Ship)
						tileData_Grid[x, y].RegisterOnDirtCB(OnTileDirty);

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
		if (currentArea.tilemap == null)
			return;
		if (currentArea.id != AreaID.Player_Ship)
			return;

		if (dirtyTilesGObjMap.ContainsKey(tile)){
			if (tile.Dirtiness <= 0){
				// Pool the dirty tile
				GameObject gobj = dirtyTilesGObjMap[tile];
				gobj.transform.SetParent(null);
				ObjectPool.instance.PoolObject(gobj);
				dirtyTilesGObjMap.Remove(tile);
				return;
			}
			// Increase or Decrease dirt by changing sprite
			Sprite newDirt = Sprite_Manager.instance.GetDirt(Mathf.RoundToInt(tile.Dirtiness * 10));
			if (newDirt == null)
				return;
			dirtyTilesGObjMap[tile].GetComponentInChildren<SpriteRenderer>().sprite = newDirt;
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
		dirt.transform.SetParent(currentArea.tilemap.transform);
		dirtyTilesGObjMap.Add(tile, dirt);

	}
	public float GetMaxX(){
		return startingX + map_width;
	}
	public float GetMaxY(){
		return startingY + map_height;
	}

	public void SaveTileData(){
		if (tileData_Grid == null)
			return;
		SavedTiles tiles = new SavedTiles();
		List<STile> saved = new List<STile>();
		for (int x = 0; x < map_width; x++)
		{
			for (int y = 0; y < map_height; y++)
			{
				Tile_Data tile = tileData_Grid[x, y];
				if (tile == null)
					continue;

				STile savedTile = new STile();
				savedTile.x = tile.X;
				savedTile.y = tile.Y;
				savedTile.tileType = tile.tileType;
				if (tile.machine_controller != null){
				    if (tile.machine_controller.baseTile == tile){
						Machine machine = tile.machine_controller.machine;
						savedTile.machine = buildable_Manager.GetMachinePrototype(machine.name);
						savedTile.machine.machineCondition = machine.machineCondition;
						Debug.Log("Saving " + machine.name);
					}
				}
				if (tile.producer != null){
					if (tile.machine_controller.baseTile != tile)
						continue;
					savedTile.producer = buildable_Manager.GetProducerPrototype(tile.producer.name);
					savedTile.producer.curProductionName = tile.producer.current_Blueprint.itemProduced.itemName;
					savedTile.producer.productionStage = tile.producer.productionStage;
					Debug.Log("Saving " + tile.producer.name);
				}

				saved.Add(savedTile);
			}
		}
		tiles.savedTiles = saved.ToArray();
		JsonWriter.WriteToJson(tiles);
	}
}

[System.Serializable]
public enum AreaID {NULL, Player_Ship, Centrum_Plaza}
[System.Serializable]
public struct Area{

	public AreaID id;
	public Tilemap tilemap;

}