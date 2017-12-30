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
		public_Jobs.Add(new Mission(stationAuthority, "For His Majesty", new MissionItem[]{new MissionItem(Item_Manager.instance.GetPrototype("Gochum Cookie Box"), 1)}, 0, 1,1));
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
