using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationExitState : State {
    Cutscene_Manager cutscene_Manager;
	public StationExitState(StateType sType) : base (sType){
		
	}
	public override void Enter(){
		if (cutscene_Manager == null)
        	cutscene_Manager = Cutscene_Manager.instance;

		LevelUI_Manager.instance.HideStationUI();

		cutscene_Manager.onComplete += CutsceneFinished;
		cutscene_Manager.StartStationExit();
	}
	public override void Update(float deltaTime){

	}
	void CutsceneFinished(){
		cutscene_Manager.onComplete -= CutsceneFinished;
		// NOTE: Here we pop to get back to state 0 (Transit)
		Game_LevelManager.instance.stateMachine.Pop();
	}
}
