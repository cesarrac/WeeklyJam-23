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
		playerInventory = new Inventory(5);
		inventoryUI = GetComponent<InventoryUI>();
		inventoryUI.Initialize(playerInventory);
	}
	void Start(){
		MouseInput_Controller.instance.onInteract += TryInteract;
		inventoryUI.onItemSelected += OnItemSelected;
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
		if (playerInventory.AddItem(itemGobj.GetComponent<Item_Controller>().item) == false)
			return;
		if (item_held != null){
			// Item gets added to inventory but Item gobj gets pooled
			itemGobj.GetComponent<Item_Controller>().Pool();
			return;
		}
		item_held = itemGobj.GetComponent<Item_Controller>();
		item_held.transform.position = transform.position;
		item_held.transform.SetParent(this.transform);
		itemHolder.GetComponent<SpriteRenderer>().sprite = itemGobj.GetComponent<SpriteRenderer>().sprite;
		item_held.gameObject.SetActive(false);
		animator.SetTrigger("pickUp");
	}
	public void AddToInventory(Item item){
		
	}
	void DropItem(){
		if (item_held == null)
			return;
		Vector2 direction = characterMovement.facingDirection == Direction.Right ? Vector2.right : Vector2.left;
		animator.SetTrigger("drop");
		itemHolder.GetComponent<SpriteRenderer>().sprite = null;
		item_held.gameObject.SetActive(true);
		item_held.transform.SetParent(null);
		item_held.transform.position = ((Vector2)transform.position + direction);
		playerInventory.RemoveItem(item_held.item.name);
		item_held = null;
	}
	void DepositItem(){
		animator.SetTrigger("drop");
		itemHolder.GetComponent<SpriteRenderer>().sprite = null;
		playerInventory.RemoveItem(item_held.item.name);
		
		item_held.Pool();
		item_held = null;
	}
	void OnItemSelected(int itemIndex){
		Item itemSelected = playerInventory.inventory_items[itemIndex].item;
		if (itemSelected == null)
			return;
		if (itemSelected.itemType != ItemType.Cargo)
			return;
		if (item_held == null){
			GameObject itemGobj = ObjectPool.instance.GetObjectForType("Item", true, itemHolder.transform.position);
			item_held = itemGobj.GetComponent<Item_Controller>();
			item_held.transform.position = transform.position;
			item_held.transform.SetParent(this.transform);
			
			animator.SetTrigger("pickUp");
			
		}
		item_held.Initialize(playerInventory.inventory_items[itemIndex].item);
		itemHolder.GetComponent<SpriteRenderer>().sprite = item_held.GetComponent<SpriteRenderer>().sprite;
		item_held.gameObject.SetActive(false);
	}
	void OnDisable(){
		if (MouseInput_Controller.instance != null){
			MouseInput_Controller.instance.onInteract -= TryInteract;
		}
		if (inventoryUI != null)
			inventoryUI.onItemSelected -= OnItemSelected;
	}
}
