using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New WeaponMachine", menuName = "Machines/New WeaponMachine")]
public class Weapons_Machine : Machine_Data {

	public override void Init(Machine_Controller controller){
		 machine_Controller = controller;
         machine_Controller.InitData(machineName, machineSprite, tileWidth, tileHeight, systemControlled, efficiencyRate, repairDifficulty);
	}
	public override void InitSystems(ShipManager shipManager){
		Debug.Log("Weapons init!");
	}
}
