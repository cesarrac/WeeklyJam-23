using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour {

	public static Item_Manager instance {get; protected set;}
	public string debugItem;
	public string[] startShipMachines;
	public string[] startingProducers;
	ItemPrototype[] available_Items;
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
		List<ItemPrototype> all_items = new List<ItemPrototype>();
		List<ItemPrototype> cargo = JsonLoader.instance.LoadItems("Cargo");
		foreach (ItemPrototype prototype in cargo)
		{
			all_items.Add(prototype);
		}
		List<ItemPrototype> machines = JsonLoader.instance.LoadItems("Machines");
		foreach (ItemPrototype prototype in machines)
		{
			all_items.Add(prototype);
		}
		List<ItemPrototype> tools = JsonLoader.instance.LoadItems("Tools");
		foreach (ItemPrototype prototype in tools)
		{
			all_items.Add(prototype);
		}

		available_Items = all_items.ToArray();

		pool = ObjectPool.instance;
	}
	public void DebugSpawnItem(){
		SpawnItem(GetPrototype(debugItem),  new Vector2(0, -3f));
	}
	public void DebugAddItemToPlayer(){
		GameObject playerGObj = Character_Manager.instance.player_GObj;
		if (playerGObj == null)
			return;
		Item newItem = CreateInstance(GetPrototype(debugItem));
		playerGObj.GetComponent<Courier_Controller>().characterData.characterInventory.AddItem(newItem);
	}
	public void SpawnStartingItems(){
		List<Item> machines = new List<Item>();
		for(int i = 0; i < startShipMachines.Length; i++){
			Item newItem = CreateInstance(GetPrototype(startShipMachines[i]));
			if (newItem == null)
				continue;

			machines.Add(newItem);
		}
		ShipManager.instance.InitStartMachines(machines.ToArray(),  new Vector2(-3, 0));
		if (startingProducers.Length > 0){
			List<Item> producers = new List<Item>();
			for (int i = 0; i < startingProducers.Length; i++)
			{
				Item newItem = CreateInstance(GetPrototype(startingProducers[i]));
				if (newItem == null)
					continue;
				producers.Add(newItem);
			}
			Buildable_Manager.instance.SpawnStartingProducers(producers);
		}
	}
	
	public ItemPrototype GetPrototype(string itemName){
		/* if (itemType == ItemType.Cargo)
			return GetCargoProto(itemName);
		if (itemType == ItemType.Machine)
			return GetMachineProto(itemName); */
		foreach (ItemPrototype prototype in available_Items)
		{
			if (prototype.name == itemName)
				return prototype;
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
	public void SpawnItem(string itemName, Vector2 position){
		SpawnItem(GetPrototype(itemName), position);
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
