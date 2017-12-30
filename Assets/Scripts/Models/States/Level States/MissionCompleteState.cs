using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionCompleteState : State {
    Mission_Manager mission_Manager;
    Mission compMission;
	public MissionCompleteState(StateType sType) : base (sType){
		
	}
	public override void Enter(){
		base.Enter();
        if (mission_Manager == null){
            mission_Manager = Mission_Manager.instance;
        }
       
        TryComplete();
	}
	public override void Update(float deltaTime){

	}
    public void TryComplete(){
        if (mission_Manager.completed_missions.Count <= 0){
            Finished();
            return;
        }
        compMission = mission_Manager.completed_missions.Dequeue();
        if (compMission == null){
            Finished();
            return;
        }
        if (compMission.isCompleted() == false){
            Finished();
            return;
        }
        LevelUI_Manager.instance.ShowCompMissionUI(compMission, TryComplete);
    }
    public override void Finished(){
        base.Finished();
        Game_LevelManager.instance.stateMachine.Pop();
    }
}