using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour {

	public static Item_Manager instance {get; protected set;}
	public string[] startItems;
	public string[] startingMachines;
	ItemPrototype[] available_Cargo;
	ItemPrototype[] available_Machines;
	ObjectPool pool;
	Dictionary<Item, GameObject> itemsInWorld;
	void Awake(){
		instance = this;
	}
	public void Initialize(){
		itemsInWorld = new Dictionary<Item, GameObject>();
	/* 	machine_datas = Resources.LoadAll<Machine_Data>("ScriptableObjects/MachineData");
		if (machine_datas.Length <= 0)
			Debug.LogError("Machine data was not loaded by Item_Manager!"); */
		
		// Load available item prototypes from Json
		available_Cargo = JsonLoader.instance.LoadCargo().ToArray();
		available_Machines = JsonLoader.instance.LoadMachineItems().ToArray();
		pool = ObjectPool.instance;
	}

	public void SpawnStartingItems(){
		SpawnItem(GetPrototype(startItems[0], ItemType.Cargo),  new Vector2(0, -3f));
		SpawnItem(GetPrototype(startItems[1], ItemType.Cargo), new Vector2(1, -3f));
		List<Item> machines = new List<Item>();
		for(int i = 0; i < startingMachines.Length; i++){
			Item newItem = CreateInstance(GetPrototype(startingMachines[i], ItemType.Machine));
			if (newItem == null)
				continue;

			machines.Add(newItem);
		}
		ShipManager.instance.InitStartMachines(machines.ToArray(),  new Vector2(-3, 0));
	}
	
	public ItemPrototype GetPrototype(string itemName, ItemType itemType){
		if (itemType == ItemType.Cargo)
			return GetCargoProto(itemName);
		if (itemType == ItemType.Machine)
			return GetMachineProto(itemName);
		ItemPrototype empty = new ItemPrototype();
		empty.name = "Empty";
		return empty;
	}
	ItemPrototype GetCargoProto(string itemName){
		foreach(ItemPrototype prototype in available_Cargo){
			if (prototype.name == itemName){
				return prototype;
			}
		}
		ItemPrototype empty = new ItemPrototype();
		empty.name = "Empty";
		return empty;
	}
	ItemPrototype GetMachineProto(string itemName){
		foreach(ItemPrototype prototype in available_Machines){
			if (prototype.name == itemName){
				return prototype;
			}
		}
		ItemPrototype empty = new ItemPrototype();
		empty.name = "Empty";
		return empty;
	}
	public Item CreateInstance(ItemPrototype prototype){
		if (prototype.name == "Empty")
			return null;
        return Item.CreateInstance(prototype);
    }
	void Spawn(Item item, Vector2 position){
		GameObject itemGObj = pool.GetObjectForType("Item", true, position);
		itemGObj.GetComponent<Item_Controller>().Initialize(item);
		itemsInWorld.Add(item, itemGObj);
	}
	public void SpawnItem(string itemName, ItemType itemType, Vector2 position){
		SpawnItem(GetPrototype(itemName, itemType), position);
	}
	public void SpawnItem(ItemPrototype prototype, Vector2 position){
		if (prototype.name == "Empty")
			return;
		Item item = CreateInstance(prototype);
		if (item == null)
			return;
		Spawn(item, position);
	}
	public void SpawnItem(Item itemInstance, Vector2 position){
		if (itemInstance == null)
			return;
		Spawn(itemInstance, position);
	}

	public void PoolItem(Item item){
		if (itemsInWorld.ContainsKey(item) == false)
			return;
		if (itemsInWorld[item].transform.parent != null)
			itemsInWorld[item].transform.SetParent(null);
			
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


[System.Serializable]
public struct ItemPrototype{
    public string name;
    public string sprite;
    public ItemType itemType;
    public ItemUseType itemUseType;
    public ItemQuality itemQuality;
    public int stackCount;
    public int cost;
    public Stat[] stats;
    public float timeToCreate;
}
