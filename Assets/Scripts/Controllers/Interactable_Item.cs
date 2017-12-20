using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable_Item : Interactable {

	void OnEnable(){
		base.Init(transform.position);
	}
	public override void Interact(){
		if (interactor != null){
			// Pick up this item
			interactor.GetComponent<Courier_Controller>().PickUpItem(this.gameObject);
		}
	}
}
