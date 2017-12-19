﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipWeapons : ShipSystem {

	public ShipWeapons(){
		shipSystemType = ShipSystemType.Weapons;
	}
	
	public override void StartSystem(){
		if (currMachine != null){
			currMachine.UseMachine();	
		}
	}
}
