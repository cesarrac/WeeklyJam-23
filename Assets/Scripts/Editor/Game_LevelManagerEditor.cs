using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Game_LevelManager))]
public class Game_LevelManagerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        //area = (AreaID) EditorGUILayout.EnumPopup("Area to Load:", area);
        
        if (GUILayout.Button("Load Area: ")){
            Game_LevelManager manager = (Game_LevelManager)target;
            manager.LoadArea();
        }
    }
}