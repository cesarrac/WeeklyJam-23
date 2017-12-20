using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Manager : MonoBehaviour {

	public static Item_Manager instance {get; protected set;}
	Machine_Data[] available_Machines;
	void Awake(){
		instance = this;
		available_Machines = Resources.LoadAll<Machine_Data>("ScriptableObjects/MachineData");
		if (available_Machines.Length <= 0)
			Debug.LogError("Machine data was not loaded by Item_Manager!");
	}	
	void Start(){

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
