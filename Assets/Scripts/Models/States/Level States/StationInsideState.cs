using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationInsideState : State {
	public StationInsideState(StateType sType) : base (sType){
		
	}
	public override void Enter(){
		base.Enter();
		// Initialize the Station's characters
		// ...so that they can generate missions to display them.
		// ... and check if the Player has a mission currently and if so...
		Mission_Manager.instance.CheckMissionComplete();
		// ... complete the current mission
		if (Mission_Manager.instance.completed_missions.Count > 0){
			Game_LevelManager.instance.ChangeStateTo(StateType.MissionComplete);
			return;
		}
		
		LevelUI_Manager.instance.DisplayStationUI();
	}
	public override void Update(float deltaTime){

	}
}