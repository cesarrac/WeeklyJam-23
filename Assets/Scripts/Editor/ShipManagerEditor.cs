using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(ShipManager))]
public class ShipManagerEditor : Editor {

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		if (GUILayout.Button("Use Machines")){
			ShipManager shipManager = (ShipManager)target;
			shipManager.StartShip();
		}
	}
	
}
