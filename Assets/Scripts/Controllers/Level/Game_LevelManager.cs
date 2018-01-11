﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_LevelManager : MonoBehaviour {
	public static Game_LevelManager instance {get; protected set;}
	public ItemPrototype[] startingMachines;
	Item[] startingItems;
	public StackFSM stateMachine {get;protected set;}
	State currentState;
	State[] LevelStates;
	void Awake(){
		instance = this;
		LevelStates = new State[]{
			new TransitState(StateType.Transit),
			new JumpState(StateType.Jump),
			new StationArrivalState(StateType.StationArrival),
			new StationInsideState(StateType.StationInside),
			new StationExitState(StateType.StationExit),
			new MissionCompleteState(StateType.MissionComplete)
		};
		stateMachine = new StackFSM();

		// Create Json data loader
		new JsonLoader();
	}
	void Start(){
		StartGame();
	}

	void StartGame(){
		Item_Manager.instance.Initialize();
		Buildable_Manager.instance.Initialize();
		TileManager.instance.GenerateTileData();
		Character_Manager.instance.StartNewPlayer("Tipo");
		Item_Manager.instance.SpawnStartingItems();
		Station_Manager.instance.Initialize();
		stateMachine.Push(LevelStates[0]);
	}

	public void ReplaceStateWith(StateType stateType){
		if (currentState.stateType == stateType)
			return;
		foreach(State state in LevelStates){
			if (state.stateType == stateType){
				stateMachine.Replace(state);
			}
		}
	}
	public void ChangeStateTo(StateType stateType){
		if (currentState.stateType == stateType)
			return;
			foreach(State state in LevelStates){
			if (state.stateType == stateType){
				stateMachine.Push(state);
				break;
			}
		}
	}
	void Update(){
		currentState = stateMachine.GetCurrentState();
		if (currentState != null){
			currentState.Update(Time.deltaTime);
		}
	}
}
