﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipNavigation : ShipSystem {

	public int maxJumpCapacity = 0;
	public int destinationStationIndex {get; protected set;}
	public int currStationIndex = -1;
	public ShipNavigation(){
		shipSystemType = ShipSystemType.Nav;
		destinationStationIndex = 0;
	}
	public void InitJumpCapacity(int cap){
		maxJumpCapacity = cap;
		Debug.Log("Jump capacity initialized!");
	}
	public void EnterStation(int currentStationIndex){
		if (Station_Manager.instance.GetStation(currentStationIndex) == null)
			return;
		currStationIndex = currentStationIndex;
	}
	public bool SetDestination(int stationIndex){
		// verify if max jump capacity is >= the destination jump location - current station location
		Station destination = Station_Manager.instance.GetStation(stationIndex);
		if (destination == null)
			return false;
		if (destination.jumpLocation > maxJumpCapacity)
			return false;
		
		destinationStationIndex = stationIndex;
		Notification_Manager.instance.AddNotification("Destination set to " + destination.stationName);
		return true;
	}

}
