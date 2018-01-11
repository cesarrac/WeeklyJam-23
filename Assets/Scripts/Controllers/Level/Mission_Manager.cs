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

	public void AcceptPublicJob(Mission newMission){
		active_missions.Add(newMission);
		Station_Manager.instance.current_station.OnMissionAccepted(newMission);
		Notification_Manager.instance.AddNotification("Job " + newMission.description + " accepted!");
		ShipManager.instance.TrySetDestination(Station_Manager.instance.GetStationIndex(newMission.stationPickUp));

	}
	public void CheckMissions(){
		if (active_missions.Count <= 0)
			return;
		List<int> missionsToRemove = new List<int>();
		for(int i = 0; i < active_missions.Count; i++){
			if (active_missions[i].isCompleted() == true){
				missionsToRemove.Add(i);
				completed_missions.Enqueue(active_missions[i]);
			}
			else{
				// if it's not complete, check if the items need to be picked up
				if (active_missions[i].stationPickUp == Station_Manager.instance.current_station){
					if (active_missions[i].itemsAcquired == true)
						continue;
					//string[] missionItemNames = active_missions[i].GetMisItemNames();
					foreach(MissionItem mItem in active_missions[i].itemsToDeliver){
						if(mItem.itemName.Length <= 0)
							continue;
						Item_Manager.instance.SpawnItem(mItem.itemName,mItem.itemType, new Vector2(-8f, -8f));
					}
				}
			}
		}	
		foreach(int i in missionsToRemove){
			active_missions.RemoveAt(i);
		}
	}

}
