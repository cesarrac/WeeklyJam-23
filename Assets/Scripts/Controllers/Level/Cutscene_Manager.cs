﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using Cinemachine;
using DG.Tweening;

public class Cutscene_Manager : MonoBehaviour {
	public static Cutscene_Manager instance {get; protected set;}
	public TilesPrefab[] prefabTiles;
	public GameObject shipHolder, stationHolder;
	ObjectPool pool;
	GameObject jumpParticles;
	//public CinemachineVirtualCamera virtualCamera;
	Sequence arriveSequence;
	public Text cutSceneText;
	public delegate void OnComplete();
	public event OnComplete onJumpComplete;
	void Awake(){
		instance = this;
	}
	void Start(){
		pool = ObjectPool.instance;
	}
	void InitializeJumpScene(){
		arriveSequence = DOTween.Sequence();
		if (jumpParticles == null)
			jumpParticles = pool.GetObjectForType("Jump Particles", true, Vector2.zero);
		cutSceneText.gameObject.SetActive(true);
	}
	public void StartJumpScene(){
		InitializeJumpScene();
		CameraShaker.instance.AddTrauma(6);
		arriveSequence.Append(shipHolder.transform.DOMoveY(-5, 0.75f).SetLoops(4, LoopType.Yoyo).OnComplete(() => ShipVanish()));
		
	}
	void ShipVanish(){
		arriveSequence.Append(shipHolder.transform.DOScaleX(0, 1.75f).OnComplete(()=>Arrive()));
	}
	void Arrive(){
		// Show text of where we are
		arriveSequence.Append(cutSceneText.DOFade(1, 3).OnComplete(() =>StationAppear()));
	}
	void StationAppear(){
		// TODO: load the correct tiles for the station we have arrived at
		/* Station arrivalStation = Station_Manager.instance.GetStation(ShipManager.instance.shipNavigation.destinationStationIndex);
		Debug.Log("Arrived at " + arrivalStation.stationName); */
		stationHolder.SetActive(true);
		
		arriveSequence.Append(stationHolder.transform.DOMoveY(0, 2).OnComplete(()=>ShipAppear()));
		arriveSequence.Join(cutSceneText.DOFade(0, 1.25f));
	}
	void ShipAppear(){
		arriveSequence.Append(shipHolder.transform.DOScaleX(1, 0.5f).OnComplete(()=>JumpComplete()));
	}
	void JumpComplete(){
		Debug.Log("Jump complete");
		cutSceneText.gameObject.SetActive(false);
		if (onJumpComplete != null){
			onJumpComplete();
		}
	}
}

[System.Serializable]
public struct TilesPrefab{
	public GameObject tileGridPrefab;
	public string id;

}