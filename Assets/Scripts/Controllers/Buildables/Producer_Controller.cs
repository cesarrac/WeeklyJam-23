using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Producer_Controller : MonoBehaviour {

	Producer producer;
	public void Init(Producer _producer){
		producer = _producer;
	}
	// UI to display production options and requirements
	// 	UI: dropdown menu with item options, text for selected's time to create, images
	//		of items required (horizontal layout that adds up to 4), and 1 image of the item produced
	// Production Queue - a queue that holds the calls to produce made by the player
	// Production Item - the production currently active
	// Start time - when player started producing item
	// Time to create - how long it takes to create the production item
	// Timer - 
	//to count how many seconds are left between start and time to create of item being created
}
