using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using DG.Tweening;
public class MiniGameManager : MonoBehaviour {
	public static MiniGameManager instance {get; protected set;}
	public GameObject dropGamePanel;
	public MiniGameControl miniGame_Repair;
	public SlipMGameControl miniGame_Slip;
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
	public void CancelRepairGame(){
		if (miniGame_Repair.gameObject.activeSelf == false)
			return;
		DeactivateRepairGame();
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
		if (miniGame_Repair.gameObject.activeSelf == false)
			return;
		miniGame_Repair.onGameSuccess -= OnRepairSuccess;
		miniGame_Repair.onGameFail -= OnRepairFail;
		miniGame_Repair.gameObject.SetActive(false);
	}
	
	public void StartSlipItemGame(Vector2 position, MiniGameDifficulty difficulty, Action onSuccess, Action onFail){
		if (miniGame_Slip.gameObject.activeSelf == true)
			return;
		miniGame_Slip.gameObject.SetActive(true);
		miniGame_Slip.transform.position = position + new Vector2(0, 2);
		miniGame_Slip.Initialize(difficulty);
		onDropGameSuccessCB += onSuccess;
		onDropGameFailCB += onFail;
		miniGame_Slip.onGameSuccess += OnSlipItemSuccess;
		miniGame_Slip.onGameFail += OnSlipItemFail;
	}
	void OnSlipItemSuccess(){
		if (onDropGameSuccessCB != null){
			onDropGameSuccessCB();
		}
		
		DeactivateSlipGame();
	}
	void OnSlipItemFail(){
		if (onDropGameFailCB != null){
			onDropGameFailCB();
		}
		
		DeactivateSlipGame();
	}
	public void DeactivateSlipGame(){
		if (miniGame_Slip.gameObject.activeSelf == false)
			return;
		miniGame_Slip.onGameFail -= OnSlipItemFail;
		miniGame_Slip.onGameSuccess -=  OnSlipItemSuccess;
		miniGame_Slip.gameObject.SetActive(false);
	}

	
	public void CancelMiniGames(){
		// Cancel any active mini games
	//DeactivateRepairGame();		// repair game deactivated through Courier Controller
		if (miniGame_Slip.gameObject.activeSelf == true)
			OnSlipItemSuccess();
	}
}
