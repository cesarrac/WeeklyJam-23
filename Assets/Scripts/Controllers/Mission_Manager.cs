using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mission_Manager : MonoBehaviour {
	public static Mission_Manager instance {get; protected set;}
	Character testCharacter;

	List<Mission> active_missions;
	public Queue<Mission> completed_missions {get; protected set;}
	void Awake(){
		instance = this;
		active_missions = new List<Mission>();
		completed_missions = new Queue<Mission>();
	}
	void Update(){

		if (Input.GetKeyDown(KeyCode.M)){
			Test();
		}
	}

	void Test(){
		testCharacter = new Character();
		testCharacter.Initialize("Test Character");
		MissionItem[] deliveryItems = new MissionItem[]{
			new MissionItem(Item_Manager.instance.GetPrototype("Gochum Cookie Box"), 1)
		};
		Mission testMission = new Mission(testCharacter, "A test mission!", deliveryItems, 0, 1, Station_Manager.instance.GetStation(1).jumpLocation);
		Debug.Log(testCharacter.characterName + " generated a new Mission!");
		Debug.Log(testMission.description + " to deliver: " + testMission.itemsToDeliver[0].count + " " + testMission.itemsToDeliver[0].itemPrototype.name);
		Debug.Log("Deliver from " + Station_Manager.instance.GetStation(testMission.stationOrigin).stationName + " to " + Station_Manager.instance.GetStation(testMission.stationDestination).stationName);
	
		active_missions.Add(testMission);

		if (ShipManager.instance.shipCargo.active_inventory.ContainsItem(deliveryItems[0].itemPrototype.name, 1) == true){
			Debug.Log("Cargo has the cookies man!");
		}
	}
	public void CheckMissionComplete(){
		if (active_missions.Count <= 0)
			return;
		List<int> missionsToRemove = new List<int>();
		for(int i = 0; i < active_missions.Count; i++){
			if (active_missions[i].isCompleted() == true){
				missionsToRemove.Add(i);
				completed_missions.Enqueue(active_missions[i]);
			}
			else{
				Debug.LogError(active_missions[i].description + " cannot complete because items were not found in inventory!");
				return;
			}
		}	
		foreach(int i in missionsToRemove){
			active_missions.RemoveAt(i);
		}
	}
}
