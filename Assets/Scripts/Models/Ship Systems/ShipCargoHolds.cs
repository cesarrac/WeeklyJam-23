using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCargoHolds : ShipSystem {

	// This system controls the main cargo (currMachine) and the other special cargos added
	//public List<Machine_Controller> secondary_holds;
	public Inventory active_inventory {get; protected set;}
	InventoryUI inventoryUI;
	public ShipCargoHolds(InventoryUI _inventoryUI){
		shipSystemType = ShipSystemType.CargoHold;
		inventoryUI = _inventoryUI;
		inventoryUI.onItemSelected += OnItemSelected;
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
	public override void UseSystem(){
		// Instead of starting systems it checks the cargo holds to see if
		// they are putting the items in them at risk of being damaged
	}
	public void InitCargo(int maxSpacesInNewCargo){
		if (currMachine == null)
			return;
		if (active_inventory != null)
			return;
		active_inventory = new Inventory(maxSpacesInNewCargo);
		inventoryUI.Initialize(active_inventory, UI_Manager.instance.shipInventoryPanel);
	}
	
	public override bool Interact(GameObject user){
		if (base.Interact(user) == false)
			return false;
		Item newItem = user.GetComponent<Courier_Controller>().iteminHand;    //item_held.item;
		if (newItem == null)
			return false;
		if (active_inventory.AddItem(newItem) == false){
			Debug.Log("Could NOT add item " + newItem.name + " to cargo hold inventory!");
			return false;
		}
		Debug.Log("Added " + newItem.name + " to Cargo Hold");
		return true;
	}

	void OnItemSelected(int itemIndex){
		if (active_inventory.inventory_items[itemIndex].item == null)
			return;
		currMachine.AnimateOn();
		Item item = active_inventory.inventory_items[itemIndex].item;
		if (active_inventory.RemoveItem(item.name, 1) == false)
			return;
		Item_Manager.instance.SpawnItem(item, currMachine.transform.position + Vector3.down);
	}

	
}
