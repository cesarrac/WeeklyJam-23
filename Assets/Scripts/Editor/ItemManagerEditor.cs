using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Item_Manager))]
public class ItemManagerEditor : Editor {
    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        if (GUILayout.Button("Spawn Debug Item")){
            Item_Manager manager = (Item_Manager)target;
            manager.DebugSpawnItem();
        }
        if (GUILayout.Button("Add Item to Player")){
            Item_Manager manager = (Item_Manager)target;
            manager.DebugAddItemToPlayer();
        }
    }
}