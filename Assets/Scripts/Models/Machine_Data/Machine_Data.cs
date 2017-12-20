using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "New Machine", menuName = "Machines/New Machine")]
public class Machine_Data : ScriptableObject {

	public string machineName = "New Machine";
	public Sprite machineSprite;
	public RuntimeAnimatorController animatorController;
	public int tileWidth = 1;
	public int tileHeight = 2;
	public float efficiencyRate = 50;
	public float startDiffOffset = 0.1f;
	public ShipSystemType systemControlled = ShipSystemType.None;
	[HideInInspector]
	public Machine_Controller machine_Controller;
	public MiniGameDifficulty repairDifficulty;
	public void Init(Machine_Controller controller)
	{
        machine_Controller = controller;
        machine_Controller.InitData(machineName, machineSprite, tileWidth, tileHeight, systemControlled, efficiencyRate, repairDifficulty);
    }
}
