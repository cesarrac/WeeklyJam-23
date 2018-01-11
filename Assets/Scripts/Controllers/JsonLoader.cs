using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class JsonLoader {
    public static JsonLoader instance {get; protected set;}
    string path;
    string jsonString;
    public JsonLoader(){
        instance = this;
    }
    public List<ItemPrototype> LoadCargo(){
        path = Application.streamingAssetsPath + "/Item Prototypes/Cargo";
        string[] filePaths = Directory.GetFiles(path, "*.json*");
        Debug.Log(filePaths.Length + " cargo files found");
        return Load<ItemPrototype>(filePaths);
        //return DoLoadItem(filePaths);
    }
    public List<ItemPrototype> LoadMachineItems(){
        path = Application.streamingAssetsPath + "/Item Prototypes/Machines";
        string[] filePaths = Directory.GetFiles(path, "*.json*");
        Debug.Log(filePaths.Length + " cargo files found");
        return Load<ItemPrototype>(filePaths);
        //return DoLoadItem(filePaths);
    }
    public List<MachinePrototype> LoadMachineData(){
        path = Application.streamingAssetsPath + "/Buildables/Machine Prototypes";
        string[] filePaths = Directory.GetFiles(path, "*.json*");
        Debug.Log(filePaths.Length + " machine files found");
        return Load<MachinePrototype>(filePaths);
       // return DoLoadItem(filePaths);
    }
    public List<ProducerPrototype> LoadProducerData(){
        path = Application.streamingAssetsPath + "/Buildables/Producer Prototypes";
        string[] filePaths = Directory.GetFiles(path, "*.json*");
        Debug.Log(filePaths.Length + " machine files found");
        return Load<ProducerPrototype>(filePaths);
    }
    
    
    public static List<T> Load<T>(string[] filePaths){
        List<T> prototypes = new List<T>();
        foreach(string filePath in filePaths){
            // WARNING: Since unity creates .meta files for each .json, we have to skip them
            if (filePath.Contains(".json.meta"))
                continue;
            Debug.Log(filePath);
            prototypes.Add(LoadFromJason<T>(filePath));
        }
        return prototypes;
    }
    static T LoadFromJason<T>(string filePath){
        string jsonString = File.ReadAllText(filePath);

        return JsonUtility.FromJson<T>(jsonString);
    }

    ItemPrototype LoadItemFromJson(string folderPath){
        jsonString = File.ReadAllText(folderPath);
       
        return JsonUtility.FromJson<ItemPrototype>(jsonString);
    }
}

