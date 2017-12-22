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
	}
	public void Pool(){
		item = null;
		ObjectPool.instance.PoolObject(this.gameObject);
	}
}
