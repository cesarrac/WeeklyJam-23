using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New PropulsionMachine", menuName = "Machines/New PropulsionMachine")]
public class Propulsion_Machine : Machine_Data {

	public override void Init(Machine_Controller controller){
		 machine_Controller = controller;
         machine_Controller.InitData(machineName, machineSprite, animatorController,tileWidth, tileHeight, systemControlled, efficiencyRate, repairDifficulty);
	}
	public override void InitSystems(ShipManager shipManager){
		Debug.Log("Propulsion init!");
	}
}
