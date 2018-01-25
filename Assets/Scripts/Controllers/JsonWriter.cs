using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class JsonWriter  {

	public static void WriteToJson(SavedTiles tiles){
		
		string content = JsonUtility.ToJson(tiles);
		string filePath = Application.streamingAssetsPath + ("/SavedTiles.json");
		File.WriteAllText(filePath, content);
		Debug.Log("Tiles saved to " + filePath);
	}
}

[System.Serializable]
public struct STile{
	public int x;
	public int y;
	public TileType tileType;
	public MachinePrototype machine;
	public ProducerPrototype producer;
}
public struct SavedTiles{

	public STile[] savedTiles;
}
