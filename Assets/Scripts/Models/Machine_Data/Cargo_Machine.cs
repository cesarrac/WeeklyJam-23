using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New CargoMachine", menuName = "Machines/New CargoMachine")]
public class Cargo_Machine : Machine_Data {

	public int maxCargoSpace = 10;
	public override void Init(Machine_Controller controller){
		 machine_Controller = controller;
         machine_Controller.InitData(machineName, machineSprite, animatorController, tileWidth, tileHeight, systemControlled, efficiencyRate, repairDifficulty);
	}
	public override void InitSystems(ShipManager shipManager){
		shipManager.shipCargo.InitCargo(maxCargoSpace);
	}
}
