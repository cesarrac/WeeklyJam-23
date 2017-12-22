using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Courier_Controller : MonoBehaviour {
	public LayerMask interactableMask;
	Animator animator;
	public Item_Controller item_held {get; protected set;}
	public GameObject itemHolder;
	CharacterMovement characterMovement;
	public Inventory playerInventory {get; protected set;}
	public InventoryUI inventoryUI {get; protected set;}
	void Awake(){
		characterMovement = GetComponent<CharacterMovement>();
		animator = GetComponentInChildren<Animator>();
		playerInventory = new Inventory(3);
		inventoryUI = GetComponent<InventoryUI>();
		
	}
	void Start(){
		inventoryUI.Initialize(playerInventory, UI_Manager.instance.playerInventoryPanel);
		MouseInput_Controller.instance.onInteract += TryInteract;
		MouseInput_Controller.instance.onUse += Use;
		inventoryUI.onItemSelected += OnItemSelected;
	}
	void Use(){
		if (item_held == null)
			return;
		if (item_held.item == null)
			return;
		if (item_held.item.itemUseType != ItemUseType.Repair)
			return;

		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		mousePosition.z = 0;
		Tile_Data tile = TileManager.instance.GetTile(mousePosition);
		if (tile != null){
			if (tile.machine != null){
				tile.machine.TryRepair();
			}
		}
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

				// If already holding an item... drop it

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
		if (playerInventory.AddItem(itemToPickUp) == false)
			return;
		if (item_held != null){
			// Item gets added to inventory but Item gobj gets pooled
			itemGobj.GetComponent<Item_Controller>().Pool();
			return;
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
		}
		
	}
	void DropItem(){
		if (item_held == null)
			return;
		if (playerInventory.RemoveItem(item_held.item.name) == false)
			return;
		Vector2 direction = characterMovement.facingDirection == Direction.Right ? Vector2.right : Vector2.left;
		if (item_held.item.itemType == ItemType.Cargo)
			animator.SetTrigger("drop");
			
		itemHolder.GetComponent<SpriteRenderer>().sprite = null;
		item_held.gameObject.SetActive(true);
		item_held.transform.SetParent(null);
		item_held.transform.position = ((Vector2)transform.position + direction);
		item_held = null;
	}
	void DepositItem(){
		if (playerInventory.RemoveItem(item_held.item.name) == false)
			return;
		
		animator.SetTrigger("drop");
		itemHolder.GetComponent<SpriteRenderer>().sprite = null;
		
		item_held.Pool();
		item_held = null;
	}
	void OnItemSelected(int itemIndex){
		Item itemSelected = playerInventory.inventory_items[itemIndex].item;
		if (itemSelected == null){
			PutAwayHeldItem();
			return;
		}
	/* 	if (itemSelected.itemType != ItemType.Cargo)
		{
			PutAwayHeldItem();
			// Activate tool here:
			
			return;
		} */
		if (item_held != null){
			if(itemSelected.itemType != ItemType.Cargo){
				PutAwayHeldItem();
			}
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
		}
	}
	void PutAwayHeldItem(){
		if (item_held == null){
			return;
		}
		Debug.Log("Putting away item held");
		animator.SetTrigger("drop");
		itemHolder.GetComponent<SpriteRenderer>().sprite = null;
		item_held.transform.SetParent(null);
		item_held.Pool();
		item_held = null;
	}
	void OnDisable(){
		if (MouseInput_Controller.instance != null){
			MouseInput_Controller.instance.onUse -= Use;
			MouseInput_Controller.instance.onInteract -= TryInteract;
		}
		if (inventoryUI != null)
			inventoryUI.onItemSelected -= OnItemSelected;
	}
}
