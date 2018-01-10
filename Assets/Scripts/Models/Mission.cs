using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission  {

	// owner: who issued this mission
	public Character owner {get; protected set;}
	public string description {get; protected set;}
	// what items and how many of them to deliver
	public MissionItem[] itemsToDeliver {get; protected set;}
	// Where to deliver
	public Station stationDestination {get; protected set;}
	// Where the mission was generated/received
	public Station stationPickUp {get; protected set;}
	public int jumpsRequired {get; protected set;}
	public bool itemsAcquired {get; protected set;} // Has the player received the items required?
	public Mission (Character _owner, string desc, MissionItem[] deliveryItems, Station _pickUp, Station _destination, int jumpRequirement){
		owner = _owner;
		description = desc;
		itemsToDeliver = deliveryItems;
		stationPickUp = _pickUp;
		stationDestination = _destination;
		jumpsRequired = jumpRequirement;
		Debug.Log("Mission created to pick up " +  itemsToDeliver[0].count + " " + 
				itemsToDeliver[0].itemPrototype.name + " at " + stationPickUp.stationName + 
				"  and deliver to " + stationDestination.stationName);
		itemsAcquired = false;
	}

	public ItemPrototype[] PickUpItems(){
		// Returns an array with the necessary items to spawn for the player to pick up
		ItemPrototype[] items = new ItemPrototype[itemsToDeliver.Length];
		for(int i = 0; i < items.Length; i++){
			if (itemsToDeliver[i].itemPrototype == null)
				continue;
			items[i] = itemsToDeliver[i].itemPrototype;
		}
		// Mark mission as picked up so we don't give the player the items more than once
		itemsAcquired = true;
		return items;
	}

	// Has this mission been completed?
	public bool isCompleted() {
		if (Station_Manager.instance.current_station != 
			stationDestination){
				return false;
		}
		// Check cargo hold inventory for items
		ShipCargoHolds cargo = ShipManager.instance.shipCargo;
		foreach(MissionItem mItem in itemsToDeliver){
			if (mItem.itemPrototype == null)
				continue;
			if (mItem.count <= 0)
				continue;
			if(cargo.active_inventory.ContainsItem(mItem.itemPrototype.name, mItem.count) == false)
				return false;
		}
		

		return true;
	}
	
}
public struct MissionItem{
	public ItemPrototype itemPrototype;
	public int count;
	
	public MissionItem(ItemPrototype prototype, int _count){
		itemPrototype = prototype;
		count = _count;
	}
}
