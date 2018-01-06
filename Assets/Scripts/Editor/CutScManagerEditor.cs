using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Cutscene_Manager))]
public class CutScManagerEditor : Editor {

	public override void OnInspectorGUI(){
		base.DrawDefaultInspector();
		if (GUILayout.Button("Actiate Jump Scene")){

			Cutscene_Manager tester = (Cutscene_Manager)target;
			tester.StartJumpScene();
		}

	}
}
