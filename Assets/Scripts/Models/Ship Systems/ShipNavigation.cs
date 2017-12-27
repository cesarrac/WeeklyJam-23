using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipNavigation : ShipSystem {

	public int maxJumpCapacity = 0;
	public int destinationStationIndex = -1;
	public int currStationIndex = -1;
	public ShipNavigation(){
		shipSystemType = ShipSystemType.Nav;
	}
	public void InitJumpCapacity(int cap){
		maxJumpCapacity = cap;
		Debug.Log("Jump capacity initialized!");
	}
	public void SetCurrentStation(int currentStationIndex){
		if (Station_Manager.instance.GetStation(currentStationIndex) == null)
			return;
		currStationIndex = currentStationIndex;
	}
	public bool SetDestination(int stationIndex){
		// verify if max jump capacity is >= the destination jump location - current station location
		if (Station_Manager.instance.GetStation(stationIndex) == null)
			return false;
		
		destinationStationIndex = stationIndex;
		Debug.Log("Destination set to index: " + destinationStationIndex);
		return true;
	}
}
