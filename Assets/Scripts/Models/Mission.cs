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
	public int stationDestination {get; protected set;}
	// Where the mission was generated/received
	public int stationOrigin {get; protected set;}
	public int jumpsRequired {get; protected set;}
	public Mission (Character _owner, string desc, MissionItem[] deliveryItems, int originIndex, int destinationIndex, int jumpRequirement){
		owner = _owner;
		description = desc;
		itemsToDeliver = deliveryItems;
		stationOrigin = originIndex;
		stationDestination = destinationIndex;
		jumpsRequired = jumpRequirement;
		Debug.Log("Mission created to deliver " +  itemsToDeliver[0].count + " " + itemsToDeliver[0].itemPrototype.name);
	}

	// Has this mission been completed?
	public bool isCompleted() {
		if (ShipManager.instance.shipNavigation.currStationIndex != 
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
