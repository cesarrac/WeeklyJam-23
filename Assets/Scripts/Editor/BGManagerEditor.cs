using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BGVisuals_Manager))]
public class BGManagerEditor : Editor {
	public AreaID currentArea;
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		currentArea = (AreaID)EditorGUILayout.EnumPopup("Current Area: ", currentArea);
		if (GUILayout.Button("Load Background")){
			BGVisuals_Manager manager = (BGVisuals_Manager)target;
			manager.LoadBgForArea(currentArea);
		}
	}
}

