using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission  {

	// owner: who issued this mission
	public Character owner {get; protected set;}
	public string description {get; protected set;}
	// what items and how many of them to deliver
	public ItemReference[] itemsToDeliver {get; protected set;}
	// Where to deliver
	public Station stationDestination {get; protected set;}
	// Where the mission was generated/received
	public Station stationPickUp {get; protected set;}
	public int jumpsRequired {get; protected set;}
	public bool itemsAcquired {get; protected set;} // Has the player received the items required?
	public Mission (Character _owner, string desc, ItemReference[] deliveryItems, Station _pickUp, Station _destination, int jumpRequirement){
		owner = _owner;
		description = desc;
		itemsToDeliver = deliveryItems;
		stationPickUp = _pickUp;
		stationDestination = _destination;
		jumpsRequired = jumpRequirement;
		Debug.Log("Mission created to pick up " +  itemsToDeliver[0].count + " " + 
				itemsToDeliver[0].itemName + " at " + stationPickUp.stationName + 
				"  and deliver to " + stationDestination.stationName);
		itemsAcquired = false;
	}

	public string[] GetMisItemNames(){
		// Returns an array with the necessary items to spawn for the player to pick up
		List<string> itemNames = new List<string>();
		for(int i = 0; i < itemsToDeliver.Length; i++){
			if (itemsToDeliver[i].count <= 0)
				continue;
			itemNames.Add(itemsToDeliver[i].itemName);
		}
		// Mark mission as picked up so we don't give the player the items more than once
		itemsAcquired = true;
		return itemNames.ToArray();
	}

	// Has this mission been completed?
	public bool isCompleted() {
		if (Station_Manager.instance.current_station != 
			stationDestination){
				return false;
		}
		// Check cargo hold inventory for items
		ShipCargoHolds cargo = ShipManager.instance.shipCargo;
		foreach(ItemReference mItem in itemsToDeliver){
			if (mItem.itemName.Length <= 0)
				continue;
			if (mItem.count <= 0)
				continue;
			if(cargo.active_inventory.ContainsItem(mItem.itemName, mItem.count) == false)
				return false;
		}
		

		return true;
	}
	
}
[System.Serializable]
public struct ItemReference{
	public string itemName;
	public int count;
	public ItemType itemType;
	public ItemReference(string name, ItemType iType, int _count){
		itemName = name;
		count = _count;
		itemType = iType;
	}
}
