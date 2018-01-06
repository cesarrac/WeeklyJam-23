using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : State {
    Cutscene_Manager cutscene_Manager;
	public JumpState(StateType sType) : base (sType){

	}
    public override void Enter(){
        base.Enter();
        if (cutscene_Manager == null)
            cutscene_Manager = Cutscene_Manager.instance;

        cutscene_Manager.onJumpComplete += JumpComplete;
        cutscene_Manager.StartJumpScene();
        Character_Manager.instance.PoolPlayer();
        Item_Manager.instance.HideItems();
        
    }
	public override void Update(float deltaTime){
       
	}
    void JumpComplete(){
        cutscene_Manager.onJumpComplete -= JumpComplete;
        Character_Manager.instance.SpawnPlayer();
        Item_Manager.instance.ShowItems();
        Finished();
    }
    public override void Finished(){
        Game_LevelManager.instance.ReplaceStateWith(StateType.StationArrival);
    }
}