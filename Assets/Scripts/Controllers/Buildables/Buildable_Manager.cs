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
    }
	public void SpawnStartingProducers(List<Item>buildableItems){
		if (buildableItems.Count <= 0)
			return;
		Vector2 startingPos = new Vector2(-3, -4);
		foreach (Item item in buildableItems)
		{	
			Tile_Data baseTile = TileManager.instance.GetTile(startingPos);
			if (baseTile == null)
				continue;
			Producer producer = CreateProducerInstance(GetProducerPrototype(item.name));
			GameObject prodGObj = SpawnProducer(producer, startingPos);
			if (prodGObj == null)
				continue;
			prodGObj.GetComponent<Producer_Controller>().Init(item, producer, baseTile);
			startingPos.x += producer.tileWidth;
		}
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
	public ProducerPrototype GetProducerPrototype(string itemName){
		ProducerPrototype empty = new ProducerPrototype();
		empty.name = "Empty";
		if (producer_prototypes.Length < 0)
			return empty;
		
		foreach(ProducerPrototype data in producer_prototypes){
			if (data.name == itemName){
				return data;
			}
		}
		return empty;
	}
	private GameObject SpawnBuildable<T>(T buildable, BuildableType buildableType, Vector2 position){
		if (buildable == null)
			return null;
		GameObject gObj = pool.GetObjectForType(buildableType.ToString(), true, position);
		if (gObj == null)
			return null;
		gObj.transform.SetParent(ShipManager.instance.transform);
		return gObj;
	}
	public Machine CreateMachineInstance(MachinePrototype prototype){
		return Machine.CreateInstance(prototype);
	}
	public GameObject SpawnMachine(Machine machine, Vector2 position){
		if (machine == null)
			return null;
		GameObject machineGObj = SpawnBuildable<Machine>(machine, BuildableType.Machine, position);
		if (machineGObj == null)
			return null;
		buildablesInWorld.Add(machine, machineGObj);
		return machineGObj;
	}
	public Producer CreateProducerInstance(ProducerPrototype prototype){
		return Producer.CreateInstance(prototype);
	}
	public GameObject SpawnProducer(Producer producer, Vector2 position){
		if (producer == null)
			return null;
		GameObject prodGObj = SpawnBuildable<Producer>(producer, BuildableType.Producer, position);
		if (prodGObj == null)
			return null;
		buildablesInWorld.Add(producer, prodGObj);
		return prodGObj;
	}
	public void PoolBuildables(){
		if (buildablesInWorld.Count <= 0)
			return;
		foreach (Buildable buildable in buildablesInWorld.Keys)
		{
			if (buildablesInWorld.ContainsKey(buildable) == false)
			return;
			if (buildablesInWorld[buildable].transform.parent != null)
				buildablesInWorld[buildable].transform.SetParent(null);
			buildablesInWorld[buildable].name = buildable.buildableType.ToString();
			pool.PoolObject(buildablesInWorld[buildable]);
		}
		buildablesInWorld.Clear();
	}
	public void PoolBuildable(Buildable buildable){
		if (buildablesInWorld.ContainsKey(buildable) == false)
			return;
		if (buildablesInWorld[buildable].transform.parent != null)
			buildablesInWorld[buildable].transform.SetParent(null);
		buildablesInWorld[buildable].name = buildable.buildableType.ToString();
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
	public ProductionBlueprint[] productionBlueprints;
	public bool showsGrowth;
	public int productionStage;
	public string curProductionName;

}