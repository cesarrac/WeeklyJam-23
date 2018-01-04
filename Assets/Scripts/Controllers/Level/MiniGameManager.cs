using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
public class MiniGameManager : MonoBehaviour {
	public static MiniGameManager instance {get; protected set;}
	public GameObject dropGamePanel;
	MiniGame miniGame_dropItem;
	public MiniGameControl miniGame_Repair;
	public SlipMGameControl miniGame;
	Machine_Controller current_machine;
	Action onDropGameSuccessCB;
	Action onDropGameFailCB;
	void Awake(){
		instance = this;
	}
	public void StartRepairGame(Machine_Controller _machine){
		if (miniGame_Repair.gameObject.activeSelf == true)
			return;
		current_machine = _machine;
		if (current_machine == null)
			return;

		Vector2 gamePosition = current_machine.transform.position + new Vector3(0, 4, 0);
		miniGame_Repair.gameObject.SetActive(true);
		miniGame_Repair.transform.position = gamePosition;
		miniGame_Repair.Initialize(current_machine.machineCondition, current_machine.repairDifficulty);
		miniGame_Repair.onGameSuccess += OnRepairSuccess;
		miniGame_Repair.onGameFail += OnRepairFail;
	}
	void OnRepairSuccess(){
		if (current_machine == null)
			return;
		current_machine.RepairSuccess();
		DeactivateRepairGame();
	}
	void OnRepairFail(){
		if (current_machine == null)
			return;
		
		current_machine.RepairFail();
		DeactivateRepairGame();	
	}
	void DeactivateRepairGame(){
		miniGame_Repair.onGameSuccess -= OnRepairSuccess;
		miniGame_Repair.onGameFail -= OnRepairFail;
		miniGame_Repair.gameObject.SetActive(false);
	}
	
	public void StartDropItemGame(Vector2 position, MiniGameDifficulty difficulty, Action onSuccess, Action onFail){
		if (miniGame.gameObject.activeSelf == true)
			return;
		miniGame.gameObject.SetActive(true);
		miniGame.transform.position = position + new Vector2(0, 2);
		miniGame.Initialize(difficulty);
		onDropGameSuccessCB += onSuccess;
		onDropGameFailCB += onFail;
		miniGame.onGameSuccess += OnDropItemSuccess;
		miniGame.onGameFail += OnDropItemFail;
	/* 	dropGamePanel.SetActive(true);
		onDropGameSuccessCB += onSuccess;
		onDropGameFailCB += onFail;
		miniGame_dropItem.onGameSuccess += OnDropItemSuccess;
		miniGame_dropItem.onGameFail += OnDropItemFail;
		miniGame_dropItem.Initialize(difficulty); */
	}
	public void CheckForGoal_DropItem(){
		if (miniGame_dropItem == null)
			return;
		if (dropGamePanel.activeSelf == false){
			return;
		}
		miniGame_dropItem.CheckForGoal();
	}
	void OnDropItemSuccess(){
		if (onDropGameSuccessCB != null){
			onDropGameSuccessCB();
		}
		
		DeactivateDropItemGame();
	}
	void OnDropItemFail(){
		if (onDropGameFailCB != null){
			onDropGameFailCB();
		}
		
		DeactivateDropItemGame();
	}
	public void DeactivateDropItemGame(){
		miniGame.onGameFail -= OnDropItemFail;
		miniGame.onGameSuccess -=  OnDropItemSuccess;
		miniGame.gameObject.SetActive(false);
	/* 	miniGame_dropItem.onGameSuccess -= OnDropItemSuccess;
		miniGame_dropItem.onGameFail -= OnDropItemFail;
		miniGame_dropItem.Deactivate();
		dropGamePanel.SetActive(false);
		onDropGameFailCB = null;
		onDropGameSuccessCB = null; */
	}

}
