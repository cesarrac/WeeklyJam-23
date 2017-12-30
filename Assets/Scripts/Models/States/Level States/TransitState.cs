using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitState : State{
	CountdownHelper countdown;
	float timeForEvent = 5;
	public TransitState(StateType sType) : base (sType){
		countdown = new CountdownHelper(timeForEvent);
	}
	public override void Enter(){
		
		// Use ship systems
		ShipManager.instance.ShipOn();
		// Show ship ui
		LevelUI_Manager.instance.DisplayShipUI();
		// Start timer
		countdown.Reset();

	}
	public override void Update(float deltaTime){
		countdown.UpdateCountdown();
		if (countdown.elapsedPercent >= 1){
			CheckForEvent();
		}
	}
	void CheckForEvent(){
		// if x == true, choose an event and make it happen
	}
}
