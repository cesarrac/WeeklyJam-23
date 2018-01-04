using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameTester : MonoBehaviour {

	public SlipMGameControl slipMiniGame;
	public void ActivateSlipGame(){
		slipMiniGame.gameObject.SetActive(true);
		slipMiniGame.onGameSuccess += OnSuccess;
		slipMiniGame.onGameFail += OnFail;
		slipMiniGame.Initialize(MiniGameDifficulty.Average);

	}
	void OnSuccess(){
		Debug.Log("Success!!");
		DeactivateSlipGame();
	}
	void OnFail(){
		Debug.Log("Fail!");
		DeactivateSlipGame();
	}
	void DeactivateSlipGame(){

		slipMiniGame.onGameSuccess -= OnSuccess;
		slipMiniGame.onGameFail -= OnFail;
		slipMiniGame.gameObject.SetActive(false);
	}
}
