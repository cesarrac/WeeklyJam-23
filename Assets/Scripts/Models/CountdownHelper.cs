using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CountdownHelper  {

	//TimeManager timeManager;
	float timeToCount, timer;
	public float elapsedPercent {get; protected set;}
	public float elapsedTime {get; protected set;}
	public bool isDone {get; protected set;}
	public CountdownHelper(float _timeToCount){
		timeToCount = _timeToCount;
		//timeManager = TimeManager.instance;
	}
	public void UpdateCountdown(){
		if (timer >= timeToCount){
			timer = 0;
			isDone = true;
		}else{
			timer += Time.deltaTime;
			elapsedTime = timer;
			elapsedPercent = timer / timeToCount;
			if (isDone == true)
				isDone = false;
		}
	}
	public void Reset(float _timeToCount = 0){
		if (_timeToCount > 0)
			timeToCount = _timeToCount;
		timer = 0;
		elapsedPercent = 0;
		elapsedTime = 0;
		isDone = false;
	}
}
