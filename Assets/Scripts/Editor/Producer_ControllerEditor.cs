using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Producer_Controller))]
public class Producer_ControllerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Debug Production")){
            Producer_Controller controller = (Producer_Controller)target;
            controller.DebugStartProduction();
        }
    }
}