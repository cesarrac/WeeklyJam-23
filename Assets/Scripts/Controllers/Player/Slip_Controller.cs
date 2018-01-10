using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slip_Controller : MonoBehaviour {
	float speed;
	int slipperiness;
	int grip;
	float threshold = 100;
	CountdownHelper countdown;
	CharacterMovement movementController;
	Courier_Controller courierController;
	bool isChecking = false;
	bool locked = false;
	Animator animator;
	void Awake(){
		movementController = GetComponent<CharacterMovement>();
		courierController = GetComponent<Courier_Controller>();
		movementController.onStartMove += OnStartMove;
		movementController.onStopMove += OnStopMove;
		countdown = new CountdownHelper(1);
		animator = GetComponentInChildren<Animator>();
	}
	void OnStartMove(){
		if (isChecking)
			return;

		if (courierController.iteminHand != null && 
			courierController.iteminHand.itemType == ItemType.Cargo){
			slipperiness = courierController.iteminHand.GetStat(StatType.Slipperiness);
			if (slipperiness <= 0)
				return;
			isChecking = true;
		}
	}
	void OnStopMove(){
		isChecking = false;
		countdown.Reset();
	}
	void Update(){
		if (isChecking == true){
			countdown.UpdateCountdown();
			if (countdown.isDone){
				if (SlipCheck() == true){
					SlipItem();
				}
			}
		}
	}
	bool SlipCheck(){
		if (slipperiness <= 0)
			return false;
		speed = movementController.curSpeed;
		float luck =  Random.Range(0, 10);
		float value = (slipperiness + (speed * 10)) - (grip + luck);

		Debug.Log("Checking for item slip! Value: " + value);
		if (value >= threshold){
			return true;
		}
		return false;
	}
	void SlipItem(){
		// Lock player movement
		movementController.LockMovement(true);
		animator.SetTrigger("slip");
		Debug.Log("Item slipping!!");
		// Start mini game
		MiniGameManager.instance.StartSlipItemGame(transform.position, MiniGameDifficulty.Average, OnSuccess, OnFail);
	}
	void OnSuccess(){
		// Unlock movement, reset timer, nothing changes
		movementController.LockMovement(false);
		animator.SetTrigger("pickUp");
	}
	void OnFail(){
		// Drop the item, when dropped the item calls a function to check its HP (** have not implemented this yet!)
		courierController.DropItem();
		// Unlock movement
		movementController.LockMovement(false);
	}

	void OnDisable(){
		if (movementController != null){
			movementController.onStartMove -= OnStartMove;
			movementController.onStopMove -= OnStopMove;
		}
	}
}
