using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character{

    public string characterName {get; protected set;}
    public Sprite characterSprite {get; protected set;}
    public Inventory characterInventory;
    public virtual void Initialize(string _name, int invMaxSpace = 3){
        characterName = _name;
        characterInventory = new Inventory(invMaxSpace);
    }
    
    
}