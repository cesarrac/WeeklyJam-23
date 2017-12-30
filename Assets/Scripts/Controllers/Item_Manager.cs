using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour {

	public static Item_Manager instance {get; protected set;}
	public ItemPrototype[] testPrototypes;
	ItemPrototype[] available_Items;
	Machine_Data[] available_Machines;
	ObjectPool pool;
	void Awake(){
		instance = this;
		available_Items = Resources.LoadAll<ItemPrototype>("ScriptableObjects/Item Prototypes");
		available_Machines = Resources.LoadAll<Machine_Data>("ScriptableObjects/MachineData");
		if (available_Machines.Length <= 0)
			Debug.LogError("Machine data was not loaded by Item_Manager!");
	}	
	void Start(){
		pool = ObjectPool.instance;
		GameObject testObj = pool.GetObjectForType("Item", true, new Vector2(0, -3f));
		testObj.GetComponent<Item_Controller>().Initialize(CreateInstance(testPrototypes[0]));
		GameObject testObj2 = pool.GetObjectForType("Item", true, new Vector2(1, -3f));
		testObj2.GetComponent<Item_Controller>().Initialize(CreateInstance(testPrototypes[1]));
	}
	public ItemPrototype GetPrototype(string itemName){
		foreach(ItemPrototype prototype in available_Items){
			if (prototype.name == itemName){
				return prototype;
			}
		}
		return null;
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
}
