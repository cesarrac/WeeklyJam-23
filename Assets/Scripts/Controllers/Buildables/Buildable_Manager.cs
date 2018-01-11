using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Buildable_Manager : MonoBehaviour
{
    public static Buildable_Manager instance {get; protected set;}
    Dictionary<Buildable, GameObject> buildablesInWorld;
	MachinePrototype[] machineData_prototypes;
	ProducerPrototype[] producer_prototypes;
    ObjectPool pool;
    private void Awake() {
        instance = this;
    }
    public void Initialize(){
        buildablesInWorld = new Dictionary<Buildable, GameObject>();
        machineData_prototypes = JsonLoader.instance.LoadMachineData().ToArray();
		producer_prototypes = JsonLoader.instance.LoadProducerData().ToArray();
        pool = ObjectPool.instance;
		Producer producer = Producer.CreateInstance(producer_prototypes[0]);
		Debug.Log(producer.name + " created!");
		Debug.Log(producer.name + " produces " + producer.itemsProduced[0].itemName);
    }
    public MachinePrototype GetMachinePrototype(string itemName){
		MachinePrototype empty = new MachinePrototype();
		empty.name = "Empty";
		if (machineData_prototypes.Length < 0)
			return empty;
		
		foreach(MachinePrototype mData in machineData_prototypes){
			if (mData.name == itemName){
				return mData;
			}
		}
		return empty;
	}
	public Machine CreateMachineInstance(MachinePrototype prototype){
		return Machine.CreateInstance(prototype);
	}
	public GameObject SpawnMachine(Machine machine, Vector2 position){
		if (machine == null)
			return null;
	
		GameObject machineGObj = pool.GetObjectForType("Machine", true, position);
		if (machineGObj == null)
			return null;
		machineGObj.transform.SetParent(ShipManager.instance.transform);
		Machine_Controller mController = machineGObj.GetComponent<Machine_Controller>();
		buildablesInWorld.Add(machine, machineGObj);
		return machineGObj;
	}
	public void PoolBuildable(Buildable buildable){
		if (buildablesInWorld.ContainsKey(buildable) == false)
			return;
		if (buildablesInWorld[buildable].transform.parent != null)
			buildablesInWorld[buildable].transform.SetParent(null);
			
		pool.PoolObject(buildablesInWorld[buildable]);
		buildablesInWorld.Remove(buildable);
	}
	public void HideBuildables(){
		if (buildablesInWorld.Count <= 0)
			return;
		foreach(GameObject gobj in buildablesInWorld.Values){
			gobj.SetActive(false);
		}
	}
	public void ShowBuildables(){
		if (buildablesInWorld.Count <= 0)
			return;
		foreach(GameObject gobj in buildablesInWorld.Values){
			gobj.SetActive(true);
		}
	}
}
[System.Serializable]
public struct BuildablePrototype{
	public string name;
    public string sprite;
    public string animatorController;
    public BuildableType buildableType;
    public Stat[] stats;
}
[System.Serializable]
public struct MachinePrototype{
	public string name;
    public string sprite;
    public string animatorController;
	public int tileWidth;
	public int tileHeight;
    public Stat[] stats;
	public ShipSystemType systemControlled;
	public MiniGameDifficulty repairDifficulty;
	public MachineCondition machineCondition;
}
[System.Serializable]
public struct ProducerPrototype{
	public string name;
    public string sprite;
    public string animatorController;
	public int tileWidth;
	public int tileHeight;
    public Stat[] stats;
	public MiniGameDifficulty repairDifficulty;
	public MachineCondition machineCondition;
	public ProductionItem[] itemsProduced;
}
[System.Serializable]
public struct ProductionItem{
	public string itemName;
	public int yield; // Total number of items created when this is produced
	public ProductionRequirement[] costs;
	// the time it takes to create it is stored within the item prototype
}
[System.Serializable]
public struct ProductionRequirement{
	public string itemRequired;
	public int total;
}