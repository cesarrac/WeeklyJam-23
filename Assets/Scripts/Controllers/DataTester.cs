using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataTester : MonoBehaviour {
	void Start(){
	/* 	MachinePrototype prototype = new MachinePrototype();
		prototype.name = "Test Machine";
		prototype.animatorController = "Test anim";
		prototype.sprite = "Item Box";
		prototype.repairDifficulty = MiniGameDifficulty.Average;
		prototype.systemControlled = ShipSystemType.Nav;
		prototype.stats = new Stat[2];
		prototype.stats[0] = new Stat(StatType.Height, 2, 0);
		prototype.stats[1] = new Stat(StatType.Width, 1, 0);
		Machine testMachine = Machine.CreateInstance(prototype);
		
        testMachine.InitController(mac_Controller); */
		new JsonLoader();
		List<ItemPrototype> iPrototypes = JsonLoader.instance.LoadCargo();
		if (iPrototypes.Count > 0)
			Debug.Log(iPrototypes[0].name);
		List<MachinePrototype> mPrototypes = JsonLoader.instance.LoadMachineData();
		if (mPrototypes.Count > 0)
			Debug.Log("Machine prototype: " + mPrototypes[0].name);
		Machine newMachine = Machine.CreateInstance(mPrototypes[0]);
		Debug.Log("New Machine: " + newMachine.name + " controls " + newMachine.systemControlled);
		for (int i = 0; i < newMachine.stats.Length; i++)
		{
			Debug.Log("Stat " + i + " " + newMachine.stats[i].statType + newMachine.stats[i].GetValue());
		}
		
	}
}
