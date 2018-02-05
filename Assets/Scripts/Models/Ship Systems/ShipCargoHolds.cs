using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCargoHolds : ShipSystem {

	// This system controls the main cargo (currMachine) and the other special cargos added
	//public List<Machine_Controller> secondary_holds;
//	public Inventory active_inventory {get; protected set;}
	InventoryUI inventoryUI;
	public Inventory_Controller inventory_Controller {get; protected set;}
	Courier_Controller playerInventoryControl;
	public ShipCargoHolds(InventoryUI _inventoryUI){
		shipSystemType = ShipSystemType.CargoHold;
		inventoryUI = _inventoryUI;
		inventoryUI.onItemSelected += OnItemSelected;
		//secondary_holds = new List<Machine_Controller>();
		//active_inventories = new Dictionary<Machine_Controller, Inventory>();
	}
	public override void UseSystem(){
		// Instead of starting systems it checks the cargo holds to see if
		// they are putting the items in them at risk of being damaged
	}
	public void InitCargo(int maxSpacesInNewCargo){
		if (currMachine == null)
			return;
		inventory_Controller = currMachine.gameObject.GetComponentInChildren<Inventory_Controller>();
		if (inventory_Controller == null){
			Debug.LogError(currMachine.machine.name + " does not have the required Inventory Controller as a child! Did Buildable Manager not add it?");
			return;
		}
		inventory_Controller.Initialize(ID_Generator.instance.GetMachineID(currMachine), maxSpacesInNewCargo);
		inventoryUI.Initialize(inventory_Controller.inventory, UI_Manager.instance.shipInventoryPanel);

	/* 	if (active_inventory != null){
			// If the new inventory does not match the old, it needs to re-init
			// and place any items in active inventory into the new inventory
			if (maxSpacesInNewCargo != active_inventory.maxSpaces){
				ReInitInventory(maxSpacesInNewCargo);
			}
			return;
		}
		else{
			// Check if there's a saved inventory and LOAD it from there
			Inventory savedInventory = Inventory_Manager.instance.LoadInventory(ID_Generator.instance.GetMachineID(currMachine));
			if (savedInventory != null){
				active_inventory = savedInventory;
			}
			else{
				// Make a new one
				active_inventory = new Inventory(maxSpacesInNewCargo);
			}
		} */
		//inventoryUI.Initialize(active_inventory, UI_Manager.instance.shipInventoryPanel);
	}
/* 	public void ReInitInventory(int maxSpacesInNewCargo){
		List<InventoryItem> curInventory = new List<InventoryItem>();
		if (active_inventory.IsEmpty() == false){
			foreach (InventoryItem invItem in active_inventory.inventory_items)
			{
				if (invItem.item == null)
					continue;
				if (invItem.count <= 0)
					continue;
				curInventory.Add(invItem);
			}
		}
		active_inventory = new Inventory(maxSpacesInNewCargo);
		if (curInventory.Count <= 0)
			return;
		foreach (InventoryItem invItem in curInventory)
		{
			active_inventory.AddItem(invItem.item, invItem.count);
			// ** TODO ** :
			// 	Code in case old inventory's items do not fit into
			// 	the new inventory. The extra cargo should go into the
			//	station's cargo.
		}
	} */
	
	public override bool Interact(GameObject user){
		if (base.Interact(user) == false)
			return false;
		Item newItem = user.GetComponent<Courier_Controller>().iteminHand;    //item_held.item;
		if (newItem == null)
			return false;
		if (user.GetComponent<Courier_Controller>().RemoveItem(newItem.name, 1) == false)
			return false;
		if (inventory_Controller.inventory.AddItem(newItem) == false){
			Debug.Log("Could NOT add item " + newItem.name + " to cargo hold inventory!");
			return false;
		}
		Debug.Log("Added " + newItem.name + " to Cargo Hold");
		return true;
	}

	void OnItemSelected(int itemIndex){
		if (inventory_Controller.inventory.inventory_items[itemIndex].item == null)
			return;
		currMachine.AnimateOn();
		if (playerInventoryControl == null)
			playerInventoryControl = Character_Manager.instance.player_GObj.GetComponent<Courier_Controller>();
		
		Inventory playerInventory = playerInventoryControl.characterData.characterInventory;
		if (playerInventory.IsFull() == true)
			return;

		Item item = inventory_Controller.inventory.inventory_items[itemIndex].item;
		if (playerInventory.AddItem(item) == false)
			return;
		if (inventory_Controller.inventory.RemoveItem(item.name, 1) == false)
			return;
			// Instead of spawning item, give it to player directly
		//Item_Manager.instance.SpawnItem(item, currMachine.transform.position + Vector3.down);
	}

	
}
