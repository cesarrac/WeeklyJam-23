using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class LevelUI_Manager : MonoBehaviour {

	public static LevelUI_Manager instance {get; protected set;}
	public GameObject shipUIPanel, stationUIPanel, missionCompPanel, jobBoardPanel;
	Button jumpButton, undockButton, missionCompButton,jobBoardButton;
	List<GameObject> jobsAdded;
	Text missionCompText;
	Action onMissionCompDismissed;
	void Awake(){
		instance = this;
		jumpButton = shipUIPanel.GetComponentInChildren<Button>();
		jumpButton.onClick.AddListener(() => CallShipJump());
		undockButton = stationUIPanel.transform.GetChild(0).GetChild(1).gameObject.GetComponent<Button>();
		jobBoardButton = stationUIPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Button>();
		undockButton.onClick.AddListener(() => CallStationUndock());
		jobBoardButton.onClick.AddListener(() => DisplayJobBoard());
		missionCompButton = missionCompPanel.GetComponentInChildren<Button>();
		missionCompButton.onClick.AddListener(() => TryNextComplete());
		missionCompText = missionCompPanel.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>();
		jobsAdded = new List<GameObject>();
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
		HideJobPanel();
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
		if (jobBoardPanel.activeSelf == true){
			HideJobPanel();
			return;
		}
		jobBoardPanel.SetActive(true);
		// Fill panel with station's jobs
		Station curStation = Station_Manager.instance.current_station;
		if (curStation.public_Jobs.Count > 0){
			foreach(Mission job in curStation.public_Jobs){
				GameObject jobGobj = ObjectPool.instance.GetObjectForType("Job", true, jobBoardPanel.transform.position);
				jobGobj.transform.SetParent(jobBoardPanel.transform);
				string description = job.description;
				description += " Deliver " + job.itemsToDeliver[0].count + " " + job.itemsToDeliver[0].itemPrototype.name;
				jobGobj.GetComponentInChildren<Text>().text = description;
				jobGobj.GetComponent<Button>().onClick.AddListener(() => OnJobClicked(jobGobj, job));
				jobsAdded.Add(jobGobj);
			}
		}
	}
	void HideJobPanel(){
		if (jobsAdded.Count > 0){
			PoolJobObj(jobsAdded.ToArray());
		}
		jobBoardPanel.SetActive(false);
	}

	void OnJobClicked(GameObject button, Mission job){
		Mission_Manager.instance.AcceptPublicJob(job);
		button.transform.SetParent(null);
		button.GetComponent<Button>().onClick.RemoveAllListeners();
		PoolJobObj(button);
	}
	void PoolJobObj(GameObject jobGobj){
		jobsAdded.Remove(jobGobj);
		ObjectPool.instance.PoolObject(jobGobj);
	}
	void PoolJobObj(GameObject[] jobs){
		foreach(GameObject gobj in jobs){
			jobsAdded.Remove(gobj);
			ObjectPool.instance.PoolObject(gobj);
		}
	}
}
