using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Producer_Controller : Machine_Controller {

	Producer producer;
	Item itemInProduction;
	Item_Manager item_Manager;
	float timeToCreate;
	CountdownHelper timer;
	Inventory storage_inventory;
	bool isProducing;
	public SpriteRenderer growth_visuals;
	
	public void Init(Item producerAsItem, Producer _producer, Tile_Data _baseTile){
		producer = _producer;
		if (_baseTile.AddProducer(producer) == false){
			return;
		}
		base.InitMachine(producerAsItem, producer, _baseTile, ShipManager.instance);
		
		// The machine controller sets the list of Neighbor tiles,
		// then we Add the producers on neighbor tiles as well
		foreach (Tile_Data tile in neighborTiles)
		{
			tile.AddProducer(producer);
		}
		item_Manager = Item_Manager.instance;
		timer = new CountdownHelper(0);
		storage_inventory = new Inventory(1);
		// Position the growth visuals X correctly according to the machine's tile width
		if (growth_visuals != null){
			growth_visuals.transform.localPosition = new Vector2(producer.tileWidth > 1 ? 0.5f : 0,0.5f);
		}
		Debug.Log("Initialized a producer with a blueprint for " + producer.current_Blueprint.itemProduced.itemName);
		// This runs when a producer has already started producing before
		if (producer.current_Blueprint.itemProduced.count > 0 && producer.productionStage >= 0){
			StartProduction(true);
			return;
		}

		SetProductionStage(0);

	

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
		if (producer.SetCurrentBlueprint(controller.iteminHand.name) == false){
				return false;
		}
		if (controller.HasItem(producer.current_Blueprint.keyIngredient.itemName, producer.current_Blueprint.keyIngredient.count) == false)
			return false;
		if (producer.current_Blueprint.secondaryIngredients.Length > 0){
			for (int i = 0; i < producer.current_Blueprint.secondaryIngredients.Length; i++)
			{
				if (controller.HasItem(producer.current_Blueprint.secondaryIngredients[i].itemName, producer.current_Blueprint.secondaryIngredients[i].count) == false)
					return false;
			}
		}
		return true;
	}
	void ChargeItems(Courier_Controller controller){
		if (controller.RemoveItem(producer.current_Blueprint.keyIngredient.itemName, producer.current_Blueprint.keyIngredient.count) == false)
			return;
		if (producer.current_Blueprint.secondaryIngredients.Length > 0){
			foreach (ItemReference itemRef in producer.current_Blueprint.secondaryIngredients)
			{
				if (controller.RemoveItem(itemRef.itemName, itemRef.count) == false)
					break;
			}
		}
	}
	public void DebugStartProduction(){
		producer.DebugSetBlueprint();
		StartProduction();
	}
	void StartProduction(bool forcedStart = false){
		Debug.Log("STARTING PRODUCTION ON " + producer.current_Blueprint.itemProduced.itemName);
		// Grab an instance of the item about to be produced
		itemInProduction = item_Manager.CreateInstance(item_Manager.GetPrototype(producer.current_Blueprint.itemProduced.itemName));
		timeToCreate = itemInProduction.timeToCreate;
			// Set timer
		if (forcedStart == true){
			float timePassed = (itemInProduction.timeToCreate * 0.25f) * producer.productionStage;
			timer.Reset(timeToCreate, timePassed);
		}
		else{
			timer.Reset(timeToCreate);
		}
	
		SetProductionStage(timer.elapsedPercent, forcedStart);
		isProducing = true;
		AnimateStayOn();
	}
	private void Update() {
		if (isProducing == true){
			timer.UpdateCountdown();
			SetProductionStage(timer.elapsedPercent);
			if (timer.elapsedPercent >= 1){
				CompleteProduction();
			}
		}
	}
	void CompleteProduction(){
		// Add the item to this producer's storage
		if(storage_inventory.AddItem(itemInProduction, producer.current_Blueprint.itemProduced.count)){
			Notification_Manager.instance.AddNotification(machine.name + " finished " + itemInProduction.name);
		}
		// Reset
		producer.ResetCurrentBlueprint();
		itemInProduction = null;
		isProducing = false;
		AnimateOff();
	}
	void SetProductionStage(float elapsedPercent, bool forceVisuals = false){
		if(elapsedPercent <= 0){
			producer.productionStage = -1;
			SetGrowthVisuals();
			return;
		}
		int pStage = producer.productionStage;
		if (elapsedPercent > 0 && elapsedPercent < 0.25f){
			pStage = 0;
		}else if (elapsedPercent >= 0.25f && elapsedPercent < 0.5f){
			pStage = 1;
		}else if (elapsedPercent >= 0.5f && elapsedPercent < 0.75f){
			pStage = 2;
		}else if (elapsedPercent >= 0.75f && elapsedPercent <= 1){
			pStage = 3;
		}
		if (pStage == producer.productionStage && forceVisuals == false)
			return;

		producer.productionStage = pStage;
		SetGrowthVisuals();
	}
	void SetGrowthVisuals(){
		if (growth_visuals == null)
			return;
		if(producer.showsGrowth == false)
			return;
		if (producer.productionStage < 0){
			growth_visuals.sprite = null;
			growth_visuals.gameObject.SetActive(false);
			return;
		}
		if (itemInProduction == null)
			return;
		Sprite growthSprite = Sprite_Manager.instance.GetSprite(itemInProduction.name + "_" + producer.productionStage.ToString());
		if (growthSprite != null){
			if (growth_visuals.gameObject.activeSelf == false){
				growth_visuals.gameObject.SetActive(true);
			}
			growth_visuals.sprite = growthSprite;
		}
	}
	public override void UserUseMachine(){
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
		SetProductionStage(0);
	}

	public override void RemoveFromTile(){
		if (baseTile != null){
			if (baseTile.RemoveProducer() == true){
				Debug.Log(producer.name + " removed from tile");
			}
		}
		if (neighborTiles != null && neighborTiles.Count > 0){
            foreach(Tile_Data tile in neighborTiles){
                tile.RemoveProducer();
            }
        }
        base.RemoveFromTile();
	}
	public override void Pool(){
		  if (animator != null)
            animator.runtimeAnimatorController = null;
		// Pool this machine's gameobject
        this.gameObject.name = "Producer";

        Buildable_Manager.instance.PoolBuildable(producer);
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