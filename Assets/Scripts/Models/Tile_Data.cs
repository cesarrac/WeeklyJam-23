using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public enum TileType {Floor, Wall}
public class Tile_Data  {

	public TileType tileType {get; protected set;}
	public int X {get; protected set;}
	public int Y {get; protected set;}
	public Vector3Int worldPos {get; protected set;}
	public Machine_Controller machine {get; protected set;}
	public Tile_Data(int gridX, int gridY, Vector3Int worldPosition, TileType tType){
		X = gridX;
		Y = gridY;
		tileType = tType;
		worldPos = worldPosition;
	}

	public bool AddMachine(Machine_Controller newMachine){
		if (machine != null)
			return false;
		machine = newMachine;
		return true;
	}
	public bool RemoveMachine(){
		if (machine == null)
			return false;
		machine = null;
		return true;
	}

 	public Tile_Data[] GetNeighbors(bool getDiags = true){
		Tile_Data[] neighbors = new Tile_Data[8];
		if (getDiags == true){
			neighbors[0] = TileManager.instance.GetTile(X, Y + 1); // N
			neighbors[1] = TileManager.instance.GetTile(X + 1, Y + 1); // NE
			neighbors[2] = TileManager.instance.GetTile(X + 1, Y); // E
			neighbors[3] = TileManager.instance.GetTile(X + 1, Y - 1); // SE
			neighbors[4] = TileManager.instance.GetTile(X, Y - 1); // S
			neighbors[5] = TileManager.instance.GetTile(X - 1, Y - 1); // SW
			neighbors[6] = TileManager.instance.GetTile(X - 1, Y); // W
			neighbors[7] = TileManager.instance.GetTile(X - 1, Y + 1);	// NW
		}
		else{
			neighbors = new Tile_Data[4];
			neighbors[0] = TileManager.instance.GetTile(X, Y + 1); // N
			neighbors[1] = TileManager.instance.GetTile(X + 1, Y); // E
			neighbors[2] = TileManager.instance.GetTile(X, Y - 1); // S
			neighbors[3] = TileManager.instance.GetTile(X - 1, Y); // W
		}
		  return neighbors.Where(tile => tile != null).ToArray();
	} 
}
