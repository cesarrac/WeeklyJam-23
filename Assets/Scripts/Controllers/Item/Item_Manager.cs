using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour {

	public static Item_Manager instance {get; protected set;}
	public ItemPrototype[] testPrototypes;
	ItemPrototype[] available_Items;
	Machine_Data[] available_Machines;
	ObjectPool pool;
	Dictionary<Item, GameObject> itemsInWorld;
	void Awake(){
		instance = this;
		itemsInWorld = new Dictionary<Item, GameObject>();
		available_Items = Resources.LoadAll<ItemPrototype>("ScriptableObjects/Item Prototypes");
		available_Machines = Resources.LoadAll<Machine_Data>("ScriptableObjects/MachineData");
		if (available_Machines.Length <= 0)
			Debug.LogError("Machine data was not loaded by Item_Manager!");
	}	
	void Start(){
		pool = ObjectPool.instance;
		SpawnItem(testPrototypes[0],  new Vector2(0, -3f));
		SpawnItem(testPrototypes[1], new Vector2(1, -3f));
	}
	public ItemPrototype GetPrototype(string itemName){
		foreach(ItemPrototype prototype in available_Items){
			if (prototype.name == itemName){
				return prototype;
			}
		}
		return null;
	}
	public void SpawnItem(ItemPrototype prototype, Vector2 position){
		GameObject itemGObj = pool.GetObjectForType("Item", true, position);
		itemGObj.GetComponent<Item_Controller>().Initialize(CreateInstance(prototype));
		Item item = itemGObj.GetComponent<Item_Controller>().item;
		if (item == null)
			return;
		itemsInWorld.Add(item, itemGObj);
	}
	public Item CreateInstance(ItemPrototype prototype){
        return Item.CreateInstance(prototype);
    }
	public Machine_Data GetMachine_Data(string itemName){
		if (available_Machines.Length < 0)
			return null;
		
		foreach(Machine_Data mData in available_Machines){
			if (mData.machineName == itemName){
				return mData;
			}
		}
		return null;
	}
	public void PoolItem(Item item){
		if (itemsInWorld.ContainsKey(item) == false)
			return;
		
		pool.PoolObject(itemsInWorld[item]);
		itemsInWorld.Remove(item);
	}
	public void HideItems(){
		if (itemsInWorld.Count <= 0)
			return;
		foreach(GameObject gobj in itemsInWorld.Values){
			gobj.SetActive(false);
		}
	}
	public void ShowItems(){
		if (itemsInWorld.Count <= 0)
			return;
		foreach(GameObject gobj in itemsInWorld.Values){
			gobj.SetActive(true);
		}
	}
}
