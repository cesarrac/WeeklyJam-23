using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipLifeSupport : ShipSystem {

	public ShipLifeSupport(){
		shipSystemType = ShipSystemType.LifeSupport;
	}
	public override void StartSystem(){
		if (currMachine != null){
			currMachine.UseMachine();	
		}
	}
}
