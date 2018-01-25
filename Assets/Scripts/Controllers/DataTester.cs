using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTester : MonoBehaviour {

	Tile_Data[,] tileGrid;
	STile[,] savedTileGrid;
	int width = 10, height = 10;
	void Start(){
		
		new JsonWriter();
		new JsonLoader();
		//SaveTileDataTest();
		LoadTileDataTest();
		
	}
	void LoadTileDataTest(){
		List<STile> savedTiles = JsonLoader.instance.LoadSavedTiles();
		if (savedTiles != null && savedTiles.Count > 0){
			tileGrid = new Tile_Data[width, height];
			foreach (STile savedTile in savedTiles)
			{
				AddTile(savedTile.x, savedTile.y, savedTile.tileType, savedTile.machine, savedTile.producer);
			}
			
			Debug.Log("Tile Data converted and loaded");
		}
		else{
			Debug.Log("No tiles found in save file!");
		}
	}
	void AddTile(int x, int y, TileType tType, MachinePrototype machinePrototype, ProducerPrototype producerPrototype){
		tileGrid[x, y] = new Tile_Data(x, y, new Vector3Int(x, y, 0), tType);
		
	}
	void SaveTileDataTest(){
		savedTileGrid = new STile[width, height];
		SavedTiles tiles = new SavedTiles();
		tiles.savedTiles = new STile[width * height];
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				savedTileGrid[x, y] = new STile();
				savedTileGrid[x, y].x = x;
				savedTileGrid[x, y].y = y;
				MachinePrototype machine = new MachinePrototype();
				machine.name = "M_" + (x * width + y);
				savedTileGrid[x, y].machine = machine;
				

				tiles.savedTiles[x * width + y] = savedTileGrid[x, y];
			}
		}

		
		JsonWriter.WriteToJson(tiles);
	}
}
