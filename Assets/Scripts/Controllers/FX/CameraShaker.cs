﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaker : MonoBehaviour {
	public static CameraShaker instance {get; protected set;}
	public float trauma, shake;
	float shakeAngle, shakeOffsetX, shakeOffsetY;
	float maxAngle = 10, maxOffset = 2;
	bool isShaking;
	void Awake(){
		instance = this;
	}
	void Update(){
		if (Input.GetKeyDown(KeyCode.T)){
			AddTrauma();
		}
		if (trauma > 0){
			ComputeShake();
			trauma -= 0.2f * Time.deltaTime;
			DoShake();
		}
		if (isShaking && trauma <= 0){
			trauma = 0;
			StopShake();
		}
	}
	public void AddTrauma(float multiplier = 1){
		trauma += (0.4f * multiplier);
		StartCamShake();
	}
	void StartCamShake(){
		ComputeShake();
	}
	void ComputeShake(){
		shake = Mathf.Pow(trauma, 2) * Time.deltaTime;
		shakeAngle = maxAngle * shake * Random.Range(-1f, 1f);
		shakeOffsetX = maxOffset * shake * Random.Range(-1f, 1f);
		shakeOffsetY = maxOffset * shake * Random.Range(-1f, 1f);
	}
	void DoShake(){
		if (isShaking == false)
			isShaking = true;
		transform.localPosition = new Vector2(shakeOffsetX, shakeOffsetY);
		transform.localRotation = Quaternion.Euler(0, 0, shakeAngle);
	}
	void StopShake(){
		Debug.Log("Stop shake");
		transform.localPosition = Vector2.zero;
		isShaking = false;
		//transform.Translate(-transform.position.x, 0, 0, transform.parent);
		//isShaking = false;
	}
}
