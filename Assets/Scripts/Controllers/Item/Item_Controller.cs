using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Controller : MonoBehaviour {

	public Item item {get; protected set;}
	SpriteRenderer spriteRenderer;
	void OnEnable(){
		spriteRenderer = GetComponent<SpriteRenderer>();
	}
	public void Initialize(Item _item){
		item = _item;
		spriteRenderer.sprite = item.sprite;
		// Check the tile under this item and make it DIRTY!
		Tile_Data tile = TileManager.instance.GetTile(transform.position);
		if (tile == null){
			Debug.Log("Item found no tile on position " + transform.position);
			
			return;
		}
		tile.IncreaseDirtness();
	}
	public void Pool(){
		Item_Manager.instance.PoolItem(item);
		item = null;
	}
}
