using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour {

	public static ShipManager instance {get; protected set;}
	public ShipPower shipPower {get; protected set;}
	public ShipPropulsion shipPropulsion {get; protected set;}
	public ShipCargoHolds shipCargo {get; protected set;}
	public ShipWeapons shipWeapons {get; protected set;}
	public ShipNavigation shipNavigation {get; protected set;}
	ShipSystem[] coreSystems;
	ObjectPool pool;

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

			Machine_Data data = Item_Manager.instance.GetMachine_Data(startingMachines[i].name);
			if(data == null)
				continue;
			if (AddMachine(data, startPos)){
				startPos.x += data.tileWidth;
			}
		}
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
		shipNavigation.SetCurrentStation(shipNavigation.destinationStationIndex);
		ShipOff();
	}
	public void ExitStation(){

	}
	public void ShipOn(){
		foreach(ShipSystem system in coreSystems){
			system.UseSystem();
		}
	}
	public void ShipOff(){
		foreach(ShipSystem system in coreSystems){
			system.StopSystem();
		}
	}
	void UpdateShipSystems(){
		
	}

	public bool AddMachine(Machine_Data data, Vector2 machinePosition){
		if (data == null)
			return false;
		GameObject machine = pool.GetObjectForType("Machine", true, machinePosition);
		machine.transform.SetParent(this.transform);
		Machine_Controller mController = machine.GetComponent<Machine_Controller>();
		data.Init(mController);

		if (AddMachine(mController) == true){
			mController.InitMachine(TileManager.instance.GetTile(machine.transform.position), this);
			data.InitSystems(this);
			return true;
		}else{
			machine.transform.SetParent(null);
			machine.name = "Machine";
			pool.PoolObject(machine);
			return false;
		}
	}


	public bool AddMachine(Machine_Controller newMachine){
		bool canAdd = false;
		switch(newMachine.shipSystemsControlled){
			case ShipSystemType.CargoHold:
					canAdd = shipCargo.AddMachine(newMachine);
					break;
			case ShipSystemType.Nav:
					canAdd = shipNavigation.AddMachine(newMachine);
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
		switch(oldMachine.shipSystemsControlled){
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
			Debug.LogError("Ship cannot JUMP to next station because the destination index has not been set to a legal station");
			return;
		}
		if (shipNavigation.CanUse() == false){
			Debug.LogError("Ship Manager cannot JUMP because ship NAVIGATION systems cannot be used");
			return;
		}
		if (shipPropulsion.CanUse() == false){
			Debug.LogError("Ship Manager cannot JUMP because ship PROPULSION systems cannot be used");
			return;
		}
		Game_LevelManager.instance.ChangeStateTo(StateType.Jump);
	}
}
