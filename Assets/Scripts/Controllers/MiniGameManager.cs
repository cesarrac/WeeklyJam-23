using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class MiniGameManager : MonoBehaviour {
	public static MiniGameManager instance {get; protected set;}
	public GameObject repairGamePanel, dropGamePanel;
	MiniGame miniGame_repair, miniGame_dropItem;
	Machine_Controller current_machine;
	public Button repairButton, cancelButton;
	public Text machineConditionText;
	Action onDropGameSuccessCB;
	Action onDropGameFailCB;
	void Awake(){
		instance = this;
		miniGame_repair = repairGamePanel.GetComponent<MiniGame>();
		miniGame_dropItem = dropGamePanel.GetComponent<MiniGame>();
		repairButton.onClick.AddListener(() => miniGame_repair.CheckForGoal());
		cancelButton.onClick.AddListener(() => OnRepairFail());
		machineConditionText.text = string.Empty;
	}
	public void StartRepairGame(Machine_Controller _machine){
		current_machine = _machine;
		if (current_machine == null)
			return;

		repairGamePanel.SetActive(true);
		miniGame_repair.onGameSuccess += OnRepairSuccess;
		miniGame_repair.onGameFail += OnRepairFail;

		miniGame_repair.Initialize(current_machine.machineCondition, current_machine.repairDifficulty);
		machineConditionText.text = current_machine.machineCondition.ToString();
	}
	void OnRepairSuccess(){
		DeactivateRepairGame();
		if (current_machine == null)
			return;
		current_machine.RepairSuccess();
	}
	void OnRepairFail(){
		if (current_machine != null)
			current_machine.RepairFail();
		DeactivateRepairGame();	
	}
	void DeactivateRepairGame(){
		miniGame_repair.onGameSuccess -= OnRepairSuccess;
		miniGame_repair.onGameFail -= OnRepairFail;
		miniGame_repair.Deactivate();
		repairGamePanel.SetActive(false);
	}
	
	public void StartDropItemGame(MiniGameDifficulty difficulty, Action onSuccess, Action onFail){
		dropGamePanel.SetActive(true);
		onDropGameSuccessCB += onSuccess;
		onDropGameFailCB += onFail;
		miniGame_dropItem.onGameSuccess += OnDropItemSuccess;
		miniGame_dropItem.onGameFail += OnDropItemFail;
		miniGame_dropItem.Initialize(difficulty);
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
		miniGame_dropItem.onGameSuccess -= OnDropItemSuccess;
		miniGame_dropItem.onGameFail -= OnDropItemFail;
		miniGame_dropItem.Deactivate();
		dropGamePanel.SetActive(false);
		onDropGameFailCB = null;
		onDropGameSuccessCB = null;
	}

}
