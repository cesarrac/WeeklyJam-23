using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitState : State{
	CountdownHelper countdown;
	float timeForEvent = 5;
	ShipManager shipManager;
	LevelUI_Manager levelUI;
	float timePerJump = 10f;
	float transitProgress = 0;
	float lastEventTime, timeBwEvent = 5;
	public TransitState(StateType sType) : base (sType){
		countdown = new CountdownHelper(timePerJump);
	}
	public override void Enter(){
		if (shipManager == null)
			shipManager = ShipManager.instance;
		if (levelUI == null)
			levelUI = LevelUI_Manager.instance;

		// First check that essential systems can turn on and Use ship systems
		if (shipManager.ShipOn() == false){
			// Push the Idle state and wait for systems to be fixed before continuing
			// This state should stop updating progress to station
		}
		// Show ship ui
		levelUI.DisplayShipUI();
		// Start timer
		countdown.Reset();

		lastEventTime = 0;
	}
	public override void Update(float deltaTime){
	 	countdown.UpdateCountdown();
		if (countdown.elapsedPercent >= 1){
			// Go to Jump state
			Jump();
			return;
		}
		// if at any moment we cant use essential systems, pop out
		if (shipManager.CanUseEssentials() == false){
			return;
		}
		// update progress
		transitProgress = countdown.elapsedPercent;
		levelUI.UpdateTransitProgress(transitProgress);

		// Every timeBwEvent (seconds) check for event
		if (countdown.elapsedTime - lastEventTime >= timeBwEvent){
			lastEventTime = countdown.elapsedTime;
			CheckForEvent();
		} 
	}
	void CheckForEvent(){
		
		// if x == true, choose an event and make it happen
		Debug.Log("Create EVENT here!");
	}
	void Jump(){
		Debug.Log("JUMP HERE! Time is up, we have arrived at a station!");
		// update progress
		transitProgress = countdown.elapsedPercent;
		levelUI.UpdateTransitProgress(transitProgress);
		ShipManager.instance.Jump();
	}
}
