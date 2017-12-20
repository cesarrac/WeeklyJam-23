using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MiniGameManager : MonoBehaviour {
	public static MiniGameManager instance {get; protected set;}
	public GameObject miniGamePanel;
	MiniGame miniGame;
	Machine_Controller current_machine;
	public Button repairButton, cancelButton;
	public Text machineConditionText;

	void Awake(){
		instance = this;
		miniGame = miniGamePanel.GetComponent<MiniGame>();
		repairButton.onClick.AddListener(() => miniGame.CheckForGoal());
		cancelButton.onClick.AddListener(() => DeactivateGame());
		machineConditionText.text = string.Empty;
	}
	public void StartMiniGame(Machine_Controller _machine){
		current_machine = _machine;
		if (current_machine == null)
			return;

		miniGamePanel.SetActive(true);
		miniGame.onGameSuccess += OnGameSuccess;
		miniGame.onGameFail += OnGameFail;

		miniGame.StartMiniGame(current_machine.machineCondition, current_machine.repairDifficulty);
		machineConditionText.text = current_machine.machineCondition.ToString();
	}
	void OnGameSuccess(){
		DeactivateGame();
		if (current_machine == null)
			return;
		current_machine.RepairCondition();
	}
	void OnGameFail(){
		DeactivateGame();	
	}
	void DeactivateGame(){
		miniGame.onGameSuccess -= OnGameSuccess;
		miniGame.onGameFail -= OnGameFail;
		miniGame.Deactivate();
		miniGamePanel.SetActive(false);
	}
	void Update(){
		if (Input.GetKeyDown(KeyCode.T)){
				miniGamePanel.SetActive(true);
				miniGame.onGameSuccess += OnGameSuccess;
				miniGame.onGameFail += OnGameFail;
				miniGame.StartMiniGame(MachineCondition.OK, MiniGameDifficulty.Easy);
		}
	}

}
