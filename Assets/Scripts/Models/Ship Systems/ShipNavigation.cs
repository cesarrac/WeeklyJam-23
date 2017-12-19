using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipNavigation : ShipSystem {

	public ShipNavigation(){
		shipSystemType = ShipSystemType.Nav;
	}
	public override void StartSystem(){
		if (currMachine != null){
			currMachine.UseMachine();	
		}
	}
}
