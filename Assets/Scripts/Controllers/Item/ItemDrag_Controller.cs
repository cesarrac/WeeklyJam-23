using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDrag_Controller : MonoBehaviour {
	Image image;
	Text countText;
	public int count {get; protected set;}
	public Item myItem {get; protected set;}
	InventoryUI inventoryUI;
	void OnEnable(){
		image = GetComponent<Image>();
		countText = GetComponentInChildren<Text>();
	}
	public void Init(Item item, InventoryUI UI){
		myItem = item;
		count = 1;
		image.sprite = item.sprite;
		countText.text = count.ToString();
		inventoryUI = UI;
	}
	void Update(){
		Vector2 point = Input.mousePosition;
		transform.position = point;
		if (Input.GetMouseButtonDown(0)){
			//Selection_Manager.instance.TryPlaceItem(this);
		}
		if (Input.GetMouseButtonDown(1)){
			CancelThisDrag();
		}
	}
	public void EndDrop(){
		//inventoryUI.CancelDragITem();
		ObjectPool.instance.PoolObject(this.gameObject);
	}
	void CancelThisDrag(){
		//inventoryUI.EndDragItem(myItem, count);
		ObjectPool.instance.PoolObject(this.gameObject);
	}	
	void OnDisable(){
		myItem = null;
		count = 0;
	}
}
