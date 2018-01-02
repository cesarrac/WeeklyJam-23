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
		Station origin = Station_Manager.instance.GetStation(0);
		Station dest = origin.neighbors[0].station;
		Mission testMission = new Mission(testCharacter, "A test mission!", deliveryItems, origin, dest, dest.jumpLocation);
		Debug.Log(testCharacter.characterName + " generated a new Mission!");
		Debug.Log(testMission.description + " to deliver: " + testMission.itemsToDeliver[0].count + " " + testMission.itemsToDeliver[0].itemPrototype.name);
		Debug.Log("Deliver from " + testMission.stationOrigin.stationName + " to " + testMission.stationDestination.stationName);
	
		active_missions.Add(testMission);

		if (ShipManager.instance.shipCargo.active_inventory.ContainsItem(deliveryItems[0].itemPrototype.name, 1) == true){
			Debug.Log("Cargo has the cookies man!");
		}
	}
	public void AcceptPublicJob(Mission newMission){
		active_missions.Add(newMission);
		Station_Manager.instance.current_station.OnMissionAccepted(newMission);
		Notification_Manager.instance.AddNotification("Job " + newMission.description + " accepted!");
		ShipManager.instance.TrySetDestination(Station_Manager.instance.GetStationIndex(newMission.stationDestination));
		foreach(MissionItem item in newMission.itemsToDeliver){
			Item_Manager.instance.SpawnItem(item.itemPrototype, new Vector2(-8f, -8f));
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
