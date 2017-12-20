using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Courier_Controller : MonoBehaviour {
	public LayerMask interactableMask;
	Animator animator;
	GameObject item_held;
	public GameObject itemHolder;
	CharacterMovement characterMovement;
	void Awake(){
		characterMovement = GetComponent<CharacterMovement>();
		animator = GetComponentInChildren<Animator>();
	}
	void Start(){
		MouseInput_Controller.instance.onInteract += TryInteract;
	}

	void TryInteract(){
		if (item_held != null){
			DropItem();
			return;
		}
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, 0, interactableMask);
		if (hit.collider != null){
			Debug.Log("Hit interactable!");
			if (hit.collider.gameObject.GetComponentInParent<Interactable>() != null){

				// If already holding an item... drop it

				hit.collider.gameObject.GetComponentInParent<Interactable>().TryInteract(this.gameObject);
			}
		}
	}

	public void PickUpItem(GameObject itemGobj){
		item_held = itemGobj;
		item_held.transform.position = transform.position;
		item_held.transform.SetParent(this.transform);
		itemHolder.GetComponent<SpriteRenderer>().sprite = item_held.GetComponent<SpriteRenderer>().sprite;
		item_held.SetActive(false);
		animator.SetTrigger("pickUp");
	}
	void DropItem(){
		Vector2 direction = characterMovement.facingDirection == Direction.Right ? Vector2.right : Vector2.left;
		animator.SetTrigger("drop");
		itemHolder.GetComponent<SpriteRenderer>().sprite = null;
		item_held.SetActive(true);
		item_held.transform.SetParent(null);
		item_held.transform.position = ((Vector2)transform.position + direction);
		item_held = null;
	}
}
