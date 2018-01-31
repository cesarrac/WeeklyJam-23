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
        if (player_GObj != null){
            player_GObj.GetComponent<CharacterMovement>().LockMovement(false);
            player_GObj.SetActive(true);
            return;
        }
        player_GObj = pool.GetObjectForType("COURIER", true, Vector2.down);
        player_GObj.GetComponent<Courier_Controller>().Initialize(playerCharacter);
        player_GObj.GetComponent<CharacterMovement>().EnterNewArea(TileManager.instance.minWalkablePos, TileManager.instance.maxWalkablePos);
        Camera_Controller.instance.SetVCamTarget(player_GObj.transform);
    }
    public void EnterNewArea(){
        player_GObj.GetComponent<CharacterMovement>().EnterNewArea(TileManager.instance.minWalkablePos, TileManager.instance.maxWalkablePos);
    }
    public void HidePlayer(){
        player_GObj.GetComponent<CharacterMovement>().LockMovement(true);
        player_GObj.GetComponent<Courier_Controller>().CancelRepair();
        player_GObj.SetActive(false);
    }
    public void PoolPlayer(){
        ObjectPool.instance.PoolObject(player_GObj);
    }
}