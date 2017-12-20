using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour {

	public static ShipManager instance {get; protected set;}
	
	public Machine_Controller[] startingMachines;
	Dictionary<ShipSystemType, ShipSystem> ship_systems;
	
	void Awake(){
		instance = this;
		ship_systems = new Dictionary<ShipSystemType, ShipSystem>();
		ship_systems.Add(ShipSystemType.Propulsion, new ShipPropulsion());
		ship_systems.Add(ShipSystemType.Nav, new ShipNavigation());
		ship_systems.Add(ShipSystemType.Weapons, new ShipWeapons());
		ship_systems.Add(ShipSystemType.LifeSupport, new ShipLifeSupport());
		ship_systems.Add(ShipSystemType.CargoHold, new ShipCargoHolds());
		ship_systems.Add(ShipSystemType.Power, new ShipPower());
	}
	void Start(){
		InitializeCurrentMachines();
	}
	void InitializeCurrentMachines(){
		if (startingMachines.Length <= 0)
			return;
		for(int i = 0; i < startingMachines.Length; i++){
			startingMachines[i].InitMachine(TileManager.instance.GetTile(startingMachines[i].transform.position), this);
			AddMachine(startingMachines[i]);
		}
	}
	
	public void StartShip(){
		StartSystems();
	}
	void StartSystems(){
		foreach(ShipSystem sSystem in ship_systems.Values){
			sSystem.StartSystem();
		}
	}
	public bool AddMachine(Machine_Controller newMachine){

		if (ship_systems.ContainsKey(newMachine.shipSystemsControlled) == false)
			return false;
		// If there's a machine already -- give up!
		if (ship_systems[newMachine.shipSystemsControlled].currMachine != null){
			return false;
		}
		ship_systems[newMachine.shipSystemsControlled].AddMachine(newMachine);
		return true;
		//Debug.Log("Added machine " + newMachine.shipSystemsControlled);
	}
	public void RemoveMachine(Machine_Controller oldMachine){

	}
	public ShipSystem GetShipSystem(ShipSystemType sType){
		if (ship_systems.ContainsKey(sType) == false)
			return null;
		return ship_systems[sType];
	}
}
