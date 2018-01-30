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
	ObjectPool pool;
	public Vector2 minWalkablePos {get; protected set;}
	public Vector2 maxWalkablePos {get; protected set;}
	void Awake(){
		instance = this;

		dirtyTilesGObjMap = new Dictionary<Tile_Data, GameObject>();
		TileDataMap = new Dictionary<AreaID, Tile_Data[,]>();
	}
	void Start(){
		buildable_Manager = Buildable_Manager.instance;
		pool = ObjectPool.instance;
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
			ClearCurrentTiles();
		}
		SetAreaTileMap(areaID);
		SetGridToTileMap();
		GenerateTileData();

		// Load background, if any
		BGVisuals_Manager.instance.LoadBgForArea(currentArea.id);
	}
	void SetAreaTileMap(AreaID areaID){
		foreach (Area area in tile_areas)
		{
			if (area.id == areaID){
				currentArea = area;
				break;
			}
		}
		currentArea.tilemap.gameObject.SetActive(true);
	}
	void SetGridToTileMap(){
		if (currentArea.tilemap == null)
		return;
		startingX = currentArea.tilemap.origin.x;
		startingY = currentArea.tilemap.origin.y;
		map_width = currentArea.tilemap.size.x;
		map_height = currentArea.tilemap.size.y;
	}

	public void LoadSavedTiles(){
		SavedTiles sTiles = JsonLoader.instance.LoadSavedTiles();
		Item_Manager item_Manager = Item_Manager.instance;
		if (sTiles.savedTiles.Length > 0){
			// TODO: Take care of the current displayed tilemap, save its data, and clear
			//		or pool the tilemap gameobject
			// TODO: Instantiate the tilemap gameobject that needs to be displayed
			//		and set both tilemap and Area id in the dictionary
			foreach (Area area in tile_areas)
			{
				if (area.id == sTiles.areaID){
					currentArea = area;
					break;
				}
			}
			SetGridToTileMap();
			GenerateTileData();
			foreach (STile sTile in sTiles.savedTiles)
			{
				if (sTile.hasMachine == true){
					MachinePrototype proto = buildable_Manager.GetMachinePrototype(sTile.machineName);
					proto.machineCondition = sTile.machineCondition;
					Item machineItem = item_Manager.CreateInstance(item_Manager.GetPrototype(proto.name));
					ShipManager.instance.PlaceMachine(machineItem, proto, new Vector2(sTile.world_x, sTile.world_y));
				}
				else if (sTile.hasProducer == true){
					ProducerPrototype proto = buildable_Manager.GetProducerPrototype(sTile.producerName);
					proto.machineCondition = sTile.machineCondition;
					proto.curProductionName = sTile.itemProduced;
					proto.productionStage = sTile.productionStage;
					Item prodItem = item_Manager.CreateInstance(item_Manager.GetPrototype(proto.name));
					Producer producer = buildable_Manager.CreateProducerInstance(proto);
					GameObject prodGObj = buildable_Manager.SpawnProducer(producer, new Vector2(sTile.world_x, sTile.world_y));
					if (prodGObj == null)
						return;
					prodGObj.GetComponent<Producer_Controller>().Init(prodItem, producer, tileData_Grid[sTile.grid_x, sTile.grid_y]);
				}
			}
		}
	}

	void GenerateTileData(){
		// Verify that we don't alreay have the tile data
		if (TileDataMap.ContainsKey(currentArea.id) == true){
			// Load it if we got it
			tileData_Grid = TileDataMap[currentArea.id];
			return;
		}
		tileData_Grid = new Tile_Data[map_width, map_height];
		minWalkablePos = maxWalkablePos = Vector2.zero;
		Vector2 lastFloorPos = Vector2.zero;
		for(int x = 0; x <map_width; x++){
			for(int y = 0; y <map_height; y++){

				Vector3Int nextTileWorldPos = new Vector3Int(startingX + x, startingY + y, 0);

				if (currentArea.tilemap.HasTile(nextTileWorldPos) == false){
					continue;
				}
				if (currentArea.tilemap.GetSprite(nextTileWorldPos).name == "Floor"){
					// Tile is Floor
					tileData_Grid[x, y] = new Tile_Data(x, y, nextTileWorldPos, TileType.Floor);
					if (minWalkablePos == Vector2.zero){
						minWalkablePos = new Vector2(nextTileWorldPos.x, nextTileWorldPos.y);
					}
					else{
						if (nextTileWorldPos.x >= lastFloorPos.x && nextTileWorldPos.y > lastFloorPos.y){
							maxWalkablePos = new Vector2(nextTileWorldPos.x, nextTileWorldPos.y);
						}
					}

					// If this is a ship, process dirty tiles
					if (currentArea.id == AreaID.Player_Ship)
						tileData_Grid[x, y].RegisterOnDirtCB(OnTileDirty);

					continue;
				}
				tileData_Grid[x, y] = new Tile_Data(x, y, nextTileWorldPos, TileType.Wall);
				
			}
		}
	}
	void ClearCurrentTiles(){
		if (currentArea.tilemap == null)
			return;
		currentArea.tilemap.gameObject.SetActive(false);
		
		currentArea = new Area();
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
				savedTile.grid_x = tile.X;
				savedTile.grid_y = tile.Y;
				savedTile.world_x = tile.worldPos.x;
				savedTile.world_y = tile.worldPos.y;
				savedTile.tileType = tile.tileType;
				if (tile.machine_controller != null){
				    if (tile.machine_controller.baseTile == tile &&
						tile.machine_controller.machine.buildableType != BuildableType.Producer){
						Machine machine = tile.machine_controller.machine;
						savedTile.hasMachine = true;
						savedTile.machineName = machine.name;
						savedTile.machineCondition = machine.machineCondition;
						Debug.Log("Saving " + savedTile.machineName);
					}
				}
				if (tile.producer != null){
					if (tile.machine_controller.baseTile != tile)
						continue;
					savedTile.hasProducer = true;
					savedTile.producerName = tile.producer.name;
					savedTile.productionStage = tile.producer.productionStage;
					savedTile.itemProduced = tile.producer.current_Blueprint.itemProduced.itemName;
					Debug.Log("Saving " + savedTile.producerName);
				}

				saved.Add(savedTile);
			}
		}
		tiles.areaID = currentArea.id;
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