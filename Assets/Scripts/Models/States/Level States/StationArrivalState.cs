using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationArrivalState : State {
	public StationArrivalState(StateType sType) : base (sType){
		
	}
	public override void Enter(){
		base.Enter();
		LevelUI_Manager.instance.HideShipUI();
		ShipManager.instance.EnterStation();
		// Do cutscene
		CutSceneFinished();
	}
	public override void Update(float deltaTime){

	}
	public void CutSceneFinished(){
		Game_LevelManager.instance.ReplaceStateWith(StateType.StationInside);
	}
}
