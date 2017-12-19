using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipPower : ShipSystem {

	public ShipPower(){
		shipSystemType = ShipSystemType.Power;
	}
	public override void StartSystem(){
		if (currMachine == null){
			Debug.LogError("Ship power has NO machine");
			return;
		}
		if (CanStart() == false){
			// Show UI with machine condition
		}

		base.StartSystem();
	}
}
