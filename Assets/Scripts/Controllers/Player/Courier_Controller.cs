using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Courier_Controller : MonoBehaviour {
	public LayerMask interactableMask;
	Animator animator;
	public Item iteminHand {get; protected set;}
	public GameObject itemHolder;
	CharacterMovement characterMovement;
	public CharacterPC characterData {get; protected set;}
	public InventoryUI inventoryUI {get; protected set;}
	Animator anim;
	bool isUsing = false; // TODO: Implement PLAYER STATES so you don't have to use bool check!
	Machine_Controller machineUsed;
	public GameObject toolHolder;
	void OnEnable(){
		characterMovement = GetComponent<CharacterMovement>();
		animator = GetComponentInChildren<Animator>();
		inventoryUI = GetComponent<InventoryUI>();
	}
	public void Initialize(CharacterPC character){
		characterData = character;
		inventoryUI.Initialize(characterData.characterInventory, UI_Manager.instance.playerInventoryPanel);
		MouseInput_Controller.instance.onInteract += RightClickInteract;
		MouseInput_Controller.instance.onUse += Use;
		inventoryUI.onItemSelected += OnItemSelected;
	}
	void Use(){
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0;
		Tile_Data tile = TileManager.instance.GetTile(mousePosition);
		if (tile != null){
			if (tile.machine != null){
				if (iteminHand != null){
					if (iteminHand.itemUseType == ItemUseType.Repair){
						TryRepairMachine(tile.machine);
						isUsing = true;
						return;
					}
				}
				
				// If player is holding nothing or not holding a tool: 
				tile.machine.DisplayMachineUI();
			}
		}
	}
	void TryRepairMachine(Machine_Controller machine){
		if (isUsing == true)
			return;
		machineUsed = machine;
		DoToolHolder(true);
		animator.SetTrigger("repair");
		characterMovement.LockMovement(true);
		machineUsed.TryRepair(OnUseDone);
	}
	public void CancelRepair(){
		if (isUsing == true && machineUsed != null){

			machineUsed.CancelRepair();
			return;
		}
	}
	void OnUseDone(){
		Debug.Log("OnUseDone");
		isUsing = false;
		animator.SetTrigger("repairDone");
		DoToolHolder(false);
		characterMovement.LockMovement(false);
		machineUsed = null;
	}
	void DoToolHolder(bool enable){
		if (enable){
			toolHolder.SetActive(true);
			toolHolder.GetComponent<Animator>().runtimeAnimatorController = Sprite_Manager.instance.GetAnimator(iteminHand.name);
		}else{
			toolHolder.SetActive(false);
		}
	}
	void RightClickInteract(){
		if (isUsing == true && machineUsed != null){

			machineUsed.CancelRepair();
			return;
		}
		if (iteminHand!= null){
			// Check for a machine under the mouse to see if we are dropping this in a cargo hold
			Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			mousePosition.z = 0;
			Tile_Data tile = TileManager.instance.GetTile(mousePosition);
			if (tile != null){
				if (tile.machine != null){
					if (tile.machine.Interact(this.gameObject) == true){
						if (tile.machine.machine.systemControlled == ShipSystemType.CargoHold)
							DepositItem();
					}
					return;
				}
			}
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
		if (iteminHand != null){
			PutAwayHeldItem();
		}
		itemGobj.GetComponent<Item_Controller>().Pool();
		iteminHand = itemToPickUp;
		// If it's cargo, place it over player's head
		if (iteminHand.itemType == ItemType.Cargo){
			itemHolder.GetComponent<SpriteRenderer>().sprite = iteminHand.sprite;
			animator.SetTrigger("pickUp");
			animator.SetBool("isCarrying", true);
		}
		
	}
	public void DropItem(){
		if (iteminHand == null)
			return;
		if (characterData.characterInventory.RemoveItem(iteminHand.name) == false)
			return;
		Vector2 direction = characterMovement.facingDirection == Direction.Right ? Vector2.right : Vector2.left;
	
		Item_Manager.instance.SpawnItem(iteminHand, (Vector2)transform.position + direction);
		PutAwayHeldItem();
	}
	void DepositItem(){
		if (iteminHand == null){
			Debug.Log("ITEM IN HAND IS NULL");
			return;
		}
		Debug.Log("Depositing item " + iteminHand.name);
		if (characterData.characterInventory.RemoveItem(iteminHand.name) == false)
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

		iteminHand = itemSelected;
			// If it's cargo, place it over player's head
		if (itemSelected.itemType == ItemType.Cargo){
			itemHolder.GetComponent<SpriteRenderer>().sprite = itemSelected.sprite;
			animator.SetTrigger("pickUp");
			animator.SetBool("isCarrying", true);
		}
	}
	void PutAwayHeldItem(){
		if (iteminHand == null)
			return;
		if (iteminHand.itemType != ItemType.Cargo){
			iteminHand = null;
			return;
		}
		animator.SetTrigger("drop");
		animator.SetBool("isCarrying", false);
		itemHolder.GetComponent<SpriteRenderer>().sprite = null;
		iteminHand = null;
	}
	public void PoolCharacter(){
		if (MouseInput_Controller.instance != null){
			MouseInput_Controller.instance.onUse -= Use;
			MouseInput_Controller.instance.onInteract -= RightClickInteract;
		}
		if (inventoryUI != null)
			inventoryUI.onItemSelected -= OnItemSelected;
	}
}
