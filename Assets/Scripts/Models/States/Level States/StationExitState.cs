using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationExitState : State {
	public StationExitState(StateType sType) : base (sType){
		
	}
	public override void Enter(){
		LevelUI_Manager.instance.HideStationUI();
		// Do leave station cutscene
		CutsceneFinished();
	}
	public override void Update(float deltaTime){

	}
	public void CutsceneFinished(){
		// NOTE: Here we pop to get back to state 0 (Transit)
		Game_LevelManager.instance.stateMachine.Pop();
	}
}
