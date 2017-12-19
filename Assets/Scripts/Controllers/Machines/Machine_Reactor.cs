using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine_Reactor : Machine {

	
	public override void InitMachine(TileData tile, ShipManager ship){
		if (tile == null){
			Debug.LogError("Trying to Inititalize machine " + machineName + " with a NULL tile!");
			return;
		}
		base.InitMachine(tile, ship);
	}
	public override void UseMachine(){
	
		// Check if machine gets decayed by normal use
		// Check if event occurs to machine that affects its condition

	}
	public override void DestroyMachine(){
		// Do special action for reactor destroyed
		base.DestroyMachine();
	}
	void OnDisable(){
		DestroyMachine();
	}
}
