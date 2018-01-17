using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Machine_Controller))]
public class MacControllerEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		if(GUILayout.Button("Decay Machine")){
			Machine_Controller controller = (Machine_Controller)target;
			controller.DecayCondition();
		}
	}
}
