using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class JsonLoader: MonoBehaviour{
    public static JsonLoader instance {get; protected set;}
    string path;
    string jsonString;
    void Awake(){
        instance = this;
    }
    void Start(){
       /* TestItemPrototype fishJuice = LoadItemFromJson("Item Prototypes/Cargo", "FishJuice.json");
       Debug.Log("Item 1: " + fishJuice.name + " " + fishJuice.itemType + " " + fishJuice.stats[0].statType + " " + fishJuice.stats[0].minValue);
       TestItemPrototype letters = LoadItemFromJson("Item Prototypes/Cargo", "Letters.json");
       Debug.Log("Item 2: " + letters.name + " " + letters.itemType + " " + letters.stats[0].statType + " " + letters.stats[0].minValue); */
       
       LoadCargo();
       
    }
    void LoadCargo(){
        path = Application.streamingAssetsPath + "/Item Prototypes/Cargo";
        string[] filePaths = Directory.GetFiles(path, "*.json*");
        Debug.Log(filePaths.Length + " cargo files found");
        List<TestItemPrototype> prototypes = new List<TestItemPrototype>();
        foreach(string filePath in filePaths){
            // WARNING: Since unity creates .meta files for each .json, we have to skip them
            if (filePath.Contains(".json.meta"))
                continue;
            Debug.Log(filePath);
            prototypes.Add(LoadItemFromJson(filePath));
            Debug.Log("Item " + (prototypes.Count - 1)  + " :" + prototypes[prototypes.Count - 1].name 
            + " " + prototypes[prototypes.Count - 1].itemType + " " + prototypes[prototypes.Count - 1].stats[0].statType 
            + " " + prototypes[prototypes.Count - 1].stats[0].minValue);
        }
    }
    public TestItemPrototype LoadItemFromJson(string folderPath){
        jsonString = File.ReadAllText(folderPath);
       
        return JsonUtility.FromJson<TestItemPrototype>(jsonString);
    }
}

[System.Serializable]
public struct TestItemPrototype{
    public string name;
    public Sprite sprite;
    public ItemType itemType;
    public ItemUseType itemUseType;
    public ItemQuality itemQuality;
    public int stackCount;
    public int cost;
    public Stat[] stats;
    public float timeToCreate;
}