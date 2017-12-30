using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelUI_Manager : MonoBehaviour {

	public static LevelUI_Manager instance {get; protected set;}
	public GameObject shipUIPanel, stationUIPanel, missionCompPanel, jobBoardPanel;
	Button jumpButton, undockButton, missionCompButton,jobBoardButton;
	Text missionCompText;
	Action onMissionCompDismissed;
	void Awake(){
		instance = this;
		jumpButton = shipUIPanel.GetComponentInChildren<Button>();
		jumpButton.onClick.AddListener(() => CallShipJump());
		undockButton = stationUIPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Button>();
		jobBoardButton = stationUIPanel.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Button>();
		undockButton.onClick.AddListener(() => CallStationUndock());
		jobBoardButton.onClick.AddListener(() => DisplayJobBoard());
		missionCompButton = missionCompPanel.GetComponentInChildren<Button>();
		missionCompButton.onClick.AddListener(() => TryNextComplete());
		missionCompText = missionCompPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
	}
	public void DisplayShipUI(){
		HideStationUI();
		shipUIPanel.SetActive(true);
	}
	public void HideShipUI(){
		shipUIPanel.SetActive(false);
	}
	public void DisplayNavigationButton(){

	}
	void CallShipJump(){
		Debug.Log("CallShipJump");
		ShipManager.instance.Jump();
	}
	public void DisplayStationUI(){
		stationUIPanel.SetActive(true);
	}
	public void HideStationUI(){
		stationUIPanel.SetActive(false);
	}
	void CallStationUndock(){
		Station_Manager.instance.Undock();
	}
	public void ShowCompMissionUI(Mission completed, Action tryNextCB){
		onMissionCompDismissed += tryNextCB;
		missionCompPanel.SetActive(true);
		missionCompText.text = completed.description;
	}
	void TryNextComplete(){
		if (onMissionCompDismissed != null){
			onMissionCompDismissed();
			onMissionCompDismissed = null;
		}
		missionCompPanel.SetActive(false);
	}
	void DisplayJobBoard(){
		jobBoardPanel.SetActive(true);
	}
}
