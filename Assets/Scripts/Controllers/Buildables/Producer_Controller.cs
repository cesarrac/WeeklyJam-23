﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Producer_Controller : Machine_Controller {

	Producer producer;
	ProductionBlueprint current_Blueprint;
	Item itemInProduction;
	Item_Manager item_Manager;
	float timeToCreate;
	CountdownHelper timer;
	Inventory storage_inventory;
	bool isProducing;
	public void Init(Item producerAsItem, Producer _producer, Tile_Data baseTile){
		producer = _producer;
		base.InitMachine(producerAsItem, producer, baseTile, ShipManager.instance);
		item_Manager = Item_Manager.instance;
		timer = new CountdownHelper(0);
		storage_inventory = new Inventory(producer.GetStat(StatType.Storage).GetValue());
	//	Debug.Log("Producer INITIALIZED: " + " key ingredient = " + producer.productionBlueprints[0].keyIngredient.count + " " + producer.productionBlueprints[0].keyIngredient.itemName + 
		//		 " secondary ingredient " + producer.productionBlueprints[0].secondaryIngredients[0].count + " " + producer.productionBlueprints[0].secondaryIngredients[0].itemName);
	}
	public override void Interact(GameObject user){
		Debug.Log("Interacting with " + producer.name);
		if (isProducing == true)
			return;

		Courier_Controller controller = user.GetComponent<Courier_Controller>();
		if (controller == null)
			return;
		// Try to start production with item in user's hand
		if (CanStart(controller) == true){
			if (HasRequiredItems(controller)){
				ChargeItems(controller);
				StartProduction();
			}
		}
	}
	bool CanStart(Courier_Controller controller){
		if (storage_inventory.IsFull())
			return false;

		if (controller.iteminHand == null)
			return false;
		
		return true;
	}
	bool HasRequiredItems(Courier_Controller controller){
		foreach (ProductionBlueprint blueprint in producer.productionBlueprints)
		{
			if (controller.iteminHand.name == blueprint.keyIngredient.itemName){
				current_Blueprint = blueprint;
				Debug.Log("User has key ingredient: " + current_Blueprint.keyIngredient.itemName);
				break;
			}
		}
		if (current_Blueprint.keyIngredient.count <= 0){
			return false;
		}
		if (controller.HasItem(current_Blueprint.keyIngredient.itemName, current_Blueprint.keyIngredient.count) == false)
			return false;
		if (current_Blueprint.secondaryIngredients.Length > 0){
			for (int i = 0; i < current_Blueprint.secondaryIngredients.Length; i++)
			{
				if (controller.HasItem(current_Blueprint.secondaryIngredients[i].itemName, current_Blueprint.secondaryIngredients[i].count) == false)
					return false;
			}
		}
		return true;
	}
	void ChargeItems(Courier_Controller controller){
		if (controller.RemoveItem(current_Blueprint.keyIngredient.itemName, current_Blueprint.keyIngredient.count) == false)
			return;
		if (current_Blueprint.secondaryIngredients.Length > 0){
			foreach (ItemReference itemRef in current_Blueprint.secondaryIngredients)
			{
				if (controller.RemoveItem(itemRef.itemName, itemRef.count) == false)
					break;
			}
		}
	}
	void StartProduction(){
		Debug.Log("STARTING PRODUCTION ON " + current_Blueprint.itemProduced.itemName);
		// Grab an instance of the item about to be produced
		itemInProduction = item_Manager.CreateInstance(item_Manager.GetPrototype(current_Blueprint.itemProduced.itemName));
		// Set timer
		timeToCreate = itemInProduction.timeToCreate;
		timer.Reset(timeToCreate);

		isProducing = true;
		AnimateStayOn();
	}
	private void Update() {
		if (isProducing == true){
			timer.UpdateCountdown();
			if (timer.elapsedPercent >= 1){
				CompleteProduction();
			}
		}
	}
	void CompleteProduction(){
		// Add the item to this producer's storage
		if(storage_inventory.AddItem(itemInProduction, current_Blueprint.itemProduced.count)){
			Notification_Manager.instance.AddNotification(machine.name + " finished " + itemInProduction.name);
		}
		// Reset
		current_Blueprint.keyIngredient.count = 0;
		itemInProduction = null;
		isProducing = false;
		AnimateOff();
	}
	public override void DisplayMachineUI(){
		if (isProducing == false)
			AnimateOn();
		
		if (storage_inventory.IsEmpty())
			return;
		Inventory playerInventory = Character_Manager.instance.player_GObj.GetComponent<Courier_Controller>().characterData.characterInventory;
		if (playerInventory.HasSpaceFor(storage_inventory.inventory_items[0].count) == false)
			return;
		Item item = storage_inventory.inventory_items[0].item;
		if (item == null)
			return;
		if (storage_inventory.RemoveItem(item.name) == false)
			return;
		playerInventory.AddItem(item);
	}

	// UI to display production options and requirements
	// 	UI: dropdown menu with item options, text for selected's time to create, images
	//		of items required (horizontal layout that adds up to 4), and 1 image of the item produced
	// Production Queue - a queue that holds the calls to produce made by the player
	// Production Item - the production currently active
	// Start time - when player started producing item
	// Time to create - how long it takes to create the production item
	// Timer - 
	//to count how many seconds are left between start and time to create of item being created
}

[System.Serializable]
public struct ProductionBlueprint{
	public ItemReference keyIngredient;
	public ItemReference[] secondaryIngredients;
	public ItemReference itemProduced;
	
}