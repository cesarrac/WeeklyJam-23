using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character{

    public string characterName {get; protected set;}
    public Sprite characterSprite {get; protected set;}
    public virtual void Initialize(string _name){
        characterName = _name;
    }
    
    
}