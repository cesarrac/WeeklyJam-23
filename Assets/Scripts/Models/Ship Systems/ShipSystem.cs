using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum ShipSystemType {None, Power, Propulsion, Weapons, Nav, LifeSupport, CargoHold}
public abstract class ShipSystem  {
	public ShipSystemType shipSystemType {get; protected set;}
	public Machine_Controller currMachine;
	public Action<int> OnShipChangedCB {get; protected set;}
	public bool isSystemOn = false;
	public virtual void AddMachine(Machine_Controller newMachine){
		if (newMachine.shipSystemsControlled != shipSystemType)
			return;
		currMachine = newMachine;
	}
	public virtual bool CanStart(){
		if (currMachine != null && currMachine.CanUse() == true){
			
			return true;
		}
		else{
			return false;
		}
	}
	public virtual void StartSystem(){
		if (CanStart() == true){
			
			currMachine.UseMachine();	
			isSystemOn = true;
			// Start ship movement
			if (OnShipChangedCB != null){
				OnShipChangedCB(1);
			}
		}
		else{
			// Show UI telling player this system could not function
			StopSystem();
			Debug.Log(shipSystemType + " systems cannot start");
		}
	}
	public virtual void StopSystem(){
		isSystemOn = false;
		if (OnShipChangedCB != null){
			OnShipChangedCB(0);
		}
	}
	public void RegisterCB(Action<int> cb){
		OnShipChangedCB += cb;
	}
	public void UnRegisterCB(Action<int> cb){
		OnShipChangedCB -= cb;
	}
}
