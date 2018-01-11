using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station   {

	public string stationName {get; protected set;}
	public int jumpLocation {get; protected set;}
	public List<Neighbor> neighbors;
	public int neighborTotal = 1;
	public int location;
	public List<Mission> public_Jobs;
	Character stationAuthority;

	public Station(string _name){
		stationName = _name;
		neighbors = new List<Neighbor>();
		stationAuthority = new Character();
		stationAuthority.Initialize(stationName + " Authority");
		public_Jobs = new List<Mission>();
	}
	public bool AddNeighbor(Station station){
		if (neighbors.Count >= neighborTotal)
			return false;
		foreach(Neighbor neighbor in neighbors){
			if (neighbor.station == null)
				continue;
			if(neighbor.station == station)
				return false;
		}
		
		
		neighbors.Add(new Neighbor(station));
		//Debug.Log(stationName + " is now neighbors with " + station.stationName);
		return true;
	}
	public void RemoveNeighborLink(){
		neighborTotal --;
		neighborTotal = Mathf.Clamp(neighborTotal, 1, 4);
	}
	public void SetJumpLocation(int jumpLoc){
		jumpLocation = jumpLoc;
		//Debug.Log(stationName + " jump loc set to: " + jumpLocation);
	}
	public void EnterStation(){
		Station_Manager.instance.OnEnterStation(this);

		// NOTE: Right now we only generate jobs when there are NONE left
		// this should actually Generate jobs when x ammount of game days pass
		if (public_Jobs.Count <= 0)
			GeneratePublicJobs();
	}
	//Generates jobs for the public job board
	void GeneratePublicJobs(){
		// TESTING HERE! Finish this so that we generate a good random set of jobs
		// Select a neighbor within jump distance of 1
		Station pickUpStation = null;
		foreach(Neighbor neighbor in neighbors){
			if (neighbor.station.jumpLocation <= 1){
				pickUpStation = neighbor.station;
				break;
			}
		}
		if (pickUpStation == null){
			Debug.LogError(stationName + " could not create a job with a pick up station with jump location of 1 or less");
			return;
		}
		public_Jobs.Add(new Mission(stationAuthority, "For His Majesty", 
						new MissionItem[]{new MissionItem("Gochum Cookie Box", ItemType.Cargo, 1)},
						pickUpStation, this, pickUpStation.jumpLocation));
	}
	public void OnMissionAccepted(Mission job){
		if (public_Jobs.Contains(job) == false)
			return;
		public_Jobs.Remove(job);
	}
	public void ExitStation(){
		
	}
	
}
public struct Neighbor{
	public Station station;
	public int distanceToNeighbor;
	public Neighbor (Station _station){
		station = _station;
		distanceToNeighbor = 0;
	}
	public void SetDistance(int distance){
		distanceToNeighbor = distance;
	}
}
