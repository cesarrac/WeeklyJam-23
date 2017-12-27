using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_Manager : MonoBehaviour {

	public static UI_Manager instance {get; protected set;}
	public GameObject playerInventoryPanel, shipInventoryPanel;
	public GameObject navPanel, cargoInventoryPanel;
	List<GameObject> activeWindows;
	string[] stationNames; // this index will be initialized by the station manager, set to the same index as the station map
	Dropdown stationDropdown;
	Text destinationStation, jumpCapacity;

	void Awake(){
		instance = this;
		activeWindows = new List<GameObject>();
		stationDropdown = navPanel.GetComponentInChildren<Dropdown>();
		destinationStation = navPanel.transform.GetChild(0).GetChild(1).GetChild(1).gameObject.GetComponent<Text>();
		jumpCapacity = navPanel.transform.GetChild(0).GetChild(1).GetChild(3).gameObject.GetComponent<Text>();
	}
	public void ShowMachineUI(ShipSystemType systemType){
		switch(systemType){
			case ShipSystemType.Nav:
				OpenNavigatorWindow();
				break;
			case ShipSystemType.CargoHold:
				OpenCargoWindow();
				break;
			default:
				Debug.Log("Could not find UI for " + systemType);
				break;
		}
	}
	void ActivatePanel(GameObject panel){
		panel.SetActive(true);
		activeWindows.Add(panel);
		
	}
	void OpenCargoWindow(){
		ActivatePanel(cargoInventoryPanel);
	}
	void OpenNavigatorWindow(){
		ActivatePanel(navPanel);
		int jumpCap = ShipManager.instance.shipNavigation.maxJumpCapacity;
		InitializeStations(Station_Manager.instance.GetStationAtDistance(jumpCap));
		jumpCapacity.text = jumpCap.ToString();
	}

	void InitializeStations(Station[] stationMap){
		stationDropdown.ClearOptions();
		stationNames = new string[stationMap.Length];
		for(int i = 0; i < stationNames.Length; i++){
			stationNames[i] = stationMap[i].stationName;
			Dropdown.OptionData option = new Dropdown.OptionData(stationNames[i]);
			stationDropdown.options.Add(option);
		}
	}
	public void OnDropdownChanged(){
		int dropdownValue = stationDropdown.value;
		if (ShipManager.instance.TrySetDestination(dropdownValue) == false){
			stationDropdown.value = 0;
			return;
		}
		destinationStation.text = stationNames[dropdownValue];
	}
	public void CloseLastActiveWindow(){
		GameObject active = activeWindows[activeWindows.Count - 1];
		active.SetActive(false);
		activeWindows.Remove(active);
	}
}
