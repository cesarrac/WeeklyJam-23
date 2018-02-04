using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class JsonWriter  {

	public static void WriteTilesToJson(SavedTiles tiles, AreaID areaID){
		
		string content = JsonUtility.ToJson(tiles);
		string filePath = Application.streamingAssetsPath + ("/Saved/Tiles/SavedTiles_" + areaID.ToString() + ".json");
		File.WriteAllText(filePath, content);
		Debug.Log("Tiles saved to " + filePath);
	}
	public static void WriteInventoryToJson(SavedInventory inventory, string uniqueID){
		string content = JsonUtility.ToJson(inventory);
		string filePath = Application.streamingAssetsPath + ("/Saved/Inventory/Inv_" + uniqueID + ".json");
		File.WriteAllText(filePath, content);
		Debug.Log("Inventory saved to " + filePath);
	}
}

[System.Serializable]
public struct STile{
	public int grid_x;
	public int grid_y;
	public int world_x;
	public int world_y;
	public TileType tileType;
	public bool hasMachine;
	public string machineName;
	public MachineCondition machineCondition;
	public bool hasProducer;
	public string producerName;
	public int productionStage;
	public string itemProduced;
}
[System.Serializable]
public struct SavedTiles{
	public AreaID areaID;
	public STile[] savedTiles;
}
[System.Serializable]
public struct SavedInventory{
	public int maxSpace;
	public ItemReference[] items;
	
}
