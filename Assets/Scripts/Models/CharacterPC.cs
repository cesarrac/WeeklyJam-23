using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterPC : Character {
    
    public override void Initialize(string _name, int invMaxSpaces = 3){
        base.Initialize(_name, invMaxSpaces);
        Debug.Log("Player character initialized with name: " + characterName);
    }
    
    
}