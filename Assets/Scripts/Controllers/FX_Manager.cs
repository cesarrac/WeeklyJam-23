using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FX_Manager : MonoBehaviour {
	public GameObject starParticles;
	void Start(){
		ShipManager.instance.shipPropulsion.RegisterCB(OnShipPropulsion);
	}
	void OnShipPropulsion(int value){
		if (value <= 0)
			StopStars();
		else
			StartStars();
	}
	void StartStars(){
		starParticles.SetActive(true);
	}
	void StopStars(){
		starParticles.SetActive(false);
	}
}
