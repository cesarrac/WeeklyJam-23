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
		shipCargo = new ShipCargoHolds();
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
	
	public void StartShip(){
		StartSystems();
	}
	void StartSystems(){
		foreach(ShipSystem system in coreSystems){
			system.StartSystem();
		}
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
				system.Interact(user);
				return true;
			}
		}
		return false;
	}
	
}
