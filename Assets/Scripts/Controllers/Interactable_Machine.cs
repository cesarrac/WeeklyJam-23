using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Machine : Interactable {

	Machine_Controller machine_Controller;

	void Awake(){
		machine_Controller = GetComponent<Machine_Controller>();
		base.Init(transform.position);
	}
	public override void Interact(){
		base.Interact();
		machine_Controller.TryRepair();
	}	
}
