using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : State {
    CountdownHelper countdown;
    float jumpTime = 2;
	public JumpState(StateType sType) : base (sType){
		countdown = new CountdownHelper(jumpTime);
	}
    public override void Enter(){
        base.Enter();
        countdown.Reset();
    }
	public override void Update(float deltaTime){
        countdown.UpdateCountdown();
        // Update Jump animations here:

        if (countdown.elapsedPercent >= 1){
            Finished();
        }
	}
    public override void Finished(){
        Game_LevelManager.instance.ReplaceStateWith(StateType.StationArrival);
    }
}