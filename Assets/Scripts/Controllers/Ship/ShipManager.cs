using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipManager : MonoBehaviour {

	public static ShipManager instance {get; protected set;}
	
	//public Machine_Controller[] startingMachines;
	Dictionary<ShipSystemType, ShipSystem> ship_systems;
	ObjectPool pool;
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
			/* GameObject machine = pool.GetObjectForType("Machine", true, transform.position);
			machine.transform.SetParent(this.transform);
			machine.transform.localPosition = startPos;
			startPos.x += data.tileWidth;
			data.Init(machine.GetComponent<Machine_Controller>());

			if (AddMachine(machine.GetComponent<Machine_Controller>())){
				machine.GetComponent<Machine_Controller>().InitMachine(TileManager.instance.GetTile(machine.transform.position), this);
			}
			else{
				machine.transform.SetParent(null);
				machine.name = "Machine";
				pool.PoolObject(machine);
			} */
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

	public bool AddMachine(Machine_Data data, Vector2 machinePosition){
		if (data == null)
			return false;
		GameObject machine = pool.GetObjectForType("Machine", true, machinePosition);
		machine.transform.SetParent(this.transform);
		Machine_Controller mController = machine.GetComponent<Machine_Controller>();
		data.Init(mController);
		if (AddMachine(mController) == true){
			mController.InitMachine(TileManager.instance.GetTile(machine.transform.position), this);
			return true;
		}else{
			machine.transform.SetParent(null);
			machine.name = "Machine";
			pool.PoolObject(machine);
			return false;
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
