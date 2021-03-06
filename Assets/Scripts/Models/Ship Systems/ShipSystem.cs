﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[Serializable]
public enum ShipSystemType {None, Power, Propulsion, Weapons, Nav, LifeSupport, CargoHold}
public class ShipSystem  {
	public ShipSystemType shipSystemType {get; protected set;}
	public Machine_Controller currMachine;
	public Action<int> OnShipChangedCB {get; protected set;}
	public bool isSystemOn = false;
	public virtual bool CanAddMachine(Machine_Controller newMachine){
		if (newMachine == null)
			return false;
		if (newMachine.machine == null)
			return false;
		if (newMachine.machine.systemControlled != shipSystemType)
			return false;
		if (currMachine != null){
			if (currMachine.machine.name == newMachine.machine.name)
				return true;
			else
				return false;
		}
		return true;
	}
	public virtual bool AddMachine(Machine_Controller newMachine){
		if (CanAddMachine(newMachine) == false)
			return false;
		currMachine = newMachine;
		return true;
	}
	public virtual bool CanUse(){
		if (currMachine != null && currMachine.CanUse() == true){
			
			return true;
		}
		else{
			return false;
		}
	}
	public virtual void UseSystem(){
		if (CanUse() == true){
			
			currMachine.UseMachine();	
			isSystemOn = true;
			
			// Ship changed to ON (1)
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

		// Ship changed to OFF (0)
		if (OnShipChangedCB != null){
			OnShipChangedCB(0);
		}
	}
	public virtual bool Interact(GameObject user){
		if (user == null)
			return false;
		if (Vector2.Distance(currMachine.transform.position, user.transform.position) > 1.5f)
			return false;
		Debug.Log(user.name + " interacting with system " + shipSystemType);
		return true;
	}
	public virtual bool RemoveMachine(){
		if (currMachine == null)
			return false;
		currMachine = null;
		return true;
	}
	public void RegisterCB(Action<int> cb){
		OnShipChangedCB += cb;
	}
	public void UnRegisterCB(Action<int> cb){
		OnShipChangedCB -= cb;
	}
}
