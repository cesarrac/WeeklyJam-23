using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Manager : MonoBehaviour {
	public static Character_Manager instance {get; protected set;}
    Dictionary<int, Character[]> stationCharacterMap;

    Character npc_Domingo;

	void Awake(){
		instance = this;
        npc_Domingo = new Character();
        npc_Domingo.Initialize("Domingo");
        InitializeStationCharacters();
	}
    void InitializeStationCharacters(){
        stationCharacterMap = new Dictionary<int, Character[]>(){
            {0, new Character[]{npc_Domingo}}
        };
    }
    
}