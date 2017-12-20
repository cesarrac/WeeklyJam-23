using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Items/New Item", order= 3)]
public class ItemPrototype : ScriptableObject{
    new public string name = "New Item";
    public Sprite sprite = null;
	public ItemType itemType;
    public ItemUseType itemUseType = ItemUseType.None;
    public ItemQuality itemQuality;
	public int stackCount = 50;
    public int cost = 10;
    public Stat[] baseStats;
    public float timeToCreate = 1f;
    
}