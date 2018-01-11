using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ShipMode{ OFF, ON, WAITING}
public class ShipManager : MonoBehaviour {

	public static ShipManager instance {get; protected set;}
	public ShipPower shipPower {get; protected set;}
	public ShipPropulsion shipPropulsion {get; protected set;}
	public ShipCargoHolds shipCargo {get; protected set;}
	public ShipWeapons shipWeapons {get; protected set;}
	public ShipNavigation shipNavigation {get; protected set;}
	ShipSystem[] coreSystems;
	ObjectPool pool;
	public ShipMode shipMode {get; protected set;}
	void Awake(){
		instance = this;
		coreSystems = new ShipSystem[5];
		shipPower = new ShipPower();
		coreSystems[0] = shipPower;
		shipPropulsion = new ShipPropulsion();
		coreSystems[1] = shipPropulsion;
		shipCargo = new ShipCargoHolds(GetComponent<InventoryUI>());
		coreSystems[2] = shipCargo;
		shipWeapons = new ShipWeapons();
		coreSystems[3] = shipWeapons;
		shipNavigation = new ShipNavigation();
		coreSystems[4] = shipNavigation;
		Debug.Log("Systems initalized!");
		shipMode = ShipMode.OFF;
	}
	/* void Start(){
		InitializeCurrentMachines();
	} */
	public void InitStartMachines(Item[] startingMachines, Vector2 startPos){
		if (startingMachines.Length <= 0)
			return;
		if (pool == null){
			pool = ObjectPool.instance;
		}
		for(int i = 0; i < startingMachines.Length; i++){

			if (TryPlaceMachine(startingMachines[i], startPos)){
				startPos.x += 2;
			}
		}
	}
	public bool TryPlaceMachine(Item machineItem, Vector2 placePosition){
		MachinePrototype prototype = Buildable_Manager.instance.GetMachinePrototype(machineItem.name);
		if(prototype.name == "Empty")
			return false; 
		return AddMachine(machineItem, prototype, placePosition);
	}
	public void EnterStation(){
		if (shipNavigation.destinationStationIndex < 0){
			Debug.LogError("No station destination set! index is less than 0! Must be set through nav machine");
			return;
		}
		Station curStation = Station_Manager.instance.GetStation(shipNavigation.destinationStationIndex);
		if (curStation == null)
			return;
		curStation.EnterStation();
		shipNavigation.EnterStation(shipNavigation.destinationStationIndex);
		ShipOff();
	}
	public void ExitStation(){

	}
	public bool CanUseEssentials(){
		if (shipPower.CanUse() == false){
			return false;
		}
		if (shipNavigation.CanUse() == false){
			return false;
		}
		if (shipPropulsion.CanUse() == false){
			return false;
		}
		return true;
	}
	public bool ShipOn(){
		// Try turning on Power first, if that doesn't start nothing else should
		if (CanUseEssentials() == false){
			Notification_Manager.instance.AddNotification("Essential systems are down!");
			shipMode = ShipMode.WAITING;
			return false;
		}

		if (shipMode == ShipMode.ON)
			return true;
			
		foreach(ShipSystem system in coreSystems){
			system.UseSystem();
		}
		Notification_Manager.instance.AddNotification("Systems check ... OK!");
		shipMode = ShipMode.ON;
		return true;
	}
	public void ShipOff(){
		foreach(ShipSystem system in coreSystems){
			system.StopSystem();
		}
		shipMode = ShipMode.OFF;
	}
	public bool AddMachine(Item machineItem, MachinePrototype prototype, Vector2 machinePosition){
		
		if (machineItem == null)
			return false;
		Machine machine = Buildable_Manager.instance.CreateMachineInstance(prototype);
		if (machine == null)
			return false;
		GameObject machineGObj = Buildable_Manager.instance.SpawnMachine(machine, machinePosition);
		if (machineGObj == null)
			return false;
		
		Machine_Controller mController = machineGObj.GetComponent<Machine_Controller>();
		mController.InitMachine(machineItem, machine, TileManager.instance.GetTile(machineGObj.transform.position), this);
		if (AddMachine(mController, machine) == true){
			//machine.InitSystems(this);
			return true;
		}else{
			mController.RemoveMachine();
			/* machine.transform.SetParent(null);
			machine.name = "Machine";
			pool.PoolObject(machine); */
			return false;
		}

		// TODO: Add machines that are not linked to ship systems,
		// 		like machines that produce goods
	}


	public bool AddMachine(Machine_Controller newMachine, Machine machine){
		bool canAdd = false;
		switch(machine.systemControlled){
			case ShipSystemType.CargoHold:
					canAdd = shipCargo.AddMachine(newMachine);
					shipCargo.InitCargo(machine.GetStat(StatType.Storage).GetValue());
					break;
			case ShipSystemType.Nav:
					canAdd = shipNavigation.AddMachine(newMachine);
					shipNavigation.InitJumpCapacity(machine.GetStat(StatType.JumpCapacity).GetValue());
					break;
			case ShipSystemType.Propulsion:
					canAdd = shipPropulsion.AddMachine(newMachine);
					break;
			case ShipSystemType.Power:
					canAdd = shipPower.AddMachine(newMachine);
					break;
			case ShipSystemType.Weapons:
					canAdd = shipWeapons.AddMachine(newMachine);
					break;
			
		}
		return canAdd;
	}
	public bool RemoveMachine(Machine_Controller oldMachine){
		bool canAdd = false;
		switch(oldMachine.machine.systemControlled){
			case ShipSystemType.CargoHold:
					canAdd = shipCargo.AddMachine(oldMachine);
					break;
			case ShipSystemType.Nav:
					canAdd = shipNavigation.AddMachine(oldMachine);
					break;
			case ShipSystemType.Propulsion:
					canAdd = shipPropulsion.AddMachine(oldMachine);
					break;
			case ShipSystemType.Power:
					canAdd = shipPower.AddMachine(oldMachine);
					break;
			case ShipSystemType.Weapons:
					canAdd = shipWeapons.AddMachine(oldMachine);
					break;
			
		}
		return canAdd;
	}

	public bool SystemInteract(ShipSystemType sType, GameObject user){
		foreach(ShipSystem system in coreSystems){
			if (system.shipSystemType == sType){
				return system.Interact(user);
			}
		}
		return false;
	}
	
	public bool TrySetDestination(int stationIndex){
		Debug.Log("Ship manager received station index: " + stationIndex);
		
		return shipNavigation.SetDestination(stationIndex);
	}
	public void Jump(){
		if (Station_Manager.instance.GetStation(shipNavigation.destinationStationIndex) == null){
			Notification_Manager.instance.AddNotification("Ship cannot JUMP to next station because the destination index has not been set to a legal station");
			return;
		}
		if (shipNavigation.CanUse() == false){
			Notification_Manager.instance.AddNotification("Ship Manager cannot JUMP because ship NAVIGATION systems cannot be used");
			return;
		}
		if (shipPropulsion.CanUse() == false){
			Notification_Manager.instance.AddNotification("Ship Manager cannot JUMP because ship PROPULSION systems cannot be used");
			return;
		}
		Game_LevelManager.instance.ChangeStateTo(StateType.Jump);
	}
}
