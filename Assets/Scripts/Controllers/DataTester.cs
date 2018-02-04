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
		
	}
	
	void AddTile(STile savedTile){

		tileGrid[savedTile.grid_x, savedTile.grid_y] = new Tile_Data(savedTile.grid_x, savedTile.grid_y, new Vector3Int(savedTile.grid_x, savedTile.grid_y, 0), savedTile.tileType);
		
		if (savedTile.hasMachine == true){
			MachinePrototype machineProto = Buildable_Manager.instance.GetMachinePrototype(savedTile.machineName);
			machineProto.machineCondition = savedTile.machineCondition;
			// use this proto to spawn the machine
		}
		else if (savedTile.hasProducer == true){
			ProducerPrototype producerProto = Buildable_Manager.instance.GetProducerPrototype(savedTile.producerName);
			producerProto.productionStage = savedTile.productionStage;
			producerProto.curProductionName = savedTile.itemProduced;

		}
		
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
				savedTileGrid[x, y].grid_x = x;
				savedTileGrid[x, y].grid_y = y;
				
				

				tiles.savedTiles[x * width + y] = savedTileGrid[x, y];
			}
		}

		
		JsonWriter.WriteTilesToJson(tiles, AreaID.Player_Ship);
	}
}
