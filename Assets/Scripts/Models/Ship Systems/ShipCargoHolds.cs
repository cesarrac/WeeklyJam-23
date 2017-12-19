using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipCargoHolds : ShipSystem {

	// This system controls the main cargo (currMachine) and the other special cargos added
	public List<Machine> secondary_holds;
	public ShipCargoHolds(){
		shipSystemType = ShipSystemType.CargoHold;
		secondary_holds = new List<Machine>();
	}
	public override void AddMachine(Machine newMachine){
		// Set base cargo hold as currMachine
		if (currMachine == null){
			currMachine = newMachine;
			return;
		}
		secondary_holds.Add(newMachine);
	}
	public override void StartSystem(){
		// Instead of starting systems it checks the cargo holds to see if
		// they are putting the items in them at risk of being damaged
	}
	
}
