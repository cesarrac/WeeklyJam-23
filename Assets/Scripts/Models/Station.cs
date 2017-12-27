using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station   {

	public string stationName {get; protected set;}
	public int jumpLocation {get; protected set;}
	public List<Neighbor> neighbors;
	public int neighborTotal = 1;
	public int location;
	public Station(string _name){
		stationName = _name;
		neighbors = new List<Neighbor>();
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
