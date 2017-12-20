using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Machine_Data : ScriptableObject {

	public string machineName = "New Machine";
	public Sprite machineSprite;
	public RuntimeAnimatorController animatorController;
	public int tileWidth = 1;
	public int tileHeight = 2;
	public float efficiencyRate = 50;
	public float startDiffOffset = 0.1f;
	public float target_position = 0;
	public ShipSystemType systemControlled = ShipSystemType.None;
	public Machine_Controller machine_Controller;
	public abstract void Init(Machine_Controller controller);
}
