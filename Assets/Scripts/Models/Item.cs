using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType {Cargo, Machine, Tool}
public enum ItemQuality {Terrible, Defective, Functional, Worthy, Majestic}
public enum ItemUseType {None, Build, Repair}
public class Item  {
	public string name {get; protected set;}
	public ItemType itemType {get; protected set;}
	public ItemQuality itemQuality {get; protected set;}
	public int stackCount {get; protected set;}
	public ItemUseType itemUseType {get; protected set;}
	public Stat[] stats {get; protected set;}
	public Sprite sprite {get; protected set;}
	public GameObject user;
	public int costToCreate {get; protected set;}
	protected Item(ItemPrototype b){
		this.name = b.name;
		this.itemType = b.itemType;
		this.stackCount = b.stackCount;
		this.itemQuality = b.itemQuality;
		this.stats = new Stat[b.baseStats.Length];
		for(int i = 0; i < this.stats.Length; i++){
			this.stats[i] = new Stat(b.baseStats[i].statType, b.baseStats[i].minValue, b.baseStats[i].maxValue);
		}
		this.sprite = b.sprite;
		this.itemUseType = b.itemUseType;
		this.costToCreate = b.cost;
	}
	public static Item CreateInstance(ItemPrototype prototype){
		return new Item(prototype);
	}
	public void RegisterUser(GameObject newUser){
		user = newUser;
	}
	public int GetStat(StatType statType, int modifier = 0){
		if (stats == null)
			return 0;
		if (stats.Length <= 0)
			return 0;
		foreach(Stat stat in stats){
			if (stat.statType == statType)
				return stat.GetValue(modifier);
		}
		
		return 0;	
	}
	/* public void UseItem(){
		if (useAction != null){
			useAction.UseItem(this);
		}
	} */
	
}
