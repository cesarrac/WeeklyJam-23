using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character_Manager : MonoBehaviour {
	public static Character_Manager instance {get; protected set;}
    Dictionary<int, Character[]> stationCharacterMap;

    Character npc_Domingo;
    ObjectPool pool;
    [HideInInspector]
    public GameObject player_GObj;

    CharacterPC playerCharacter;
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
    public void StartNewPlayer(string name){
        pool = ObjectPool.instance;
        playerCharacter = new CharacterPC();
        playerCharacter.Initialize(name);
        SpawnPlayer();
    }
    public void SpawnPlayer(){
        player_GObj = pool.GetObjectForType("COURIER", true, Vector2.down);
        player_GObj.GetComponent<Courier_Controller>().Initialize(playerCharacter);
        Camera_Controller.instance.SetVCamTarget(player_GObj.transform);
    }
    public void PoolPlayer(){
        ObjectPool.instance.PoolObject(player_GObj);
    }
}