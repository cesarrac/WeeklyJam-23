﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Courier_Controller : MonoBehaviour {
	public LayerMask interactableMask;
	Animator animator;
	public Item_Controller item_held {get; protected set;}
	public GameObject itemHolder;
	CharacterMovement characterMovement;
	public CharacterPC characterData {get; protected set;}
	public InventoryUI inventoryUI {get; protected set;}
	Animator anim;
	bool isRepairing = false; // TODO: Implement PLAYER STATES so you don't have to use bool check!

	void OnEnable(){
		characterMovement = GetComponent<CharacterMovement>();
		animator = GetComponentInChildren<Animator>();
		inventoryUI = GetComponent<InventoryUI>();
	}
	public void Initialize(CharacterPC character){
		characterData = character;
		inventoryUI.Initialize(characterData.characterInventory, UI_Manager.instance.playerInventoryPanel);
		MouseInput_Controller.instance.onInteract += TryInteract;
		MouseInput_Controller.instance.onUse += Use;
		inventoryUI.onItemSelected += OnItemSelected;
	}
/* 	void Start(){
		inventoryUI.Initialize(playerInventory, UI_Manager.instance.playerInventoryPanel);
		MouseInput_Controller.instance.onInteract += TryInteract;
		MouseInput_Controller.instance.onUse += Use;
		inventoryUI.onItemSelected += OnItemSelected;
	} */
	void Use(){
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0;
		Tile_Data tile = TileManager.instance.GetTile(mousePosition);
		if (tile != null){
			if (tile.machine != null){
				if (item_held != null && item_held.item != null){
					if (item_held.item.itemUseType == ItemUseType.Repair){
						TryRepairMachine(tile.machine);
						isRepairing = true;
						return;
					}
				}
				
				// If player is holding nothing or not holding a tool: 
				tile.machine.DisplayMachineUI();
			}
		}
	}
	void TryRepairMachine(Machine_Controller machine){
		if (isRepairing == true)
			return;
		animator.SetTrigger("repair");
		characterMovement.LockMovement(true);
		machine.TryRepair(OnUseDone);
	}
	void OnUseDone(){
		Debug.Log("OnUseDone");
		isRepairing = false;
		animator.SetTrigger("repairDone");
		characterMovement.LockMovement(false);
	}
	void TryInteract(){
		if (item_held != null){
			// Check for a machine under the mouse to see if we are dropping this in a cargo hold
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = 0;
			Tile_Data tile = TileManager.instance.GetTile(mousePosition);
			if (tile != null){
				if (tile.machine != null){
					if (tile.machine.Interact(this.gameObject) == true){
						if (tile.machine.shipSystemsControlled == ShipSystemType.CargoHold)
							DepositItem();
					}
					return;
				}
			}
/* 
			DropItem();
			return; */
		}

		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, interactableMask);
		if (hit.collider != null){
			Debug.Log("Hit interactable!");
			if (hit.collider.gameObject.GetComponentInParent<Interactable>() != null){

				hit.collider.gameObject.GetComponentInParent<Interactable>().TryInteract(this.gameObject);
			}
		}
		else{
			// NO hit, we are probably trying to drop the currently held item
			DropItem();
		}
	}
	public void PickUpItem(GameObject itemGobj){
		Item itemToPickUp = itemGobj.GetComponent<Item_Controller>().item;
		if (characterData.characterInventory.AddItem(itemToPickUp) == false)
			return;
		if (item_held != null){
			PutAwayHeldItem();
			/* // Item gets added to inventory but Item gobj gets pooled
			itemGobj.GetComponent<Item_Controller>().Pool();
			return; */
		}
		item_held = itemGobj.GetComponent<Item_Controller>();
		item_held.Initialize(itemToPickUp);
		item_held.transform.position = transform.position;
		item_held.transform.SetParent(this.transform);
		item_held.gameObject.SetActive(false);
		// If it's cargo, place it over player's head
		if (itemGobj.GetComponent<Item_Controller>().item.itemType == ItemType.Cargo){
			itemHolder.GetComponent<SpriteRenderer>().sprite = itemGobj.GetComponent<SpriteRenderer>().sprite;
			animator.SetTrigger("pickUp");
			animator.SetBool("isCarrying", true);
		}
		
	}
	public void DropItem(){
		if (item_held == null)
			return;
		if (characterData.characterInventory.RemoveItem(item_held.item.name) == false)
			return;
		Vector2 direction = characterMovement.facingDirection == Direction.Right ? Vector2.right : Vector2.left;
		if (item_held.item.itemType == ItemType.Cargo){
			animator.SetTrigger("drop");
			animator.SetBool("isCarrying", false);
		}
			
		itemHolder.GetComponent<SpriteRenderer>().sprite = null;
		item_held.gameObject.SetActive(true);
		item_held.transform.SetParent(null);
		item_held.transform.position = ((Vector2)transform.position + direction);
		item_held = null;
	}
	void DepositItem(){
		if (characterData.characterInventory.RemoveItem(item_held.item.name) == false)
			return;
		Debug.Log("DepositItem");
		PutAwayHeldItem();
	}
	void OnItemSelected(int itemIndex){
		Item itemSelected = characterData.characterInventory.inventory_items[itemIndex].item;
		if (itemSelected == null){
			PutAwayHeldItem();
			return;
		}
		// Put away held item IF we are carrying cargo and we select a NON-cargo item (like a Tool)
		if(itemSelected.itemType != ItemType.Cargo){
				PutAwayHeldItem();
		}

		if (item_held == null){
			GameObject itemGobj = ObjectPool.instance.GetObjectForType("Item", true, itemHolder.transform.position);
			item_held = itemGobj.GetComponent<Item_Controller>();
			item_held.transform.position = transform.position;
			item_held.transform.SetParent(this.transform);
		}
		item_held.Initialize(itemSelected);
		item_held.gameObject.SetActive(false);

			// If it's cargo, place it over player's head
		if (itemSelected.itemType == ItemType.Cargo){
			itemHolder.GetComponent<SpriteRenderer>().sprite = itemSelected.sprite;
			animator.SetTrigger("pickUp");
			animator.SetBool("isCarrying", true);
		}
	}
	void PutAwayHeldItem(){
		if (item_held == null){
			return;
		}
		if (item_held.item == null){
			return;
		}
		item_held.transform.SetParent(null);
		

		if (item_held.item.itemType != ItemType.Cargo){
			item_held.Pool();
			item_held = null;
			return;
		}
		Debug.Log("Putting away item held");
		animator.SetTrigger("drop");
		animator.SetBool("isCarrying", false);
		itemHolder.GetComponent<SpriteRenderer>().sprite = null;
		item_held.Pool();
		item_held = null;
	}
	public void PoolCharacter(){
		if (MouseInput_Controller.instance != null){
			MouseInput_Controller.instance.onUse -= Use;
			MouseInput_Controller.instance.onInteract -= TryInteract;
		}
		if (inventoryUI != null)
			inventoryUI.onItemSelected -= OnItemSelected;
	}
}
