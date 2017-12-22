using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New NavMachine", menuName = "Machines/New NavMachine")]
public class Nav_Machine : Machine_Data {

	public int maxJumpCapacity = 1;
	public override void Init(Machine_Controller controller){
		 machine_Controller = controller;
         machine_Controller.InitData(machineName, machineSprite, animatorController,tileWidth, tileHeight, systemControlled, efficiencyRate, repairDifficulty);
	}
	public override void InitSystems(ShipManager shipManager){
		shipManager.shipNavigation.InitJumpCapacity(maxJumpCapacity);
	
	}
}
