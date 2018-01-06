using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Camera_Controller : MonoBehaviour {
	public static Camera_Controller instance {get; protected set;}
	CinemachineBrain vCamBrain;

	void OnEnable(){
		vCamBrain = GetComponent<CinemachineBrain>();
	}
	void Awake(){
		instance = this;
	}
	public void SetVCamTarget(Transform target){
		vCamBrain.ActiveVirtualCamera.Follow = target;

	}
}
