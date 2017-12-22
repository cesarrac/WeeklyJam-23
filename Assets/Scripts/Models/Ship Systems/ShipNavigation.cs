using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipNavigation : ShipSystem {

	public int maxJumpCapacity = 0;
	public ShipNavigation(){
		shipSystemType = ShipSystemType.Nav;
	}
	public override void StartSystem(){
		if (currMachine != null){
			currMachine.UseMachine();	
		}
	}
	public void InitJumpCapacity(int cap){
		maxJumpCapacity = cap;
		Debug.Log("Jump capacity initialized!");
	}
}
