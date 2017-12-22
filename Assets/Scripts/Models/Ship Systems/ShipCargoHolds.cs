using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCargoHolds : ShipSystem {

	// This system controls the main cargo (currMachine) and the other special cargos added
	//public List<Machine_Controller> secondary_holds;
	Inventory active_inventory;
	public ShipCargoHolds(){
		shipSystemType = ShipSystemType.CargoHold;
		//secondary_holds = new List<Machine_Controller>();
		//active_inventories = new Dictionary<Machine_Controller, Inventory>();
	}
	public override bool AddMachine(Machine_Controller newMachine){
		// Set base cargo hold as currMachine
		if (currMachine == null){
			currMachine = newMachine;
			
			return true;
		}
		//secondary_holds.Add(newMachine);
		return false;
	}
	public override void StartSystem(){
		// Instead of starting systems it checks the cargo holds to see if
		// they are putting the items in them at risk of being damaged
	}
	public void InitCargo(int maxSpacesInNewCargo){
		if (currMachine == null)
			return;
		if (active_inventory != null)
			return;
		active_inventory = new Inventory(maxSpacesInNewCargo);
		
	}
	
	public override bool Interact(GameObject user){
		Item newItem = user.GetComponent<Courier_Controller>().item_held.item;
		if (newItem == null)
			return false;
		if (active_inventory.AddItem(newItem) == false){
			Debug.Log("Could NOT add item " + newItem.name + " to cargo hold inventory!");
			return false;
		}
		Debug.Log("Added " + newItem.name + " to Cargo Hold");
		return true;
	}
}
