using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MiniGameTester))]
public class MGameTesterEditor : Editor {

	public override void OnInspectorGUI(){
		base.DrawDefaultInspector();
		if (GUILayout.Button("Actiate slip game")){

			MiniGameTester tester = (MiniGameTester)target;
			tester.ActivateSlipGame();
		}

	}
}
