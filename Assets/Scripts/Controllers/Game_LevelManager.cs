using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_LevelManager : MonoBehaviour {

	public ItemPrototype[] startingMachines;
	Item[] startingItems;
	void Start(){
		InitStartingShipMachines();
	}
	void InitStartingShipMachines(){
		TileManager.instance.GenerateTileData();
		startingItems = new Item [startingMachines.Length];
		int i = 0;
		foreach(ItemPrototype prototype in startingMachines){
			startingItems[i] = Item_Manager.instance.CreateInstance(prototype);
			i++;
		}
		ShipManager.instance.InitStartMachines(startingItems, new Vector2(-3, 0));
		Station_Manager.instance.Initialize();
	}
}
