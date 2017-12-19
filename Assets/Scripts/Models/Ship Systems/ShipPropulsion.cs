using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class ShipPropulsion : ShipSystem {


	public ShipPropulsion(){
		shipSystemType = ShipSystemType.Propulsion;
	}
	
	public override void StartSystem(){
		if (currMachine == null){
			Debug.LogError("Ship propulsion has NO machine");
			return;
		}
		if (CanStart() == false){
			// Show UI with machine condition
		}

		base.StartSystem();
	}

}
